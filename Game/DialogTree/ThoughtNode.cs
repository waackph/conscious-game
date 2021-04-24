using System.Collections.Generic;

namespace conscious
{
    public class ThoughtNode
    {
        private int _id;
        private int _linkageId;
        public List<ThoughtLink> Links { get; }

        public string Thought { get; }
        public bool IsRoot { get; }
        public int ThingId { get; }

        public ThoughtNode(int id, string thought, int linkageId, bool isRoot, int thingId)
        {
            _id = id;
            Thought = thought;
            _linkageId = linkageId;
            Links = new List<ThoughtLink>();
            IsRoot = isRoot;
            ThingId = thingId;
        }

        public void AddLink(ThoughtLink link)
        {
            Links.Add(link);
        }

        public void RemoveLink(ThoughtLink link)
        {
            Links.Remove(link);
        }

        public bool HasLinks()
        {
            if(Links.Count > 0)
            {
                return true;
            }
            return false;
        }
    }
}