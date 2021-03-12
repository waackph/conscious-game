using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// using System.Collections.Generic;

using conscious.Sequences;


namespace conscious
{
    public class SequenceManager : IComponent
    {
        // private List<Sequence> _sequences;
        public bool SequenceActive;
        public Sequence _currentSequence;

        public SequenceManager()
        {
            SequenceActive = false;
            _currentSequence = null;
            // _sequences = new List<Sequence>();
        }

        public void Update(GameTime gameTime)
        {
            if(!(_currentSequence == null))
            {
                _currentSequence.PlaySequence(gameTime);
                if(_currentSequence.SequenceFinished)
                    SequenceActive = false;
            }
            else
            {
                SequenceActive = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch){ }

        public void StartSequence(Sequence sequence)
        {
            _currentSequence = sequence;
            SequenceActive = true;
        }
    }
}