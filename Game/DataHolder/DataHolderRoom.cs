using System.Collections.Generic;

namespace conscious
{
    public class DataHolderRoom : IDataHolder
    {
        public int RoomWidth { get; set; }
        public int xLimStart { get; set; }
        public int xLimEnd { get; set; }
        public int yLimStart { get; set; }
        public int yLimEnd { get; set; }
        public List<DataHolderEntity> Things { get; set; }
        public DataHolderSequence EntrySequence { get; set; }
        public string SoundFilePath { get; set; }
        public string LightMapPath { get; set; }
        public string AtmoSoundFilePath { get; set; }
        public string WalkingSoundFilePath { get; set; }
        public DataHolderThoughtNode Thought { get; set; }
    }
}