using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace conscious
{
    public class UIThought : UIText
    {
        private bool _isClickable;
        public bool DoDisplay;
        public UIThought(bool isClickable,
                         bool doDisplay,
                         SpriteFont font, 
                         string text, 
                         string name, 
                         Texture2D texture, 
                         Vector2 position) : base(font, text, name, texture, position)
        {
            Collidable = true;
            _isClickable = isClickable;

            DoDisplay = doDisplay;
        }

        public override void Update(GameTime gameTime)
        {
            if(BoundingBox.Contains(Mouse.GetState().Position.ToVector2()) && _isClickable)
            {
                _color = Color.Brown;
            }
            else if(_color != Color.Gray)
            {
                _color = Color.Gray;
            }
        }

        public void SetPosition(float x, float y)
        {
            Position = new Vector2(x, y);
        }
    }
}