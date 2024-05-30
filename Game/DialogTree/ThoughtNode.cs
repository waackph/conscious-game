using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace conscious
{
    /// <summary>Class <c>ThoughNode</c> holds data of something the protagonist thinks 
    /// given a thing in the world the player draws the protagonists attention to or in response
    /// to a thought option chosen by the player in a monolog (a dialog in thought).
    /// A thought node is a node in a thought tree. It can be the start of an inner dialog
    /// (a more complex tree of thought options and answers).
    /// </summary>
    ///
    public class ThoughtNode
    {
        private int Id;
        private int _linkageId;
        public List<ThoughtLink> Links { get; }

        public string Thought { get; set; }
        public bool IsRoot { get; }
        public bool IsUsed { get; set; }
        public bool IsInnerDialog { get; set; }
        public int ThingId { get; }
        public SoundEffect EventSound { get; }
        public bool RepeatedSound { get; }

        public ThoughtNode(int id, string thought, int linkageId, bool isRoot, int thingId, 
                           SoundEffect eventSound = null, bool repeatedSound = false)
        {
            Id = id;
            Thought = thought;
            EventSound = eventSound;
            RepeatedSound = repeatedSound;
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
            dataHolderThoughtNode.Id = Id;
            dataHolderThoughtNode.Thought = Thought;
            dataHolderThoughtNode.LinkageId = _linkageId;
            dataHolderThoughtNode.IsRoot = IsRoot;
            dataHolderThoughtNode.ThingId = ThingId;
            dataHolderThoughtNode.Links = dhLinks;
            dataHolderThoughtNode.SoundPath = EventSound?.Name;
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
            dataHolderThoughtNode.Id = Id;
            dataHolderThoughtNode.Thought = Thought;
            dataHolderThoughtNode.LinkageId = _linkageId;
            dataHolderThoughtNode.IsRoot = IsRoot;
            dataHolderThoughtNode.ThingId = ThingId;
            dataHolderThoughtNode.Links = dhLinks;
            dataHolderThoughtNode.SoundPath = EventSound?.Name;
           return dataHolderThoughtNode;
        }
    }
}