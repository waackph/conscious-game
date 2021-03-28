using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace conscious
{
    public class InteractionManager
    {
        private Cursor _cursor;
        private Player _player;
        private ControlsManager _controlsManager;
        private EntityManager _entityManager;
        private InventoryManager _inventoryManager;
        private RoomManager _roomManager;
        private DialogManager _dialogManager;
        private MoodStateManager _moodStateManager;
        private Entity _lastEntityClicked;
        private Verb _verbClicked;
        private ButtonState _lastButtonState;
        private bool _interactionActive;
        private bool _itemInInventory;
        private bool _dialogActive;

        public InteractionManager(Player player, Cursor cursor, 
                                  ControlsManager controlsManager, EntityManager entityManager, 
                                  InventoryManager inventoryManager, RoomManager roomManager,
                                  DialogManager dialogManager, MoodStateManager moodStateManager){
            _interactionActive = false;
            _itemInInventory = false;
            _dialogActive = false;

            _lastEntityClicked = null;
            _verbClicked = Verb.None;
            _lastButtonState = Mouse.GetState().LeftButton;

            _player = player;
            _cursor = cursor;
            _controlsManager = controlsManager;
            _entityManager = entityManager;
            _inventoryManager = inventoryManager;
            _roomManager = roomManager;
            _dialogManager = dialogManager;
            _moodStateManager = moodStateManager;
        }

        public void Update(GameTime gameTime)
        {
            Vector2 direction = Vector2.Zero;
            Vector2 mousePosition = Vector2.Zero;
            Entity entityHovered = null;
            Entity currentEntityClicked = null;
            bool entityHit = false;
            _dialogActive = false;

            // Get entity the mouse is hovering over
            foreach(Entity entity in _entityManager.GetEntitiesOfType<UIComponent>())
            {
                if(entity.BoundingBox.Contains(_cursor.Position.X, _cursor.Position.Y) && entity.Collidable)
                {
                    entityHit = true;
                    entityHovered = entity;
                    break;
                }
            }

            if(!entityHit)
            {
                foreach(Entity entity in _entityManager.GetEntitiesOfType<Thing>())
                {
                    if(entity.BoundingBox.Contains(_cursor.MouseCoordinates.X, _cursor.MouseCoordinates.Y) && entity.Collidable)
                    {
                        entityHit = true;
                        entityHovered = entity;
                        break;
                    }
                }
            }

            // assign hovered entity if mouse button was just released (ie has been clicked) 
            // OR get mouse position if there is no entity while pressed for player to walk
            if(Mouse.GetState().LeftButton == ButtonState.Released && _lastButtonState == ButtonState.Pressed)
            {
                currentEntityClicked = entityHovered;
                if(currentEntityClicked != null)
                    _interactionActive = true;
            }
            else if(Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if(entityHovered == null)
                    mousePosition = _cursor.MouseCoordinates;
            }
            
            // Check what type current clicked entity is and call appropriate interaction function
            if(entityHovered != null)
            {
                if(IsSameOrSubclass(typeof(UIVerb), entityHovered.GetType()))
                {
                    if(currentEntityClicked != null)
                    {
                        UIVerb verb = (UIVerb)currentEntityClicked;
                        _verbClicked = verb.VerbText;
                    }   
                }
                else if(IsSameOrSubclass(typeof(UIInventoryPlace), entityHovered.GetType()))
                {
                    _itemInInventory = true;
                    UIInventoryPlace place = (UIInventoryPlace)entityHovered;
                    Item item = place.InventoryItem;
                    entityHovered = item;

                    if(item != null)
                    {
                        DeriveVerb(item, isNear:true);

                        if(currentEntityClicked != null)
                        {
                            DoInteraction(item);
                            currentEntityClicked = item;
                        }
                    }
                }
                else if(IsSameOrSubclass(typeof(Item), entityHovered.GetType()))
                {
                    _itemInInventory = false;
                    Item item = (Item)entityHovered;

                    bool isNear = IsEntityNearPlayer(item);
                    DeriveVerb(item, isNear);
                    if(currentEntityClicked != null)
                        DoInteraction(item);
                }
                else if(IsSameOrSubclass(typeof(Character), entityHovered.GetType()))
                {
                    Character character = (Character)entityHovered;
                    bool isNear = IsEntityNearPlayer(character);
                    DeriveVerb(character, isNear);
                    if(currentEntityClicked != null)
                        DoInteraction(character);
                }
            }

            // In case of active walkTo Interaction
            if(_verbClicked == Verb.WalkTo && IsValidThing(_lastEntityClicked) && _interactionActive)
            {
                if(!IsEntityNearPlayer(_lastEntityClicked))
                {
                    direction = Vector2.Normalize(_lastEntityClicked.Position - _player.Position);
                }
                else
                {
                    _interactionActive = false;
                }
            }

            AssignCursorHoverText(_verbClicked, entityHovered, _lastEntityClicked);

            // Final code of update iteration here:
            _lastButtonState = Mouse.GetState().LeftButton;
            _controlsManager.MousePosition = mousePosition;
            _controlsManager.Direction = direction;
            if(IsValidThing(currentEntityClicked) && _interactionActive)
            {
                _lastEntityClicked = currentEntityClicked;
            }
            else if(!_interactionActive)
            {
                _lastEntityClicked = null;
                _verbClicked = Verb.None;
            }
        }

        public bool IsSameOrSubclass(Type potentialBase, Type potentialDescendant)
        {
            return potentialDescendant.IsSubclassOf(potentialBase) || potentialDescendant == potentialBase;
        }

        public bool IsValidThing(Entity entity)
        {
            return (entity != null && IsSameOrSubclass(typeof(Thing), entity.GetType()));
        }

        public void DeriveVerb(Item item, bool isNear)
        {
            if(_verbClicked == Verb.None)
            {
                if(isNear)
                    _verbClicked = Verb.Examine;
                else
                    _verbClicked = Verb.WalkTo;
            }
            else if(!isNear)
                _verbClicked = Verb.WalkTo;
        }

        public void DeriveVerb(Character character, bool isNear)
        {
            if(_verbClicked == Verb.None){
                if(isNear)
                    _verbClicked = Verb.TalkTo;
                else
                    _verbClicked = Verb.WalkTo;
            }
        }

        public bool IsEntityNearPlayer(Entity entity)
        {
            bool isNear;
            float distance = _player.GetDistance(entity);
            if(distance <= Math.Max(entity.Height, entity.Width)*0.66 + _player.Width*0.66)
                isNear = true;
            else
                isNear = false;
            return isNear;
        }

        public void DoInteraction(Item item)
        {
            // TODO: handle dialog texts (separate into single line of player and dialog between character and player)
            // TODO: separate interaction code into tasks (especially what happens in the world and what in the game/progress logic and what in the item)
            bool isAble = false;
            bool interactionSuccess = false;
            switch(_verbClicked)
            {
                case Verb.Examine:
                    _interactionActive = false;
                    string text = item.Examine();
                    interactionSuccess = true;
                    _dialogManager.DoDisplayText(text);
                    break;
                case Verb.PickUp:
                    _interactionActive = false;
                    isAble = item.PickUpAble;
                    if(isAble)
                    {
                        if(!_itemInInventory)
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
                            if(_lastEntityClicked != null)
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
                        if(_lastEntityClicked != null)
                        {
                            _interactionActive = false;
                            Item combinedItem = item.Combine((Item)_lastEntityClicked);
                            if(combinedItem != null)
                            {
                                RemoveItemFromWorld(_roomManager.currentRoom, (Item)_lastEntityClicked);
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
                        if(_lastEntityClicked != null)
                        {
                            _interactionActive = false;
                            if(IsSameOrSubclass(typeof(Character), _lastEntityClicked.GetType()))
                            {
                                Character character = (Character)_lastEntityClicked;
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
            if(interactionSuccess && item.MoodChange != MoodState.None)
            {
                _moodStateManager.StateChange = item.MoodChange;
            }
        }

        public void DoInteraction(Character character)
        {
            bool interactionSuccess = false;
            switch(_verbClicked)
            {
                case Verb.Give:
                    if(_lastEntityClicked != null)
                    {
                        _interactionActive = false;
                        if(IsSameOrSubclass(typeof(Item), _lastEntityClicked.GetType()))
                        {
                            Item item = (Item)_lastEntityClicked;
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
            if(interactionSuccess && character.MoodChange != MoodState.None)
            {
                _moodStateManager.StateChange = character.MoodChange;
            }
        }

        public void AssignCursorHoverText(Verb verb, Entity currentEntity, Entity lastEntity)
        {
            // ... assign Text, that should be displayed as Mouse label text
            string verbText = "";
            string firstEntity = "";
            string secondEntity = "";
            string interactionText = "";
            if(verb == Verb.None && currentEntity == null && lastEntity == null || _dialogActive)
            {
                _cursor.InteractionLabel = "";
                return;
            }
            if(verb != Verb.None)
            {
                verbText = GetVerbString(verb);
            }
            if(IsValidThing(lastEntity))
            {
                firstEntity = lastEntity.Name;
                if(IsValidThing(currentEntity))
                {
                    secondEntity = currentEntity.Name;
                }
            }
            else if(IsValidThing(currentEntity))
            {
                firstEntity = currentEntity.Name;
            }
            
            interactionText += verbText;
            if(firstEntity != "")
            {
                if(verb == Verb.Give)
                {
                    interactionText += " " + firstEntity + " to " + secondEntity;
                }
                else if(verb == Verb.Combine)
                {
                    interactionText += " " + firstEntity + " with " + secondEntity;
                }
                else if(verb == Verb.Use && _lastEntityClicked != null && _interactionActive)
                {
                    interactionText += " " + firstEntity + " with " + secondEntity;
                }
                else
                {
                    interactionText += " " + firstEntity;
                }
            }
            _cursor.InteractionLabel = interactionText;
        }

        public string GetVerbString(Verb verb)
        {
            string verbString = "";
            switch(verb)
            {
                case Verb.Examine:
                    verbString = "Examine";
                    break;
                case Verb.Combine:
                    verbString = "Combine";
                    break;
                case Verb.Give:
                    verbString = "Give";
                    break;
                case Verb.PickUp:
                    verbString = "Pick up";
                    break;
                case Verb.TalkTo:
                    verbString = "Talk to";
                    break;
                case Verb.Use:
                    verbString = "Use";
                    break;
                case Verb.WalkTo:
                    verbString = "Walk to";
                    break;
            }
                return verbString;
        }

        public void handleUse(Room room, Player player, Item item)
        {
            bool doRemove = false;
            if(item.UseWith){
                if(IsSameOrSubclass(typeof(Item), _lastEntityClicked.GetType()))
                {
                    doRemove = item.Use(room, _inventoryManager, player, (Item)_lastEntityClicked);
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
    }
}