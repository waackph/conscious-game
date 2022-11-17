using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    /// <summary>Class <c>SequenceManager</c> implements a sequence player system.
    /// Here, a sequence played is managed.
    /// </summary>
    ///
    public class SequenceManager : IComponent
    {
        private MoodStateManager _moodStateManager;
        private Thing _thing;
        private MoodState _endOfSequenceMood;
        public bool SequenceActive;
        public Sequence _currentSequence;

        public SequenceManager(MoodStateManager moodStateManager)
        {
            _moodStateManager = moodStateManager;
            SequenceActive = false;
            _currentSequence = null;
            _thing = null;
            _endOfSequenceMood = MoodState.None;
        }

        public void Update(GameTime gameTime)
        {
            if(!(_currentSequence == null))
            {
                _currentSequence.PlaySequence(gameTime, _thing);
                if(_currentSequence.SequenceFinished)
                {
                    SequenceActive = false;
                    _moodStateManager.StateChange = _endOfSequenceMood;
                    _endOfSequenceMood = MoodState.None;
                }
            }
            else
            {
                SequenceActive = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch){ }

        public void StartSequence(Sequence sequence, Thing thing, MoodState moodState)
        {
            _currentSequence = sequence;
            SequenceActive = true;
            _thing = thing;
            _endOfSequenceMood = moodState;
        }
    }
}