using System.Collections.Generic;

namespace conscious
{
    public class DataHolderMorphingItem : DataHolderItem
    {
        public Dictionary<MoodState, DataHolderEntity> Items { get; set; }
    }
}