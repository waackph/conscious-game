using System.Collections.Generic;

namespace conscious
{
    public class DataHolderRoom : IDataHolder
    {
        public int RoomWidth { get; set; }
        public List<DataHolderEntity> Things { get; set; }
        public Sequence EntrySequence { get; set; }
        public string SoundFilePath { get; set; }
        public string LightMapPath { get; set; }
    }
}