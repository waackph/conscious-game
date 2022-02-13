using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace conscious
{
    public class MoodStateManager : IComponent
    {
        private UIText _moodText;
        private EntityManager _entityManager;
        public MoodState moodState { get; private set; }
        public MoodState StateChange { get; set; }
        public event EventHandler<MoodState> MoodChangeEvent;

        public MoodStateManager(EntityManager entityManager, SpriteFont font, Texture2D pixel)
        {
            moodState = MoodState.Depressed;
            _entityManager = entityManager;
            string text = generateMoodText();
            _moodText = new UIText(font, text, "moodText", pixel, new Vector2(20, 40));
        }

        public virtual void Update(GameTime gameTime)
        {
            if(StateChange != MoodState.None && StateChange != moodState)
            {
                moodState = StateChange;

                OnMoodChangeEvent(moodState);

                _entityManager.RemoveEntity(_moodText);
                _moodText.UpdateText(generateMoodText());
                FillEntityManager();
                // We dont use MorphingItem for now (maybe will not be necessary for game)
                // foreach(MorphingItem item in _entityManager.GetEntitiesOfType<MorphingItem>())
                // {
                //     item.setCurrentItem();
                // }
                // foreach(UIInventoryPlace place in _entityManager.GetEntitiesOfType<UIInventoryPlace>())
                // {
                //     if(place.InventoryItem != null)
                //     {
                //         if(IsSameOrSubclass(typeof(MorphingItem), place.InventoryItem.GetType()))
                //         {
                //             MorphingItem morph = (MorphingItem)place.InventoryItem;
                //             morph.setCurrentItem();
                //         }
                //     }
                // }
            }
        }

        protected virtual void OnMoodChangeEvent(MoodState e)
        {
            MoodChangeEvent?.Invoke(this, e);
        }

        private string generateMoodText()
        {
            string text;
            switch(moodState)
            {
                case MoodState.Depressed:
                    text = MoodState.Depressed.ToString();
                    break;
                case MoodState.Regular:
                    text = MoodState.Regular.ToString();
                    break;
                case MoodState.Manic:
                    text = MoodState.Manic.ToString();
                    break;
                default:
                    text = "Error";
                    break;
            }

            return text;
        }

        public virtual void Draw(SpriteBatch spriteBatch){ }

        public bool IsSameOrSubclass(Type potentialBase, Type potentialDescendant)
        {
            return potentialDescendant.IsSubclassOf(potentialBase) || potentialDescendant == potentialBase;
        }

        public void FillEntityManager()
        {
            _entityManager.AddEntity(_moodText);
        }
    }
}