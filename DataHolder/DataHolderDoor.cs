namespace conscious
{
    public class DataHolderDoor : DataHolderItem
    {
        public int ItemDependency { get; set; }
        public bool IsUnlocked { get; set; }
        public int RoomId { get; set; }
    }
}