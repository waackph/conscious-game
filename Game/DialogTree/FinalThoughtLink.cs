namespace conscious
{
    public class FinalThoughtLink : ThoughtLink
    {
        public MoodState MoodChange { get; }
        public Verb Verb { get; }
        public AnimatedSprite Animation { get; }
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
            MoodChange = moodChange;
            Verb = verb;
            Animation = animation;
            UnlockId = unlockId;
        }
        
    }
}