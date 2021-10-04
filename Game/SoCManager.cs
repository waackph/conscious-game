using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace conscious
{
    public class SoCManager : IComponent
    {
        private MoodStateManager _moodStateManager;
        public Queue<ThoughtNode> Thoughts { get; private set; }
        private int _maxThoughts;
        private List<ThoughtLink> _currentSubthoughtLinks;
        private ThoughtNode _currentThought;
        
        public event EventHandler<VerbActionEventArgs> ActionEvent;
        public event EventHandler<ThoughtNode> AddThoughtEvent;
        public Verb VerbResult { get; private set; }

        public SoCManager(MoodStateManager moodStateManager)
        {
            _moodStateManager = moodStateManager;
            Thoughts = new Queue<ThoughtNode>();
            _maxThoughts = 2;
            VerbResult = Verb.None;
        }

        public void Update(GameTime gameTime){ }

        public void Draw(SpriteBatch spriteBatch){ }

        public void AddThought(ThoughtNode thought)
        {
            if(!containsThoughtNode(Thoughts, thought))
            {
                if(Thoughts.Count + 1 > _maxThoughts)
                {
                    Thoughts.Dequeue();
                }
                Thoughts.Enqueue(thought);
                // Invoke event for UiDisplayThoughtManager to add the thought UI Element as well
                OnAddThoughtEvent(thought);
            }
        }

        private bool containsThoughtNode(Queue<ThoughtNode> thoughts, ThoughtNode node)
        {
            foreach(ThoughtNode thought in thoughts)
            {
                if(thought.Thought == node.Thought)
                {
                    return true;
                }
            }
            return false;
        }

        public ThoughtNode SelectThought(string thoughtName)
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
                            _currentSubthoughtLinks = displayNode.Links;
                            // _uiDisplayThought.StartThoughtMode(displayNode, displayNode.Links);
                            return displayNode;
                        }
                    }
                }
            }
            return null;
        }

        public bool IsSuccessEdgeChosen(string thoughtName)
        {
            ThoughtLink option = GetOption(thoughtName);
            if(option != null && !option.IsLocked && option.MoodValid(_moodStateManager.moodState))
            {
                if(typeof(FinalThoughtLink) == option.GetType())
                {
                    FinalThoughtLink finalOption = (FinalThoughtLink)option;
                    return finalOption.IsSuccessEdge;
                }
            }
            return false;
        }

        public ThoughtNode SelectSubthought(string thoughtName)
        {
            ThoughtLink option = GetOption(thoughtName);
            if(option != null && !option.IsLocked && option.MoodValid(_moodStateManager.moodState))
            {
                ThoughtNode node = option.NextNode;
                if(node == null || !node.HasLinks())
                {
                    // If there is a last node (without links), it should be displayed as a concluding thought in the SoC Main Window
                    if(node != null && !node.HasLinks())
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
                        {
                            VerbActionEventArgs data = new VerbActionEventArgs();
                            data.ThingId = _currentThought.ThingId;
                            data.verbAction = finalOption.Verb;
                            OnActionEvent(data);
                        }
                        if(finalOption.MoodChange != MoodState.None)
                        {
                            _moodStateManager.StateChange = finalOption.MoodChange;
                        }
                    }
                    // _uiDisplayThought.EndThoughtMode();
                    return null;
                }
                else
                {
                    _currentSubthoughtLinks = node.Links;
                    // _uiDisplayThought.ChangeSubthought(node, node.Links);
                    return node;
                }
            }
            return null;
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

        protected virtual void OnActionEvent(VerbActionEventArgs e)
        {
            ActionEvent?.Invoke(this, e);
        }

        protected virtual void OnAddThoughtEvent(ThoughtNode e)
        {
            AddThoughtEvent?.Invoke(this, e);
        }

        private Node FindLinkById(int id)
        {
            throw new NotImplementedException();
        }

        public ThoughtNode GetThought(string thoughtText)
        {
            foreach(ThoughtNode thought in Thoughts)
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
