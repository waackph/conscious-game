using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;
using System;

namespace conscious
{
    public class RoomInteractionManager : IComponent
    {
        private EntityManager _entityManager;
        private SoCManager _socManager;
        private InventoryManager _inventoryManager;
        private ControlsManager _controlsManager;
        private RoomManager _roomManager;
        private UiDialogManager _dialogManager;

        private Cursor _cursor;
        private Player _player;

        private ButtonState _lastButtonState;
        private Thing _thingClickedInRoom;
        private Thing _thingClickedInInventory;
        private Verb _lastVerbChosen;
        private int _maxThingsClicked;
        private Queue<Thing> _lastThingsClicked;
        private bool _isWalking;
        private bool _interactionActive;
        private bool _dialogActive;

        public RoomInteractionManager(EntityManager entityManager, 
                                      SoCManager socManager, 
                                      InventoryManager inventoryManager, 
                                      ControlsManager controlsManager,
                                      RoomManager roomManager,
                                      UiDialogManager dialogManager,
                                      Cursor cursor,
                                      Player player) 
        {
            _entityManager = entityManager;
            _socManager = socManager;
            _socManager.ActionEvent += executeThoughtInteraction;
            _inventoryManager = inventoryManager;
            _controlsManager = controlsManager;
            _roomManager = roomManager;
            _dialogManager = dialogManager;

            _cursor = cursor;
            _player = player;

            _lastThingsClicked = new Queue<Thing>();
            
            _lastButtonState = Mouse.GetState().LeftButton;
            _interactionActive = false;
            _isWalking = false;
            _dialogActive = true;
            _maxThingsClicked = 3;
            _lastVerbChosen = Verb.None;
        }

        #region GAMELOOP

        public void Update(GameTime gameTime)
        {
            Vector2 direction = Vector2.Zero;
            Vector2 mousePosition = Vector2.Zero;

            Thing thingHovered = null;
            Thing thingClicked = null;

            if(Keyboard.GetState().IsKeyDown(Keys.C))
            {
                finishInteraction();
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
                            finishInteraction();
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
                if(thingHovered == null)
                    mousePosition = _cursor.MouseCoordinates;
            }
            else
            {
                if(_isWalking && _interactionActive && _thingClickedInRoom != null)
                {
                    if(IsEntityNearPlayer(_thingClickedInRoom))
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
                        direction = Vector2.Normalize(_thingClickedInRoom.Position - _player.Position);
                    }
                }
            }

            // Set controls manager variables to move in direction when clicked
            _controlsManager.MousePosition = mousePosition;
            _controlsManager.Direction = direction;

            // Set values for next iteration
            _lastButtonState = Mouse.GetState().LeftButton;
        }

        public void Draw(SpriteBatch spriteBatch) {}

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
            bool entityHit = false;

            // Get entity the mouse is hovering over
            foreach(UIInventoryPlace entity in _entityManager.GetEntitiesOfType<UIInventoryPlace>())
            {
                if(entity.BoundingBox.Contains(_cursor.Position.X, _cursor.Position.Y) && entity.Collidable)
                {
                    entityHit = true;
                    return entity.InventoryItem;
                }
            }

            if(!entityHit)
            {
                foreach(Thing entity in _entityManager.GetEntitiesOfType<Thing>())
                {
                    if(entity.BoundingBox.Contains(_cursor.MouseCoordinates.X, _cursor.MouseCoordinates.Y) && entity.Collidable)
                    {
                        entityHit = true;
                        return entity;
                    }
                }
            }
            
            return null;
        }

        private bool IsEntityNearPlayer(Entity entity)
        {
            bool isNear;
            float distance = _player.GetDistance(entity);
            if(distance <= Math.Max(entity.Height, entity.Width)*0.66 + _player.Width*0.66)
                isNear = true;
            else
                isNear = false;
            return isNear;
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

        private void finishInteraction()
        {
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
            if(IsSameOrSubclass(typeof(Item), thing.GetType()))
            {
                ExecuteInteraction((Item)thing, verb);
            }
            else if(IsSameOrSubclass(typeof(Character), thing.GetType()))
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
            if(IsSameOrSubclass(typeof(Character), thing1.GetType()))
            {
                ExecuteInteraction((Character)thing1, (Item)thing2, verb);
            }
            else if(IsSameOrSubclass(typeof(Character), thing2.GetType()))
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

        private bool IsSameOrSubclass(Type potentialBase, Type potentialDescendant)
        {
            return potentialDescendant.IsSubclassOf(potentialBase) || potentialDescendant == potentialBase;
        }
        
        // TODO: maybe decouple item/character interaction to other class - or move maybe move calls to dialogManager
        // TODO: Rethink code because examine does not exist anymore, is handled in thoughts, here only use/combine and stuff
        private void ExecuteInteraction(Item item, Verb verb)
        {
            // TODO: handle dialog texts (separate into single line of player and dialog between character and player)
            // TODO: separate interaction code into tasks (especially what happens in the world and what in the game/progress logic and what in the item)
            switch(verb)
            {
                case Verb.Examine:
                    string text = item.Examine();
                    _dialogManager.DoDisplayText(text);
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
                            _dialogManager.DoDisplayText("That's already in my inventory.");
                        }
                    }
                    else{
                        _dialogManager.DoDisplayText("I can't pick that up.");
                    }
                    break;
                case Verb.Use:
                    if(item.UseAble && !item.UseWith)
                    {
                        handleUse(_roomManager.currentRoom, _player, item, null);
                    }
                    break;
                default:
                    _dialogManager.DoDisplayText("I can't do that.");
                    break;
            }
            finishInteraction();
            // TODO: Decide if item mood necessary - isnt that controlled over the thought?
            // if(interactionSuccess && item.MoodChange != MoodState.None)
            // {
            //     _moodStateManager.StateChange = item.MoodChange;
            // }
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
                        _dialogManager.DoDisplayText("I can't use that.");
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
                        _dialogManager.DoDisplayText("I can't combine that.");
                    }
                    break;
                default:
                    _dialogManager.DoDisplayText("I can't do that.");
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
                    _dialogActive = true;
                    break;
                default:
                    _dialogManager.DoDisplayText(character.Name + " would not like that.");
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
                        _dialogManager.DoDisplayText(character.Name + " would not like that.");
                    }
                    break;
                default:
                    _dialogManager.DoDisplayText(character.Name + " would not like that.");
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