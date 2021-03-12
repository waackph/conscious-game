using System.Collections.Generic;

namespace conscious
{
    public class DataHolderRoom : IDataHolder
    {
        public int RoomWidth { get; set; }
        public List<DataHolderEntity> Things { get; set; }
    }
}