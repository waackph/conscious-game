namespace conscious
{
    public class DataHolderFinalThoughtLink : DataHolderThoughtLink
    {
        public MoodState moodChange { get; set; }
        public Verb verb { get; set; }
        public DataHolderAnimatedSprite Animation { get; set; }
        public DataHolderSequence sequence { get; set; }
        public int UnlockId { get; set; }
        public bool IsSuccessEdge { get; set; }
        public int EventThoughtId { get; set; }
    }
}