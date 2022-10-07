using System.Collections.Generic;
using Newtonsoft.Json;

namespace conscious
{
    public class ThoughtNode
    {
        private int _id;
        private int _linkageId;
        public List<ThoughtLink> Links { get; }

        public string Thought { get; set; }
        public bool IsRoot { get; }
        public bool IsUsed { get; set; }
        public bool IsInnerDialog { get; set; }
        public int ThingId { get; }

        public ThoughtNode(int id, string thought, int linkageId, bool isRoot, int thingId)
        {
            _id = id;
            Thought = thought;
            _linkageId = linkageId;
            Links = new List<ThoughtLink>();
            IsRoot = isRoot;
            IsUsed = false;
            IsInnerDialog = false;
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

        public DataHolderThoughtNode GetDataHolderThoughtNode()
        {
            DataHolderThoughtNode dataHolderThoughtNode = new DataHolderThoughtNode();
            List<DataHolderThoughtLink> dhLinks = new List<DataHolderThoughtLink>();
            if(HasLinks())
            {
                foreach(ThoughtLink link in Links)
                {
                    dhLinks.Add(link.GetDataHolderThoughtLink());
                }
            }
            dataHolderThoughtNode.Id = _id;
            dataHolderThoughtNode.Thought = Thought;
            dataHolderThoughtNode.LinkageId = _linkageId;
            dataHolderThoughtNode.IsRoot = IsRoot;
            dataHolderThoughtNode.ThingId = ThingId;
            dataHolderThoughtNode.Links = dhLinks;
            return dataHolderThoughtNode;
        }

        public DataHolderThoughtNode GetDataHolderThoughtNode(DataHolderThoughtNode dataHolderThoughtNode)
        {
            List<DataHolderThoughtLink> dhLinks = new List<DataHolderThoughtLink>();
            if(HasLinks())
            {
                foreach(ThoughtLink link in Links)
                {
                    dhLinks.Add(link.GetDataHolderThoughtLink());
                }
            }
            dataHolderThoughtNode.Id = _id;
            dataHolderThoughtNode.Thought = Thought;
            dataHolderThoughtNode.LinkageId = _linkageId;
            dataHolderThoughtNode.IsRoot = IsRoot;
            dataHolderThoughtNode.ThingId = ThingId;
            dataHolderThoughtNode.Links = dhLinks;
           return dataHolderThoughtNode;
        }
    }
}