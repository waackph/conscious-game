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

        public UIText(SpriteFont font, string text, string name, Texture2D texture, Vector2 position) : base(name, texture, position)
        {
            _font = font;
            _text = Regex.Replace(text, "(.{" + 45 + "})", "$1" + Environment.NewLine);
            _color = Color.Black;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(!string.IsNullOrEmpty(_text))
            {
                base.Draw(spriteBatch);
                spriteBatch.DrawString(_font, _text, Position, _color);
            }
        }
        
        public float GetStringWidth()
        {
            return _font.MeasureString(_text).X;
        }

        public float GetStringHeight()
        {
            return _font.MeasureString(_text).Y;
        }
    }
}
