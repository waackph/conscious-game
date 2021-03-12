using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    public class MoodStateManager : IComponent
    {
        public MoodState moodState { get; set; }
        public MoodState StateChange { get; set; }

        public MoodStateManager()
        {
            moodState = MoodState.depressed;
        }

        public virtual void Update(GameTime gameTime)
        {
            if(StateChange != moodState)
            {
                moodState = StateChange;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch){ }

    }
}