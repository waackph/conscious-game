namespace conscious
{
    public class DataHolderDoor : DataHolderItem
    {
        public int ItemDependency { get; set; }
        public bool IsUnlocked { get; set; }
        public int RoomId { get; set; }
        public int DoorId { get; set; }
        public float InitPlayerPosX { get; set; }
        public float InitPlayerPosY { get; set; }
        public string CloseTexturePath { get; set; }
        public bool IsRoomChangeDoor { get; set; }
        public string CloseSoundFilePath { get; set; }
    }
}