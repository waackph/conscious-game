using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    public class SequenceManager : IComponent
    {
        private Thing _thing;
        public bool SequenceActive;
        public Sequence _currentSequence;

        public SequenceManager()
        {
            SequenceActive = false;
            _currentSequence = null;
            _thing = null;
        }

        public void Update(GameTime gameTime)
        {
            if(!(_currentSequence == null))
            {
                _currentSequence.PlaySequence(gameTime, _thing);
                if(_currentSequence.SequenceFinished)
                    SequenceActive = false;
            }
            else
            {
                SequenceActive = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch){ }

        public void StartSequence(Sequence sequence, Thing thing)
        {
            _currentSequence = sequence;
            SequenceActive = true;
            _thing = thing;
        }
    }
}