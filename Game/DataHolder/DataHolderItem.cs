namespace conscious
{
    public class DataHolderItem : DataHolderEntity
    {
        public int Id { get; set; }
        public bool PickUpAble { get; set; }
        public bool UseAble { get; set; }
        public bool CombineAble { get; set; }
        public bool GiveAble { get; set; }
        public bool UseWith { get; set; }
        public string ExamineText { get; set; }
    }
}