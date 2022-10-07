using System.Collections.Generic;

namespace conscious
{
    public class DataHolderThoughtNode
    {
        public int Id { get; set; }
        public string Thought { get; set; }
        public int LinkageId { get; set; }
        public bool IsRoot { get; set; }
        public int ThingId { get; set; }
        public List<DataHolderThoughtLink> Links { get; set; }
    }
}