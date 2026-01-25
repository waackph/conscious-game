using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace conscious
{
    /// <summary>Class <c>MoodStateManager</c> implements the handling of a change of the mood state. 
    /// It handles events and triggers events according to the current mood state.
    /// </summary>
    ///
    public class MoodStateManager : IComponent
    {
        private UIText _moodText;
        private EntityManager _entityManager;
        private Direction _direction;
        public MoodState moodState { get; private set; }
        public MoodState StateChange { get; set; }
        public event EventHandler<MoodStateChangeEventArgs> MoodChangeEvent;

        public MoodStateManager(EntityManager entityManager, SpriteFont font, Texture2D transitionTexture, Texture2D pixel)
        {
            _direction = Direction.None;
            moodState = MoodState.Regular;
            _entityManager = entityManager;
            string text = generateMoodText();
            _moodText = new UIText(font, text, "moodText", pixel, new Vector2(20, 40), 1);
            EventBus.Subscribe<MoodTransitionFinishedEvent>(OnTransitionFinished);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (StateChange != MoodState.None && StateChange != moodState)
            {
                _direction = setChangeDirection(moodState, StateChange);
                moodState = StateChange;

                _entityManager.newMood = moodState;
                _entityManager.doTransition = true;
                // Notify MoodChangeManager about finished transition
                EventBus.Publish(this, new MoodTransitionStartedEvent()
                {
                    CurrentMoodState = moodState
                });
            }
        }

        private void OnTransitionFinished(object sender, MoodTransitionFinishedEvent e)
        {
            MoodStateChangeEventArgs moodChangeEventArgs = new MoodStateChangeEventArgs();
            moodChangeEventArgs.ChangeDirection = _direction;
            moodChangeEventArgs.CurrentMoodState = moodState;

            OnMoodChangeEvent(moodChangeEventArgs);

            _entityManager.RemoveEntity(_moodText);
            _moodText.UpdateText(generateMoodText());
            FillEntityManager();
        }

        protected virtual void OnMoodChangeEvent(MoodStateChangeEventArgs e)
        {
            MoodChangeEvent?.Invoke(this, e);
        }

        private Direction setChangeDirection(MoodState oldMood, MoodState newMood)
        {
            Direction currentDirection;
            if(oldMood == MoodState.Depressed && newMood == MoodState.Regular)
            {
                currentDirection = Direction.Up;
            }
            else if(oldMood == MoodState.Depressed && newMood == MoodState.Manic)
            {
                currentDirection = Direction.DoubleUp;
            }
            else if(oldMood == MoodState.Regular && newMood == MoodState.Depressed)
            {
                currentDirection = Direction.Down;
            }
            else if(oldMood == MoodState.Regular && newMood == MoodState.Manic)
            {
                currentDirection = Direction.Up;
            }
            else if(oldMood == MoodState.Manic && newMood == MoodState.Depressed)
            {
                currentDirection = Direction.DoubleDown;
            }
            else if(oldMood == MoodState.Manic && newMood == MoodState.Regular)
            {
                currentDirection = Direction.Down;
            }
            else
            {
                currentDirection = Direction.None;
            }
            return currentDirection;
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

        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }

        public bool IsSameOrSubclass(Type potentialBase, Type potentialDescendant)
        {
            return potentialDescendant.IsSubclassOf(potentialBase) || potentialDescendant == potentialBase;
        }

        public void FillEntityManager()
        {
            // _entityManager.AddEntity(_moodText);
        }
    }
}