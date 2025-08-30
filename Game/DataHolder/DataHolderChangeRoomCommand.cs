namespace conscious
{
    public class DataHolderChangeRoomCommand : DataHolderCommand
    {
        public float StartPositionX { get; set; }
        public float StartPositionY { get; set; }
        public string AnimState { get; set; }
        public int NextRoomId { get; set; }
        
    }
}