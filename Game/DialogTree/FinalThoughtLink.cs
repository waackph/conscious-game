namespace conscious
{
    /// <summary>Class <c>FinalThoughtLink</c> holds data of an option in a monolog (a dialog in thought)
    /// that is a final option that finishes a thought of the protagonist.
    /// A final thought link option holds also a possible change in the mood state, 
    /// a verb that is executed on the given Thing in the world, 
    /// an animation that is executed of the thing or a sequence that is played.
    /// A final thought link might be the success edge in a tree that finishes interaction with a Thing.
    /// </summary>
    ///
    public class FinalThoughtLink : ThoughtLink
    {
        public MoodState MoodChange { get; }
        public Verb Verb { get; }
        public AnimatedSprite Animation { get; }
        public Sequence ThoughtSequence { get; }
        public int UnlockId { get; }
        public bool IsSuccessEdge { get; }
        public ThoughtNode EventThought { get; }

        public FinalThoughtLink(MoodState moodChange,
                                Verb verb,
                                AnimatedSprite animation,
                                Sequence sequence,
                                int unlockId,
                                int id, 
                                ThoughtNode nextNode, 
                                string option, 
                                bool isLocked, 
                                MoodState[] validMoods,
                                bool isSuccessEdge, ThoughtNode eventThought = null)
                : base(id, nextNode, option, isLocked, validMoods)
        {
            MoodChange = moodChange;
            Verb = verb;
            Animation = animation;
            UnlockId = unlockId;
            IsSuccessEdge = isSuccessEdge;
            ThoughtSequence = sequence;
            EventThought = eventThought;
        }

        public override DataHolderThoughtLink GetDataHolderThoughtLink()
        {
            DataHolderFinalThoughtLink dataHolderThoughtLink = new DataHolderFinalThoughtLink();
            dataHolderThoughtLink = (DataHolderFinalThoughtLink)base.GetDataHolderThoughtLink(dataHolderThoughtLink);
            dataHolderThoughtLink.moodChange = MoodChange;
            dataHolderThoughtLink.verb = Verb;
            dataHolderThoughtLink.Animation = Animation?.GetDataHolderAnimatedSprite();
            dataHolderThoughtLink.sequence = ThoughtSequence?.GetDataHolderSequence();
            dataHolderThoughtLink.UnlockId = UnlockId;
            dataHolderThoughtLink.IsSuccessEdge = IsSuccessEdge;
            dataHolderThoughtLink.EventThought = EventThought;
            return dataHolderThoughtLink;
        }
        
        public DataHolderThoughtLink GetDataHolderThoughtLink(DataHolderFinalThoughtLink dataHolderThoughtLink)
        {
            dataHolderThoughtLink = (DataHolderFinalThoughtLink)base.GetDataHolderThoughtLink(dataHolderThoughtLink);
            dataHolderThoughtLink.moodChange = MoodChange;
            dataHolderThoughtLink.verb = Verb;
            dataHolderThoughtLink.Animation = Animation?.GetDataHolderAnimatedSprite();
            dataHolderThoughtLink.sequence = ThoughtSequence?.GetDataHolderSequence();
            dataHolderThoughtLink.UnlockId = UnlockId;
            dataHolderThoughtLink.IsSuccessEdge = IsSuccessEdge;
            dataHolderThoughtLink.EventThought = EventThought;
            return dataHolderThoughtLink;
        }
    }
}