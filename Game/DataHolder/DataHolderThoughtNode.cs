using System.Collections.Generic;

namespace conscious
{
    public class DataHolderThoughtNode
    {
        public int Id { get; set; }
        public string Thought { get; set; }
        public int LinkageId { get; set; }
        public bool IsRoot { get; set; }
        public int ThingId { get; set; } // If there is a verb action associated, this is the thingId to which it links
        public List<DataHolderThoughtLink> Links { get; set; }
    }
}