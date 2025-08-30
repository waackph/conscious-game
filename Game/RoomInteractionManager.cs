using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;
using System;
using System.Linq;

namespace conscious
{
    /// <summary>Class <c>RoomInteractionManager</c> implements a interaction system 
    /// that manages all interactions between the player and the current Room and its Things.
    /// </summary>
    ///
    public class RoomInteractionManager : IComponent
    {
        private EntityManager _entityManager;
        private SoCManager _socManager;
        private InventoryManager _inventoryManager;
        private ControlsManager _controlsManager;
        private RoomManager _roomManager;
        private UiDialogManager _dialogManager;
        private SequenceManager _sequenceManager;

        private AStarShortestPath _pathfinder;

        private Cursor _cursor;
        private Player _player;

        private GameTime _gameTime;

        private ButtonState _lastButtonState;
        private Thing _thingClickedInRoom;
        private Thing _thingClickedInInventory;
        private Verb _lastVerbChosen;
        private int _maxThingsClicked;
        private Queue<Thing> _lastThingsClicked;
        private bool _isWalking;
        private bool _interactionActive;
        private List<Vector2> _path;
        private int _currentPathPoint;

        private double threshDiff = 200f;

        public RoomInteractionManager(EntityManager entityManager, 
                                      SoCManager socManager, 
                                      InventoryManager inventoryManager, 
                                      ControlsManager controlsManager,
                                      RoomManager roomManager,
                                      UiDialogManager dialogManager,
                                      SequenceManager sequenceManager,
                                      AStarShortestPath pathfinder,
                                      Cursor cursor,
                                      Player player) 
        {
            _entityManager = entityManager;
            _socManager = socManager;
            _socManager.ActionEvent += executeThoughtInteraction;
            _socManager.FinalEdgeSelected += doPlayerFinalThoughtActions;
            _inventoryManager = inventoryManager;
            _controlsManager = controlsManager;
            _roomManager = roomManager;
            _dialogManager = dialogManager;
            _sequenceManager = sequenceManager;

            _pathfinder = pathfinder;

            _cursor = cursor;
            _player = player;

            _lastThingsClicked = new Queue<Thing>();
            
            _lastButtonState = Mouse.GetState().LeftButton;
            _interactionActive = false;
            _isWalking = false;
            _maxThingsClicked = 3;
            _lastVerbChosen = Verb.None;
        }

        #region GAMELOOP

        public void Update(GameTime gameTime)
        {
            _gameTime = gameTime;
            
            Vector2 direction = Vector2.Zero;
            Vector2 mousePosition = Vector2.Zero;

            Thing thingHovered = null;
            Thing thingClicked = null;

            if(Keyboard.GetState().IsKeyDown(Keys.C))
            {
                finishInteraction(canceled:true);
            }
            else if(Keyboard.GetState().IsKeyDown(Keys.Tab) && _interactionActive && _inventoryManager.InventoryActive)
            {
                finishInteraction(canceled:true);
            }

            thingHovered = CheckCursorHoversThing();
            if(thingHovered != null)
            {
                if(!_inventoryManager.InventoryActive || (_inventoryManager.InventoryActive && thingHovered.IsInInventory))
                {
                    _cursor.InteractionLabel = thingHovered.Name;
                }
            }
            else
            {
                _cursor.InteractionLabel = "";
            }

            if(Mouse.GetState().LeftButton == ButtonState.Released && _lastButtonState == ButtonState.Pressed)
            {
                thingClicked = thingHovered;
                if(thingClicked != null)
                {
                    if(thingClicked.IsInInventory)
                    {
                        _inventoryManager.CloseInventory();
                    }
                    if(!_interactionActive)
                    {
                        triggerThought(thingClicked);
                    }
                    if(_interactionActive && isTwoPartInteraction(_lastVerbChosen))
                    {
                        // correctly assign the currently clicked thing
                        if(_thingClickedInRoom != null)
                        {
                            if(thingClicked.IsInInventory)
                            {
                                _thingClickedInInventory = thingClicked;
                            }
                        }
                        else if(_thingClickedInInventory != null)
                        {
                            if(!thingClicked.IsInInventory)
                            {
                                _thingClickedInRoom = thingClicked;
                            }
                        }
                        // if thing in room near or both in inventory => execute interaction
                        // if both not in inventory => cancel interaction
                        // if thing in room not near => start walking
                        if(_thingClickedInInventory == null)
                        {
                            finishInteraction(canceled:true);
                        }
                        else if(_thingClickedInRoom == null)
                        {
                            doInteraction(thingClicked, _thingClickedInInventory, _lastVerbChosen);
                        }
                        else
                        {
                            if(IsEntityNearPlayer(_thingClickedInRoom))
                            {
                                doInteraction(_thingClickedInRoom, _thingClickedInInventory, _lastVerbChosen);
                            }
                            else
                            {
                                startWalking(_thingClickedInRoom, _lastVerbChosen);
                            }
                        }
                    }
                    else if(_interactionActive && _isWalking && _thingClickedInRoom != thingClicked)
                    {
                        finishInteraction();
                        triggerThought(thingClicked);
                    }
                }
            }
            else if(Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                bool uiHit = false;
                // Check if a non-room entity (a UI entity) has been clicked
                foreach(UIArea entity in _entityManager.GetEntitiesOfType<UIArea>())
                {
                    if(entity.BoundingBox.Contains(_cursor.Position.X, _cursor.Position.Y))
                    {
                        uiHit = true;
                        break;
                    }
                }
                if(thingHovered == null && !uiHit)
                    mousePosition = _cursor.MouseCoordinates;
            }
            else
            {
                if(_isWalking && _interactionActive && _thingClickedInRoom != null)
                {
                    Vector2 diff = _path[_currentPathPoint] - _player.CollisionBox.Center.ToVector2();
                    if(IsEntityNearPlayer(_thingClickedInRoom) || 
                       (CheckCheckpointNear(_player, _path[_currentPathPoint]) && _path.Count == _currentPathPoint + 1)
                    )
                    {
                        if(isTwoPartInteraction(_lastVerbChosen))
                        {
                            doInteraction(_thingClickedInRoom, _thingClickedInInventory, _lastVerbChosen);
                        }
                        else if(isOnePartInteraction(_lastVerbChosen))
                        {
                            doInteraction(_thingClickedInRoom, _lastVerbChosen);
                        }
                    }
                    else
                    {
                        if(CheckCheckpointNear(_player, _path[_currentPathPoint]) && _path.Count > _currentPathPoint + 1)
                        {
                            _currentPathPoint++;
                        }
                        direction = Vector2.Normalize(diff);
                    }
                }
            }

            // Set controls manager variables to move in direction when clicked
            _controlsManager.MousePosition = mousePosition;
            _controlsManager.Direction = direction;

            // Set values for next iteration
            _lastButtonState = Mouse.GetState().LeftButton;
        }

        private Vector2 getThingCenterTopBottomPos(Entity entity, bool bottom=true)
        {
            if(bottom)
                return entity.CollisionBox.Center.ToVector2() + new Vector2(0, entity.CollisionBox.Height/2);
            else
                return entity.CollisionBox.Center.ToVector2() - new Vector2(0, entity.CollisionBox.Height/2);
        }

        public void Draw(SpriteBatch spriteBatch) {}

        private void doPlayerFinalThoughtActions(object sender, FinalEdgeEventArgs e)
        {
            if(e.verbAction == Verb.WakeUp)
            {
                _player.WakeUp();
            }
            
            if(e.seq != null)
            {
                _sequenceManager.StartSequence(e.seq, _player, e.EdgeMood);
            }
            if(e.EventThoughtId > 0)
            {
                // add thingId to link properties to get connected thing
                foreach(Thing tmp in _roomManager.currentRoom.GetThings())
                {
                    if(tmp.EventThought != null && tmp.EventThought.Id == e.EventThoughtId)
                    {
                        _socManager.AddThought(tmp.EventThought);
                        break;
                    }
                }                    
            }
        }

        #endregion

        #region UPDATE_HELPERS

        private void triggerThought(Thing thing)
        {
            if(thing.Thought != null)
            {
                _socManager.AddThought(thing.Thought);
                addClickedThing(thing);
            }
        }

        private Thing CheckCursorHoversThing()
        {

            // Get entity the mouse is hovering over
            foreach(UIInventoryPlace entity in _entityManager.GetEntitiesOfType<UIInventoryPlace>())
            {
                if(entity.BoundingBox.Contains(_cursor.Position.X, _cursor.Position.Y) && entity.Collidable)
                {
                    return entity.InventoryItem;
                }
            }

            foreach(UIArea entity in _entityManager.GetEntitiesOfType<UIArea>())
            {
                if(entity.BoundingBox.Contains(_cursor.Position.X, _cursor.Position.Y))
                {
                    return null;
                }
            }

            List<Thing> things = new List<Thing>(_entityManager.GetEntitiesOfType<Thing>());
            foreach(Thing entity in things.OrderBy(e => -e.DrawOrder))
            {
                if(entity.BoundingBox.Contains(_cursor.MouseCoordinates.X, _cursor.MouseCoordinates.Y) && GlobalData.IsNotBackgroundOrPlayer(entity))// && entity.Collidable)
                {
                    if(entity.Thought == null)
                        return null;
                    else
                        return entity;
                }
            }
            
            return null;
        }

        private bool CheckPositionsNear(Vector2 pos1, Vector2 pos2)
        {
            Vector2 diff = pos1 - pos2;
            if(Math.Abs(diff.X) < threshDiff && Math.Abs(diff.Y) < threshDiff)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsEntityNearPlayer(Entity entity)
        {
            return CheckPositionsNear(_player.CollisionBox.Center.ToVector2(), 
                                      getThingCenterTopBottomPos(entity, bottom: true))
                    || 
                    CheckPositionsNear(_player.CollisionBox.Center.ToVector2(), 
                                       getThingCenterTopBottomPos(entity, bottom: false));
        }

        private bool CheckCheckpointNear(Player player, Vector2 checkPos)
        {
            return CheckPositionsNear(player.CollisionBox.Center.ToVector2(), checkPos);
        }

        private void addClickedThing(Thing thing)
        {
            if(_lastThingsClicked.Count + 1 > _maxThingsClicked)
            {
                _lastThingsClicked.Dequeue();
            }
            _lastThingsClicked.Enqueue(thing);
        }

        private bool isTwoPartInteraction(Verb verb)
        {
            return (verb == Verb.Give || verb == Verb.Combine || verb == Verb.UseWith);
        }

        private bool isOnePartInteraction(Verb verb)
        {
            return (verb == Verb.Examine || verb == Verb.PickUp || verb == Verb.Use || verb == Verb.TalkTo);
        }

        private void addConcludingThought(string thoughtText)
        {
            if(thoughtText != "" && thoughtText != null)
                _socManager.AddThought(new ThoughtNode(0, thoughtText, 0, true, 0));
        }

        private void finishInteraction(bool canceled=false)
        {
            if(canceled)
            {
                addConcludingThought("What was I thinking?!");
            }
            _socManager.InteractionIsSuccessfull(canceled);
            _lastVerbChosen = Verb.None;
            _thingClickedInRoom = null;
            _thingClickedInInventory = null;
            _interactionActive = false;
            _isWalking = false;
        }

        private void startWalking(Thing thing, Verb verb)
        {
            _interactionActive = true;
            _isWalking = true;
            _thingClickedInRoom = thing;
            _lastVerbChosen = verb;
            _currentPathPoint = 0;

            Vector2 playerPos = _player.CollisionBox.Center.ToVector2();
            Vector2 thingPos = getThingCenterTopBottomPos(_thingClickedInRoom, bottom: true); // _thingClickedInRoom.CollisionBox.Center.ToVector2();
            _path = _pathfinder.AStarSearch(playerPos, thingPos);
        }

        #endregion

        #region INTERACTION

        public void executeThoughtInteraction(object sender, VerbActionEventArgs e)
        {
            Verb verb = e.verbAction;
            int thingId = e.ThingId;
            Thing thing = GetThingFromQueue(thingId);
            if(thing != null)
            {
                if(_interactionActive)
                {
                    finishInteraction();
                }
                if(isTwoPartInteraction(verb))
                {
                    _interactionActive = true;
                    _lastVerbChosen = verb;
                    if(thing.IsInInventory)
                    {
                        _thingClickedInInventory = thing;
                    }
                    else
                    {
                        _thingClickedInRoom = thing;
                        _inventoryManager.ShowInventory();
                    }
                }
                else if(isOnePartInteraction(verb))
                {
                    bool isNear = IsEntityNearPlayer(thing);
                    
                    if(isNear || thing.IsInInventory)
                    {
                        doInteraction(thing, verb);
                    }
                    else
                    {
                        startWalking(thing, verb);
                    }
                }
            }
        }

        private void doInteraction(Thing thing, Verb verb)
        {
            if(GlobalData.IsSameOrSubclass(typeof(Item), thing.GetType()))
            {
                ExecuteInteraction((Item)thing, verb);
            }
            else if(GlobalData.IsSameOrSubclass(typeof(Character), thing.GetType()))
            {
                ExecuteInteraction((Character)thing, verb);
            }
            else
            {
                finishInteraction();
            }
        }

        private void doInteraction(Thing thing1, Thing thing2, Verb verb)
        {
            if(GlobalData.IsSameOrSubclass(typeof(Character), thing1.GetType()))
            {
                ExecuteInteraction((Character)thing1, (Item)thing2, verb);
            }
            else if(GlobalData.IsSameOrSubclass(typeof(Character), thing2.GetType()))
            {
                ExecuteInteraction((Character)thing2, (Item)thing1, verb);
            }
            else
            {
                ExecuteInteraction((Item)thing1, (Item)thing2, verb);
            }
        }

        private Thing GetThingFromQueue(int id)
        {
            foreach(Thing thing in _lastThingsClicked)
            {
                if(thing.Id == id)
                {
                    return thing;
                }
            }
            return null;
        }
        
        private void ExecuteInteraction(Item item, Verb verb)
        {
            switch(verb)
            {
                case Verb.Examine:
                    string text = item.Examine();
                    addConcludingThought(text);
                    if(item.Thought != null)
                    {
                        _socManager.AddThought(item.Thought);
                    }
                    break;
                case Verb.PickUp:
                    if(item.PickUpAble)
                    {
                        if(!item.IsInInventory)
                        {
                            item.PickUp();
                            _roomManager.currentRoom.RemoveThing(item);
                            _inventoryManager.AddItem(item);
                        }
                        else
                        {
                            addConcludingThought("That's already in my inventory.");
                        }
                    }
                    else{
                        addConcludingThought("I can't pick that up.");
                    }
                    break;
                case Verb.Use:
                    if(item.UseAble && !item.UseWith)
                    {
                        handleUse(_roomManager.currentRoom, _player, item, null);
                    }
                    break;
                default:
                    addConcludingThought("That did not work. I feel so stupid..");
                    break;
            }
            finishInteraction();
        }

        private void ExecuteInteraction(Item item1, Item item2, Verb verb)
        {
            switch(verb)
            {
                case Verb.UseWith:
                    if((item1.UseAble && item1.UseWith) || (item2.UseAble && item2.UseWith))
                    {
                        handleUse(_roomManager.currentRoom, _player, item1, item2);
                    }
                    else
                    {
                        addConcludingThought("I can't use that.");
                    }
                    break;
                case Verb.Combine:
                    if(item1.CombineAble || item2.CombineAble)
                    {
                        Item combinedItem = item1.Combine(item2);
                        if(combinedItem != null)
                        {
                            RemoveItemFromWorld(_roomManager.currentRoom, item1);
                            RemoveItemFromWorld(_roomManager.currentRoom, item2);
                            _inventoryManager.AddItem(combinedItem);
                        }
                    }
                    else
                    {
                        addConcludingThought("I can't combine that.");
                    }
                    break;
                default:
                    addConcludingThought("I can't do that.");
                    break;
            }
            finishInteraction();
        }

        private void ExecuteInteraction(Character character, Verb verb)
        {
            switch(verb)
            {
                case Verb.TalkTo:
                    character.TalkTo();
                    break;
                default:
                    addConcludingThought(character.Name + " would not like that.");
                    break;
            }
            finishInteraction();
        }

        private void ExecuteInteraction(Character character, Item item, Verb verb)
        {
            switch(verb)
            {
                case Verb.Give:
                    if(item.GiveAble)
                    {
                        bool isSuccess = character.Give(item);
                        if(isSuccess == true)
                        {
                            RemoveItemFromWorld(_roomManager.currentRoom, item);
                        }
                    }
                    else
                    {
                        addConcludingThought(character.Name + " would not like that.");
                    }
                    break;
                default:
                    addConcludingThought(character.Name + " would not like that.");
                    break;
            }
            finishInteraction();
        }

        public void handleUse(Room room, Player player, Item item1, Item item2)
        {
            bool doRemove = false;
            if(item2 != null)
            {
                doRemove = item1.Use(room, _inventoryManager, player, item2);
            }
            else
            {
                doRemove = item1.Use(room, _inventoryManager, player);
            }
            if(doRemove == true)
            {
                RemoveItemFromWorld(room, item1);
            }
        }

        public void RemoveItemFromWorld(Room room, Item item)
        {
            if(room.GetThings().Contains(item)){
                room.RemoveThing(item);
            }
            else{
                _inventoryManager.DeleteItem(item);
            }
        }

        #endregion

    }
}