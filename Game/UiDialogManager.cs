using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;
using System.Linq;

namespace conscious
{
    public class UiDialogManager : IComponent
    {
        private UIText _currentText;
        private Node _currentNode;
        private List<UIComponent> _texts;
        private EntityManager _entityManager;
        private MoodStateManager _moodStateManager;
        private SpriteFont _displayFont;
        private Texture2D _pixel;
        private bool _displayActive;
        private float _displayTime;
        private float _currentTime;
        private bool _textInilized;
        private Vector2 _textPosition;
        private MouseState _lastMouseState;

        private bool _responseSelectionMode;
        private bool _responsesInitilized;
        private bool _isRoot;


        public List<Node> TreeStructure;
        public bool DialogActive { get; protected set; }

        public UiDialogManager(EntityManager entityManager,
                             MoodStateManager moodStateManager, 
                             SpriteFont displayFont, 
                             Texture2D pixel)
        {
            // General
            _entityManager = entityManager;
            _moodStateManager = moodStateManager;
            _displayFont = displayFont;
            _pixel = pixel;
            _texts = new List<UIComponent>();
            _lastMouseState = Mouse.GetState();

            // Dialog
            TreeStructure = new List<Node>();
            DialogActive = false;
            _responseSelectionMode = false;
            _responsesInitilized = false;
            _isRoot = true;

            // Current Text Displayment
            _displayActive = false;
            _textInilized = false;
            _displayTime = 3f;
            _currentTime = 0f;
            _textPosition = new Vector2(50f, 50f);
        }

        public IEnumerable<T> GetThingsOfType<T>() where T : UIComponent
        {
            return _texts.OfType<T>();
        }

        public void Update(GameTime gameTime)
        {
            if(DialogActive)
            {
                if(_isRoot == true)
                {
                    RemoveText();
                    _currentNode = GetNode(1);

                    DoDisplayText(_currentNode.GetLine());
                    _isRoot = false;
                }
                else if(_responseSelectionMode == false)
                {
                    CurrentTextDisplay(gameTime);
                }
                else
                {
                    if(_responsesInitilized == false)
                    {
                        InitilizeResponses();
                        _responsesInitilized = true;
                    }
                    else
                    {
                        ChooseResponse();
                    }
                }
            }
            else
            {
                CurrentTextDisplay(gameTime);
            }

            _lastMouseState = Mouse.GetState();
        }

        public void Draw(SpriteBatch spriteBatch){ }

        public void StartDialog(List<Node> tree)
        {
            TreeStructure = tree;
            DialogActive = true;
        }

        public Node GetNode(int nodeId)
        {
            foreach(Node node in TreeStructure)
            {
                if(node.GetId() == nodeId)
                {
                    return node;
                }
            }
            return null;
        }

        private void InitilizeResponses()
        {
            RemoveText();
            float offset = 0f;
            foreach(Edge edge in _currentNode.GetEdges()){
                Vector2 responsePosition = new Vector2(_textPosition.X, _textPosition.Y + offset);
                offset = offset + 20f;
                Edge responseEdge = edge;
                if(edge.MoodDependence == MoodState.None || edge.MoodDependence == _moodStateManager.moodState)
                {
                    UIResponse response = new UIResponse(edge,
                                                        _displayFont,
                                                        edge.GetLine(), 
                                                        "Response_" + (offset/20f).ToString(), 
                                                        _pixel, 
                                                        responsePosition);
                    AddText(response);
                }
            }
        }

        public void ChooseResponse()
        {
            MouseState currentMouseState = Mouse.GetState();
            if(currentMouseState.LeftButton == ButtonState.Released && _lastMouseState.LeftButton == ButtonState.Pressed)
            {
                foreach(UIResponse response in GetThingsOfType<UIResponse>())
                {
                    if(response.BoundingBox.Contains(currentMouseState.Position))
                    {
                        if(response.ResponseEdge.getNextNodeId() != 0)
                        {
                            _currentNode = GetNode(response.ResponseEdge.getNextNodeId());
                            RemoveText();
                            DoDisplayText(_currentNode.GetLine());
                            _responsesInitilized = false;
                            _responseSelectionMode = false;
                        }
                        else
                        {
                            TerminateDialog();
                        }
                        break;
                    }
                }
            }
        }

        public void TerminateDialog()
        {
            RemoveText();
            TreeStructure = new List<Node>();
            DialogActive = false;
            _responseSelectionMode = false;
            _responsesInitilized = false;
            _isRoot = true;
        }

        public void DoDisplayText(string displayText)
        {
            UIText text = new UIText(_displayFont, displayText, "Display Text", _pixel, _textPosition);
            RemoveText();
            _currentText = text;
            AddText(_currentText);
           _textInilized = true;
        }

        public void CurrentTextDisplay(GameTime gameTime)
        {
            if(_textInilized == true)
            {
                float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                _currentTime = elapsedSeconds;
                _displayActive = true;
                _textInilized = false;
            }
            if(_displayActive)
            {
                float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                _currentTime += elapsedSeconds;
                if(_currentTime > _displayTime)
                {
                    RemoveText();
                    _displayActive = false;
                    if(DialogActive)
                    {
                        _responseSelectionMode = true;
                    }
                }
            }
        }

        public void AddText(UIComponent text)
        {
            _texts.Add(text);
            _entityManager.AddEntity(text);
        }

        public void RemoveText(UIComponent text)
        {
            if(_texts.Contains(text))
            {
                _texts.Remove(text);
                _entityManager.RemoveEntity(text);
            }
        }

        public void RemoveText()
        {
            foreach(UIComponent text in _texts)
            {
                _entityManager.RemoveEntity(text);
            }
            _texts.Clear();
        }

        public void FillEntityManager()
        {
            foreach(UIComponent text in _texts)
            {
                _entityManager.AddEntity(text);
            }
        }
    }
}