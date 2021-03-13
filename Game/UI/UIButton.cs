using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace conscious
{
    public class UIButton : UIText
    {
        // fields
        private MouseState _currentMouse;
        private MouseState _previousMouse;
        private Color _penColor;
        private bool _isHovering;

        // properties
        public EventHandler _clickEvent;
        public override Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)Position.X,
                                     (int)Position.Y, 
                                     (int)Width, 
                                     (int)Height);
            }
        }

        public UIButton(EventHandler clickEvent, SpriteFont font, string text, string name, Texture2D texture, Vector2 position) 
                        : base(font, text, name, texture, position)
        {
            _clickEvent = clickEvent;
            _penColor = Color.Black;
            _isHovering = false;
        }

        public override void Update(GameTime gameTime)
        {
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            _isHovering = false;
            
            if(BoundingBox.Contains(_currentMouse.Position.ToVector2()))
            {
                _isHovering = true;

                if(_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
                {
                    _clickEvent?.Invoke(this, new EventArgs());
                    // Similar to:
                    // if(Click != null)
                    //     Click(this, new EventArgs());
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Color color = Color.White;

            if(_isHovering)
            {
                color = Color.Gray;
            }

            spriteBatch.Draw(EntityTexture, BoundingBox, color);
            
            if(!string.IsNullOrEmpty(_text))
            {
                float x = (BoundingBox.X + (BoundingBox.Width / 2)) - (_font.MeasureString(_text).X / 2);
                float y = (BoundingBox.Y + (BoundingBox.Height / 2)) - (_font.MeasureString(_text).Y / 2);

                spriteBatch.DrawString(_font, _text, new Vector2(x, y), _penColor);
            }
        }
    }
}