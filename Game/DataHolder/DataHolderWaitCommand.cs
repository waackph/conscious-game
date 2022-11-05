namespace conscious
{
    public class DataHolderWaitCommand : DataHolderCommand
    {
        public int MillisecondsToWait { get; set; }
        public string CmdSoundFilePath { get; set; }
    }
}