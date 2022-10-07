namespace conscious
{
    public class FinalThoughtLink : ThoughtLink
    {
        public MoodState MoodChange { get; }
        public Verb Verb { get; }
        public AnimatedSprite Animation { get; }
        public Sequence ThoughtSequence { get; }
        public int UnlockId { get; }
        public bool IsSuccessEdge { get; }

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
                                bool isSuccessEdge)
                : base(id, nextNode, option, isLocked, validMoods)
        {
            MoodChange = moodChange;
            Verb = verb;
            Animation = animation;
            UnlockId = unlockId;
            IsSuccessEdge = isSuccessEdge;
            ThoughtSequence = sequence;
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
            return dataHolderThoughtLink;
        }
    }
}