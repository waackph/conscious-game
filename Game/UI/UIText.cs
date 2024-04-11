using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text.RegularExpressions;

namespace conscious
{
    public class UIText : UIComponent
    {
        protected string _text;
        protected SpriteFont _font;
        protected Color _color;

        public override Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)Position.X,
                                     (int)Position.Y, 
                                     (int)GetStringWidth(), 
                                     (int)GetStringHeight());
            }
        }

        public UIText(SpriteFont font, string text, string name, Texture2D texture, Vector2 position, int drawOrder) 
        : base(name, texture, position, drawOrder)
        {
            _font = font;
            _text = Regex.Replace(text, "(.{" + 45 + "})", "$1" + Environment.NewLine);
            _color = Color.Black;

            DrawOrder = 7;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            bool isInvalid = IsStringInvalid(_text);

            if(!string.IsNullOrEmpty(_text))  // && !_text.Contains('\r') && !_text.Contains('\n') && !isInvalid)
            {
                base.Draw(spriteBatch);
                spriteBatch.DrawString(_font, _text, Position, _color);
            }
        }
        
        public float GetStringWidth()
        {
            bool isInvalid = IsStringInvalid(_text);
            float width = 5f;
            // if(!isInvalid)
            width = _font.MeasureString(_text).X;
            return width;
        }

        public void UpdateText(string text)
        {
            _text = text;
        }

        public float GetStringHeight()
        {
            bool isInvalid = IsStringInvalid(_text);
            float height = 5f;
            // if(!isInvalid)
            height = _font.MeasureString(_text).Y;

            return height;
        }

        private bool IsStringInvalid(string text)
        {
            bool isInvalid = false;
            foreach(char ch in text)
            {
                if(_font.Characters.Contains(ch))
                {
                    isInvalid = true;
                    break;
                }
            }
            return isInvalid;
        }
    }
}
