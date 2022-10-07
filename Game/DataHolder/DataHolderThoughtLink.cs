namespace conscious
{
    public class DataHolderThoughtLink
    {
        public int Id { get; set; }
        public DataHolderThoughtNode NextNode { get; set; }
        public string Option { get; set; }
        public bool IsLocked { get; set; }
        public MoodState[] ValidMoods { get; set; }
    }
}