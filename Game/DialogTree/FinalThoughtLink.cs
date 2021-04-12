namespace conscious
{
    public class FinalThoughtLink : ThoughtLink
    {
        private MoodState _moodChange;
        private Verb _verb;
        private AnimatedSprite _animation;

        public  int UnlockId { get; }

        public FinalThoughtLink(MoodState moodChange,
                                Verb verb,
                                AnimatedSprite animation,
                                int unlockId,
                                int id, 
                                ThoughtNode nextNode, 
                                string option, 
                                bool isLocked, 
                                MoodState[] validMoods)
                : base(id, nextNode, option, isLocked, validMoods)
        {
            _moodChange = moodChange;
            _verb = verb;
            _animation = animation;
            UnlockId = unlockId;
        }
        
    }
}