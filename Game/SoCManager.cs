using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace conscious
{
    public class SoCManager : IComponent
    {
        private Queue<ThoughtNode> _thoughts;
        private UiDisplayThoughtManager _uiDisplayThought;
        private List<ThoughtLink> _currentSubthoughtLinks;
        private ThoughtNode _currentSubthought;
        private ThoughtNode _currentThought;

        public SoCManager()
        {
            
        }

        public void Update(GameTime gameTime){ }

        public void Draw(SpriteBatch spriteBatch){ }

        public void AddThought(ThoughtNode thought)
        {
            _thoughts.Enqueue(thought);
            _uiDisplayThought.AddThought(thought);
        }

        public void RemoveThought(ThoughtNode thought)
        {
            _thoughts.Dequeue();
        }

        public void SelectThought(string thoughtName)
        {
            ThoughtNode node = GetThought(thoughtName);
            if(node.HasLinks())
            {
                _uiDisplayThought.StartThoughtMode(node, node.Links);
            }
        }

        public void SelectSubthought(string thoughtName)
        {
            ThoughtLink option = GetOption(thoughtName);
            ThoughtNode node = option.NextNode;
            if(node == null || !node.HasLinks())
            {
                _uiDisplayThought.EndThoughtMode();
            }
            else
            {
                _uiDisplayThought.ChangeSubthought(node, node.Links);
                _currentSubthoughtLinks = node.Links;
            }
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
