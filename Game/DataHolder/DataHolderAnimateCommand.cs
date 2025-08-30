namespace conscious
{
    public class DataHolderAnimateCommand : DataHolderCommand
    {
        public float StartPositionX { get; set; }
        public float StartPositionY { get; set; }
        public string AnimState { get; set; }
        public float ScaleSize { get; set; } = 1f;
        
    }
}