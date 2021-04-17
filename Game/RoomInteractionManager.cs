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

        private Cursor _cursor;
        private Player _player;
        private ButtonState _lastButtonState;
        private bool _interactionActive;
        private int _maxThingsClicked;
        private Queue<Thing> _lastThingsClicked;

        public RoomInteractionManager(SoCManager socManager) 
        {
            _socManager = socManager;
            _socManager.ActionEvent += executeThoughtInteraction;
            
            _lastButtonState = Mouse.GetState().LeftButton;
            _interactionActive = false;
            _maxThingsClicked = 3;
        }

        public void Update(GameTime gameTime)
        {
            Vector2 direction = Vector2.Zero;
            Vector2 mousePosition = Vector2.Zero;

            Thing thingHovered = null;
            Thing thingClicked = null;

            thingHovered = CheckCursorHoversThing();

            if(Mouse.GetState().LeftButton == ButtonState.Released && _lastButtonState == ButtonState.Pressed)
            {
                thingClicked = thingHovered;
                if(thingClicked != null)
                {
                    _socManager.AddThought(thingClicked.Thought);
                    addClickedThing(thingClicked);
                }
            }
            else if(Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if(thingHovered == null)
                    mousePosition = _cursor.MouseCoordinates;
            }

            // Set values for next iteration
            _lastButtonState = Mouse.GetState().LeftButton;
        }

        public void Draw(SpriteBatch spriteBatch)
        {

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

        private void addClickedThing(Thing thing)
        {
            if(_lastThingsClicked.Count + 1 > _maxThingsClicked)
            {
                _lastThingsClicked.Dequeue();
            }
            _lastThingsClicked.Enqueue(thing);
        }

        public void executeThoughtInteraction(object sender, VerbActionEventArgs e)
        {
            Verb verb = e.verbAction;
            int thingId = e.ThingId;
            Thing thing = GetThingFromQueue(thingId);
            bool isNear = IsEntityNearPlayer(thing);

            // Execute action
            if(IsSameOrSubclass(typeof(Item), thing.GetType()))
            {
                Item item = (Item)thing;
                ExecuteInteraction(item);
            }
            else if(IsSameOrSubclass(typeof(Character), thing.GetType()))
            {
                Character character = (Character)thing;
                ExecuteInteraction(character);
            }
        }

        private void ExecuteInteraction(Item item)
        {
            throw new NotImplementedException();
        }

        private void ExecuteInteraction(Character character)
        {
            throw new NotImplementedException();
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
    }
}