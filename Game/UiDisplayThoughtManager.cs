using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;
using System.Linq;

namespace conscious
{
    public class UiDisplayThoughtManager : IComponent
    {
        private EntityManager _entityManager;
        private SoCManager _socManager;
        private Queue<UIThought> _thoughts;
        private SpriteFont _font;
        private Texture2D _pixel;
        private float _bgX;
        private float _bgY;
        private float _offsetY;
        private float _offsetX;
        private float _thoughtOffsetY;
        private float _thoughtOffsetX;
        private int _maxThoughts;
        private UIArea _consciousnessBackground;
        private UIArea _subthoughtBackground;
        private List<UIThought> _currentSubthoughtLinks;
        private UIThought _currentSubthought;
        private UIThought _currentThought;
        private MouseState _lastMouseState;
        public bool IsInThoughtMode { get; protected set; }

        public UiDisplayThoughtManager(EntityManager entityManager, SoCManager socManager, SpriteFont font, Texture2D pixel)
        {
            _entityManager = entityManager;
            _socManager = socManager;
            _thoughts = new Queue<UIThought>();
            _maxThoughts = 2;
            // Main SoC Area
            _bgX = 300f; // 150f;
            _bgY = 900f; // 500f;
            _offsetY = 20f;
            _offsetX = 250f;
            // In Thought Area
            _thoughtOffsetY = _bgY;
            _thoughtOffsetX = _bgX + 50f;
            
            _font = font;
            _pixel = pixel;
           
            _lastMouseState = Mouse.GetState();
            _currentSubthought = null;
            _currentSubthoughtLinks = null;
            _currentThought = null;
            IsInThoughtMode = false;

        }

        public void LoadContent(Texture2D consciousnessBackground)
        {
            Vector2 bgPosition = new Vector2(_bgX, _bgY);
            _consciousnessBackground = new UIArea("SoC Background", consciousnessBackground, bgPosition);

            Vector2 thoughtBgPosition = new Vector2(_bgX + _thoughtOffsetX, _bgY + _thoughtOffsetY);
            _subthoughtBackground = new UIArea("Thought Background", consciousnessBackground, thoughtBgPosition);
        }

        public void Update(GameTime gameTime)
        {
            _lastMouseState = Mouse.GetState();
        }

        public void Draw(SpriteBatch spriteBatch){ }

        public void AddThought(ThoughtNode thought)
        {
            // Add if the UI not already contains the thought
            if(!containsThoughtNode(_thoughts, thought))
            {
                UIThought uiThought = convertNodeToUi(thought);
                if(_thoughts.Count + 1 > _maxThoughts)
                {
                    RemoveThought();
                }
                // render new positions of thoughts
                uiThought = CalculateThoughtPositions(uiThought);
                // add new thought
                _thoughts.Enqueue(uiThought);
                _entityManager.AddEntity(uiThought);
            }
        }

        private UIThought CalculateThoughtPositions(UIThought thought)
        {
            int thoughtNumber = 0;
            float heightOffset = 0f;
            //Update position of thoughts in queue
            foreach(UIThought th in _thoughts)
            {
                _entityManager.RemoveEntity(th);
                th.SetPosition(_bgX - _offsetX,
                                _bgY + thoughtNumber * _offsetY + heightOffset);
                heightOffset += th.BoundingBox.Height;
                _entityManager.AddEntity(th);
                thoughtNumber++;
            }
            thought.SetPosition(_bgX - _offsetX,
                                _bgY + thoughtNumber*_offsetY + heightOffset);
            return thought;
        }

        public IEnumerable<T> GetThingsOfType<T>() where T : UIComponent
        {
            return _thoughts.OfType<T>();
        }

        public void CheckThoughtClicked()
        {
            MouseState currentMouseState = Mouse.GetState();
            if(currentMouseState.LeftButton == ButtonState.Released && _lastMouseState.LeftButton == ButtonState.Pressed)
            {
                foreach(UIThought uiThought in GetThingsOfType<UIThought>())
                {
                    if(uiThought.BoundingBox.Contains(currentMouseState.Position))
                    {
                        // Do logic stuff (run tree logic in SoCManager and maybe add a thought to UI or terminate thought)
                        // Check if click was in SoC
                        if(_thoughts.Contains(uiThought))
                        {
                            // TODO: call Manager for new subthought
                            _socManager.SelectThought(uiThought.Name);
                        }
                        else if(_currentSubthoughtLinks != null && _currentSubthoughtLinks.Contains(uiThought))
                        {
                            // TODO: call Manager for traverse in current subthought
                            _socManager.SelectSubthought(uiThought.Name);
                        }
                        break;
                    }
                }
            }
        }

        public void StartThoughtMode(ThoughtNode node, List<ThoughtLink> links)
        {
            IsInThoughtMode = true;
            _currentThought = convertNodeToUi(node);
            _currentSubthoughtLinks = convertLinksToUi(links);
            _entityManager.AddEntity(_subthoughtBackground);
            calculateSubthoughtPositions();
            addSubthought();
        }

        public void EndThoughtMode()
        {
            IsInThoughtMode = false;
            _currentThought = null;
            _currentSubthought = null;
            _currentSubthoughtLinks = null;
            _entityManager.RemoveEntity(_subthoughtBackground);
        }

        private void calculateSubthoughtPositions()
        {
            int thoughtNumber = 0;
            float heightOffset = 0f;
            if(_currentSubthought != null)
            {
                _currentSubthought.SetPosition(_thoughtOffsetX - _offsetX,
                                            _thoughtOffsetY + thoughtNumber*_offsetY + heightOffset);
                thoughtNumber++;
                heightOffset += _currentSubthought.Height;
            }
            foreach(UIThought option in _currentSubthoughtLinks)
            {
                option.SetPosition(_thoughtOffsetX - _offsetX,
                                   _thoughtOffsetY + thoughtNumber*_offsetY + heightOffset);
                thoughtNumber++;
                heightOffset += option.Height;
            }
        }

        public void ChangeSubthought(ThoughtNode node, List<ThoughtLink> links)
        {
            removeSubthought();
            _currentSubthought = convertNodeToUi(node);
            _currentSubthoughtLinks = convertLinksToUi(links);
            calculateSubthoughtPositions();
            addSubthought();
        }

        private void removeSubthought()
        {
            if(_currentSubthought != null)
            {
                _entityManager.RemoveEntity(_currentSubthought);
                _currentSubthought = null;
            }
            foreach(UIThought link in _currentSubthoughtLinks)
            {
                _entityManager.RemoveEntity(link);
            }
            _currentSubthoughtLinks = null;
        }

        private void addSubthought()
        {
            if(_currentSubthought != null)
            {
                _entityManager.AddEntity(_currentSubthought);
            }
            foreach(UIThought link in _currentSubthoughtLinks)
            {
                _entityManager.AddEntity(link);
            }
        }

        private bool containsThoughtNode(Queue<UIThought> thoughts, ThoughtNode node)
        {
            foreach(UIThought thought in thoughts)
            {
                if(thought.Name == node.Thought)
                {
                    return true;
                }
            }
            return false;
        }

        public void RemoveThought()
        {
            UIThought thought = _thoughts.Dequeue();
            if(thought != null)
            {
                _entityManager.RemoveEntity(thought);
            }
        }

        private UIThought convertNodeToUi(ThoughtNode node)
        {
            if(node != null)
            {
                UIThought uiThought = new UIThought(_font, 
                                                    node.Thought, node.Thought, 
                                                    _pixel, 
                                                    Vector2.One);
                return uiThought;
            }
            else
            {
                return null;
            }
        }

        private List<UIThought> convertLinksToUi(List<ThoughtLink> links)
        {
            List<UIThought> uiOptions = new List<UIThought>();
            foreach(ThoughtLink link in links)
            {
                // TODO: add a disabled style, if current moodState is not valid for this option
                UIThought uiThought = new UIThought(_font, 
                                                    link.Option, link.Option, 
                                                    _pixel, 
                                                    Vector2.One);
            }
            return uiOptions;
        }

        public void FillEntityManager()
        {
            _entityManager.AddEntity(_consciousnessBackground);
        }
    }
}