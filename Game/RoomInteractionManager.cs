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
        private Thing _lastThingClicked;
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
            bool isNear = false;

            thingHovered = CheckCursorHoversThing();
            if(thingHovered != null)
            {
                isNear = IsEntityNearPlayer(thingHovered);
                _cursor.InteractionLabel = thingHovered.Name;
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
                    if(!isNear && !thingClicked.IsInInventory)
                    {
                        _lastThingClicked = thingClicked;
                        _isWalking = true;
                    }
                    else if(thingClicked.Thought != null)
                    {
                        // Add thing / Show thought
                        _socManager.AddThought(thingClicked.Thought);
                        addClickedThing(thingClicked);
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
                if(_isWalking && _lastThingClicked != null)
                {
                    if(IsEntityNearPlayer(_lastThingClicked))
                    {
                        if(_interactionActive)
                        {
                            doInteraction(_lastThingClicked, _lastVerbChosen);
                            _lastVerbChosen = Verb.None;
                            _interactionActive = false;
                        }
                        else if(_lastThingClicked.Thought != null)
                        {
                            // Add thing / Show thought as soon as player is near thing
                            _socManager.AddThought(_lastThingClicked.Thought);
                            addClickedThing(_lastThingClicked);
                        }
                        _isWalking = false;
                        _lastThingClicked = null;
                    }
                    else
                    {
                        direction = Vector2.Normalize(_lastThingClicked.Position - _player.Position);
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

        #endregion

        #region INTERACTION

        public void executeThoughtInteraction(object sender, VerbActionEventArgs e)
        {
            Verb verb = e.verbAction;
            int thingId = e.ThingId;
            Thing thing = GetThingFromQueue(thingId);
            bool isNear = IsEntityNearPlayer(thing);
            
            if(isNear || thing.IsInInventory)
            {
                _interactionActive = false;
                _isWalking = false;
                _lastThingClicked = null;
                _lastVerbChosen = Verb.None;
                doInteraction(thing, verb);
            }
            else
            {
                _interactionActive = true;
                _isWalking = true;
                _lastThingClicked = thing;
                _lastVerbChosen = verb;
            }
        }

        private void doInteraction(Thing thing, Verb verb)
        {
            // Execute action
            if(IsSameOrSubclass(typeof(Item), thing.GetType()))
            {
                Item item = (Item)thing;
                ExecuteInteraction(item, verb);
            }
            else if(IsSameOrSubclass(typeof(Character), thing.GetType()))
            {
                Character character = (Character)thing;
                ExecuteInteraction(character, verb);
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
            bool isAble = false;
            bool interactionSuccess = false;
            switch(verb)
            {
                case Verb.Examine:
                    _interactionActive = false;
                    string text = item.Examine();
                    interactionSuccess = true;
                    _dialogManager.DoDisplayText(text);
                    if(item.Thought != null)
                    {
                        _socManager.AddThought(item.Thought);
                    }
                    break;
                case Verb.PickUp:
                    _interactionActive = false;
                    isAble = item.PickUpAble;
                    if(isAble)
                    {
                        if(!item.IsInInventory)
                        {
                            item.PickUp();
                            _roomManager.currentRoom.RemoveThing(item);
                            _inventoryManager.AddItem(item);
                            interactionSuccess = true;
                        }
                        else{
                            _dialogManager.DoDisplayText("That's already in my inventory.");
                        }
                    }
                    else{
                        _dialogManager.DoDisplayText("I can't pick that up.");
                    }
                    break;
                case Verb.Use:
                    isAble = item.UseAble;
                    if(isAble)
                    {
                        interactionSuccess = true;
                        if(item.UseWith)
                        {
                            if(_lastThingClicked != null)
                            {
                                _interactionActive = false;
                                handleUse(_roomManager.currentRoom, _player, item);
                            }
                        }
                        else
                        {
                            _interactionActive = false;
                            handleUse(_roomManager.currentRoom, _player, item);
                        }
                    }
                    else
                    {
                        _interactionActive = false;
                        _dialogManager.DoDisplayText("I can't use that.");
                    }
                    break;
                case Verb.Combine:
                    isAble = item.CombineAble;
                    if(isAble)
                    {
                        if(_lastThingClicked != null)
                        {
                            _interactionActive = false;
                            Item combinedItem = item.Combine((Item)_lastThingClicked);
                            if(combinedItem != null)
                            {
                                RemoveItemFromWorld(_roomManager.currentRoom, (Item)_lastThingClicked);
                                RemoveItemFromWorld(_roomManager.currentRoom, item);
                                _inventoryManager.AddItem(combinedItem);
                            }
                        }
                    }
                    else
                    {
                        _interactionActive = false;
                        _dialogManager.DoDisplayText("I can't combine that.");
                    }
                    break;
                case Verb.Give:
                    isAble = item.GiveAble;
                    if(isAble)
                    {
                        if(_lastThingClicked != null)
                        {
                            _interactionActive = false;
                            if(IsSameOrSubclass(typeof(Character), _lastThingClicked.GetType()))
                            {
                                Character character = (Character)_lastThingClicked;
                                if(character.GiveAble)
                                {
                                    bool isSuccess = character.Give(item);
                                    if(isSuccess == true)
                                    {
                                        interactionSuccess = true;
                                        RemoveItemFromWorld(_roomManager.currentRoom, item);
                                    }
                                }
                            }
                        }
                        else
                        {
                            _interactionActive = false;
                            _dialogManager.DoDisplayText("Who would have interest in that?");
                        }
                    }
                    break;
                case Verb.WalkTo:
                    break;
                default:
                    _interactionActive = false;
                    _dialogManager.DoDisplayText("I can't do that.");
                    break;
            }
            // TODO: Decide if item mood necessary - isnt that controlled over the thought?
            // if(interactionSuccess && item.MoodChange != MoodState.None)
            // {
            //     _moodStateManager.StateChange = item.MoodChange;
            // }
        }

        private void ExecuteInteraction(Character character, Verb verb)
        {
            bool interactionSuccess = false;
            switch(verb)
            {
                case Verb.Give:
                    if(_lastThingClicked != null)
                    {
                        _interactionActive = false;
                        if(IsSameOrSubclass(typeof(Item), _lastThingClicked.GetType()))
                        {
                            Item item = (Item)_lastThingClicked;
                            if(item.GiveAble)
                            {
                                bool isSuccess = character.Give(item);
                                if(isSuccess == true)
                                {
                                    interactionSuccess = true;
                                    RemoveItemFromWorld(_roomManager.currentRoom, item);
                                }
                            }
                        }
                    }
                    break;
                case Verb.TalkTo:
                    _interactionActive = false;
                    interactionSuccess = true;
                    character.TalkTo();
                    _dialogActive = true;
                    break;
                case Verb.WalkTo:
                    break;
                default:
                    _interactionActive = false;
                    _dialogManager.DoDisplayText(character.Name + " would not like that.");
                    break;
            }
            // TODO: Decide if item mood necessary - isnt that controlled over the thought?
            // if(interactionSuccess && character.MoodChange != MoodState.None)
            // {
            //     _moodStateManager.StateChange = character.MoodChange;
            // }
        }

        public void handleUse(Room room, Player player, Item item)
        {
            bool doRemove = false;
            if(item.UseWith){
                if(IsSameOrSubclass(typeof(Item), _lastThingClicked.GetType()))
                {
                    doRemove = item.Use(room, _inventoryManager, player, (Item)_lastThingClicked);
                }
                else
                {
                    _dialogManager.DoDisplayText("I can't use that.");
                    return;
                }
            }
            else{
                doRemove = item.Use(room, _inventoryManager, player);
            }
            if(doRemove == true){
                RemoveItemFromWorld(room, item);
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