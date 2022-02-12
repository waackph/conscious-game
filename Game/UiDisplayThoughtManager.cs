using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;

namespace conscious
{
    public class UiDisplayThoughtManager : IComponent
    {
        private EntityManager _entityManager;
        private MoodStateManager _moodStateManager;
        private SoCManager _socManager;
        private Cursor _cursor;
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

        public UiDisplayThoughtManager(EntityManager entityManager, MoodStateManager moodStateManager, SoCManager socManager, Cursor cursor, SpriteFont font, Texture2D pixel)
        {
            _entityManager = entityManager;
            _moodStateManager = moodStateManager;
            _socManager = socManager;
            _socManager.AddThoughtEvent += AddThoughtFromSoC;
            _socManager.FinishInteractionEvent += FinishThought;

            _cursor = cursor;

            _thoughts = new Queue<UIThought>();

            _maxThoughts = 2;
            // Main SoC Area
            _bgX = 1600f; // 150f;
            _bgY = 200f; // 500f;
            _offsetY = 20f;
            _offsetX = 250f;
            // In Thought Area
            _thoughtOffsetY = 55;
            _thoughtOffsetX = 0; //_bgX + 210f;
            
            _font = font;
            _pixel = pixel;
           
            _lastMouseState = Mouse.GetState();
            _currentSubthought = null;
            _currentSubthoughtLinks = null;
            _currentThought = null;
            IsInThoughtMode = false;
        }

        public void LoadContent(Texture2D consciousnessBackground, Texture2D consciousnessBackgroundSubthought)
        {
            Vector2 bgPosition = new Vector2(_bgX, _bgY);
            _consciousnessBackground = new UIArea("SoC Background", consciousnessBackground, bgPosition);

            Vector2 thoughtBgPosition = new Vector2(_bgX + _thoughtOffsetX, 
                                                    _bgY + _consciousnessBackground.Height + _consciousnessBackground.Height/2 + _thoughtOffsetY);
            _subthoughtBackground = new UIArea("Thought Background", consciousnessBackgroundSubthought, thoughtBgPosition);
        }

        public void Update(GameTime gameTime)
        {
            CheckThoughtClicked();
            _lastMouseState = Mouse.GetState();
        }

        public void Draw(SpriteBatch spriteBatch){ }

        // private void UpdateThoughtQueue()
        // {
        //     if(!_thoughts.ToList().ConvertAll<string>(_thoughts => _thoughts.Name).Equals(_socManager.Thoughts.ToList().ConvertAll<string>(Thoughts => Thoughts.Thought)))
        //     {
        //         foreach(ThoughtNode node in _socManager.Thoughts)
        //         {
        //             AddThought(node);
        //         }
        //     }
        // }

        public void AddThoughtFromSoC(object sender, ThoughtNode e)
        {
            AddThought(e);
        }

        public void FinishThought(object sender, bool e)
        {
            if(_currentThought != null) 
            {
                if(e)
                {
                    _currentThought.IsUsed = true;
                }
                _currentThought.IsActive = false;
                _currentThought = null;
            }
        }

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
            float uiXPos = _bgX - _offsetX;
            float uiYPos = _bgY - _consciousnessBackground.Height/2;
            int thoughtNumber = 0;
            float heightOffset = 0f;
            
            //Update position of thoughts in queue
            foreach(UIThought th in _thoughts)
            {
                _entityManager.RemoveEntity(th);
                th.SetPosition(uiXPos,
                               uiYPos + thoughtNumber * _offsetY + heightOffset);
                heightOffset += th.BoundingBox.Height;
                _entityManager.AddEntity(th);
                thoughtNumber++;
            }
            thought.SetPosition(uiXPos,
                                uiYPos + thoughtNumber * _offsetY + heightOffset);
            return thought;
        }

        // TODO: Do this in the UI Manager and call the Mode-Methods from there
        public void CheckThoughtClicked()
        {
            MouseState currentMouseState = Mouse.GetState();
            if(currentMouseState.LeftButton == ButtonState.Released && _lastMouseState.LeftButton == ButtonState.Pressed)
            {
                foreach(UIThought uiThought in _entityManager.GetEntitiesOfType<UIThought>())
                {
                    if(uiThought.BoundingBox.Contains(_cursor.Position) && !uiThought.IsUsed && !uiThought.IsActive)
                    {
                        ThoughtNode node;
                        // Do logic stuff (run tree logic in SoCManager and maybe add a thought to UI or terminate thought)
                        // Check if click was in SoC
                        if(_thoughts.Contains(uiThought))
                        {
                            node = _socManager.SelectThought(uiThought.Name);
                            if(node != null)
                            {
                                if(IsInThoughtMode)
                                {
                                    EndThoughtMode();
                                }
                                if(_currentThought != null)
                                    _currentThought.IsActive = false;
                                _currentThought = uiThought;
                                _currentThought.IsActive = true;
                                StartThoughtMode(node, node.Links);
                            }
                        }
                        else if(_currentSubthoughtLinks != null && _currentSubthoughtLinks.Contains(uiThought))
                        {
                            node = _socManager.SelectSubthought(uiThought.Name);
                            if(node == null)
                            {
                                EndThoughtMode();
                            }
                            else
                            {
                                ChangeSubthought(node, node.Links);
                            }
                        }
                        break;
                    }
                }
            }
        }

        public void StartThoughtMode(ThoughtNode node, List<ThoughtLink> links)
        {
            // TODO: Restart Thought Mode when another thought is clicked 
            //       while the subthought of another is already active
            IsInThoughtMode = true;
            _socManager.IsInThoughtMode = true;
            _entityManager.AddEntity(_subthoughtBackground);
            _currentSubthought = convertNodeToUi(node, doDisplay:true);
            _currentSubthoughtLinks = convertLinksToUi(links);
            calculateSubthoughtPositions();
            addSubthought();
        }

        public void EndThoughtMode()
        {
            IsInThoughtMode = false;
            _socManager.IsInThoughtMode = false;
            removeSubthought();
            _entityManager.RemoveEntity(_subthoughtBackground);
        }

        public void ChangeSubthought(ThoughtNode node, List<ThoughtLink> links)
        {
            removeSubthought();
            _currentSubthought = convertNodeToUi(node);
            _currentSubthoughtLinks = convertLinksToUi(links);
            calculateSubthoughtPositions();
            addSubthought();
        }

        private void calculateSubthoughtPositions()
        {
            float uiXPos = _bgX + _thoughtOffsetX - _offsetX;
            float uiYPos = _bgY + _consciousnessBackground.Height/2 + _thoughtOffsetY/2;
            int thoughtNumber = 0;
            float heightOffset = 0f;
            if(_currentSubthought != null && _currentSubthought.DoDisplay)
            {
                _currentSubthought.SetPosition(uiXPos,
                                               uiYPos + thoughtNumber * _offsetY + heightOffset);
                thoughtNumber++;
                heightOffset += _currentSubthought.BoundingBox.Height;
            }
            foreach(UIThought option in _currentSubthoughtLinks)
            {
                // add the offset to better differentiate the characters response from the options
                int optionOffset = 10;
                option.SetPosition(uiXPos + optionOffset,
                                   uiYPos + thoughtNumber * _offsetY + heightOffset);
                thoughtNumber++;
                heightOffset += option.BoundingBox.Height;
            }
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
            // We only want to see the options aka links in this menu
            if(_currentSubthought != null && _currentSubthought.DoDisplay)
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

        private UIThought convertNodeToUi(ThoughtNode node, bool doDisplay=true)
        {
            if(node != null)
            {
                bool isClickable = false;
                if(node.HasLinks() && node.IsRoot)
                {
                    isClickable = true;
                }
                UIThought uiThought = new UIThought(isClickable,
                                                    false,
                                                    doDisplay,
                                                    _font, 
                                                    node.Thought, node.Thought, 
                                                    _pixel, 
                                                    Vector2.One);
                if(node.IsRoot)
                    uiThought.IsUsed = node.IsUsed;
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
                if(!link.IsLocked && link.MoodValid(_moodStateManager.moodState))
                {
                    // TODO: add a disabled style, if current moodState is not valid for this option
                    UIThought uiThought = new UIThought(isClickable:true,
                                                        isVisited:link.IsVisited,
                                                        doDisplay:true,
                                                        _font, 
                                                        link.Option, link.Option, 
                                                        _pixel, 
                                                        Vector2.One);
                    if(typeof(FinalThoughtLink) == link.GetType() && link.IsVisited)
                    {
                        FinalThoughtLink finalLink = (FinalThoughtLink)link;
                        if(finalLink.IsSuccessEdge)
                            uiThought.IsUsed = true;
                    }
                    uiOptions.Add(uiThought);
                }
            }
            return uiOptions;
        }

        public void FillEntityManager()
        {
            _entityManager.AddEntity(_consciousnessBackground);
            if(_thoughts.Count > 0)
            {
                foreach(UIThought thought in _thoughts)
                {
                    _entityManager.AddEntity(thought);
                }
                // if there is a subthought currently selected, render it
                if(_currentSubthought != null && _currentSubthoughtLinks != null)
                {
                    _entityManager.AddEntity(_subthoughtBackground);
                    _entityManager.AddEntity(_currentSubthought);
                    foreach(UIThought thought in _currentSubthoughtLinks)
                    {
                        _entityManager.AddEntity(thought);
                    }
                }
            }
        }
    }
}