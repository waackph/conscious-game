using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace conscious
{
    public class MoodStateManager : IComponent
    {
        public MoodState moodState { get; private set; }
        public MoodState StateChange { get; set; }

        private EntityManager _entityManager;

        public MoodStateManager(EntityManager entityManager)
        {
            moodState = MoodState.Depressed;
            _entityManager = entityManager;
        }

        public virtual void Update(GameTime gameTime)
        {
            if(StateChange != moodState)
            {
                moodState = StateChange;
                foreach(MorphingItem item in _entityManager.GetEntitiesOfType<MorphingItem>())
                {
                    item.setCurrentItem();
                }
                foreach(UIInventoryPlace place in _entityManager.GetEntitiesOfType<UIInventoryPlace>())
                {
                    if(place.InventoryItem != null)
                    {
                        if(IsSameOrSubclass(typeof(MorphingItem), place.InventoryItem.GetType()))
                        {
                            MorphingItem morph = (MorphingItem)place.InventoryItem;
                            morph.setCurrentItem();
                        }
                    }
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch){ }

        public bool IsSameOrSubclass(Type potentialBase, Type potentialDescendant)
        {
            return potentialDescendant.IsSubclassOf(potentialBase) || potentialDescendant == potentialBase;
        }
    }
}