using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace conscious
{
    public class SoCManager : IComponent
    {
        private UiDisplayThoughtManager _uiDisplayThought;
        private Queue<ThoughtNode> _thoughts;
        private int _maxThoughts;
        private List<ThoughtLink> _currentSubthoughtLinks;
        private ThoughtNode _currentSubthought;
        private ThoughtNode _currentThought;

        public SoCManager(UiDisplayThoughtManager uiDisplayThought)
        {
            _uiDisplayThought = uiDisplayThought;            
            _thoughts = new Queue<ThoughtNode>();
            _maxThoughts = 2;
        }

        public void Update(GameTime gameTime){ }

        public void Draw(SpriteBatch spriteBatch){ }

        public void AddThought(ThoughtNode thought)
        {
            if(_thoughts.Count + 1 > _maxThoughts)
            {
                RemoveThought();
            }
            _thoughts.Enqueue(thought);
            _uiDisplayThought.AddThought(thought);
        }

        public void RemoveThought()
        {
            _thoughts.Dequeue();
        }

        public void SelectThought(string thoughtName)
        {
            ThoughtNode node = GetThought(thoughtName);
            // If thought is an Selectable Thought: choose link from root
            if(node.HasLinks())
            {
                if(node.IsRoot)
                {
                    _currentThought = node;
                    node.Links.Sort((x, y) => x.Id.CompareTo(y.Id));
                    foreach(ThoughtLink link in node.Links)
                    {
                        if(!link.IsLocked)
                        {
                            ThoughtNode displayNode = link.NextNode;
                            _uiDisplayThought.StartThoughtMode(displayNode, displayNode.Links);
                            break;
                        }
                    }
                }
            }
        }

        public void SelectSubthought(string thoughtName)
        {
            ThoughtLink option = GetOption(thoughtName);
            // TODO: check here, if current mood state is valid to even select that option  
            // or if the option is currently locked
            // ...
            ThoughtNode node = option.NextNode;
            if(node == null || !node.HasLinks())
            {
                _uiDisplayThought.EndThoughtMode();
                // If there is a last node (without links), it should be displayed as a concluding thought in the SoC Main Window
                if(!node.HasLinks())
                {
                    AddThought(node);
                }
                // If the link is a final option, execute possible operations
                if(typeof(FinalThoughtLink) == option.GetType())
                {
                    FinalThoughtLink finalOption = (FinalThoughtLink)option;
                    if(finalOption.UnlockId != 0)
                        unlockThoughtLink(finalOption.UnlockId);
                    if(finalOption.Verb != Verb.None)
                        executeVerb(finalOption.Verb);
                    if(finalOption.Animation != null)
                        executeAnimation(finalOption.Animation);
                }
            }
            else
            {
                _uiDisplayThought.ChangeSubthought(node, node.Links);
                _currentSubthoughtLinks = node.Links;
            }
        }

        private void unlockThoughtLink(int unlockId)
        {
            if(_currentThought != null)
            {
                // TODO: May be needed in the future - Traverse Tree from root, to unlock any edge in the tree
                // FindLinkById(unlockId)
                foreach(ThoughtLink link in _currentThought.Links)
                {
                    if(link.Id == unlockId)
                    {
                        link.IsLocked = false;
                    }
                }
            }
        }

        private Node FindLinkById(int id)
        {
            throw new NotImplementedException();
        }

        private void executeVerb(Verb verb)
        {
            throw new NotImplementedException();
        }

        private void executeAnimation(AnimatedSprite sprite)
        {
            throw new NotImplementedException();
        }

        public ThoughtNode GetThought(string thoughtText)
        {
            foreach(ThoughtNode thought in _thoughts)
            {
                if(thought.Thought == thoughtText)
                {
                    return thought;
                }
            }
            return null;
        }

        public ThoughtLink GetOption(string thoughtText)
        {
            if(_currentSubthoughtLinks != null)
            {
                foreach(ThoughtLink thought in _currentSubthoughtLinks)
                {
                    if(thought.Option == thoughtText)
                    {
                        return thought;
                    }
                }
            }
            return null;
        }
    }
}
