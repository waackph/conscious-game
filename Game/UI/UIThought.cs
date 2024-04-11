using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace conscious
{
    public class UIThought : UIText
    {
        public bool IsClickable { get; set; }
        public bool IsActive { get; set; }
        public bool IsUsed { get; set; }
        public bool IsVisited { get; set; }
        public bool DoDisplay;
        public UIThought(bool isClickable,
                         bool isVisited,
                         bool doDisplay,
                         SpriteFont font, 
                         string text, 
                         string name, 
                         Texture2D texture, 
                         Vector2 position, int drawOrder) 
                         : base(font, text, name, texture, position, drawOrder)
        {
            Collidable = true;
            IsActive = false;
            IsClickable = isClickable;
            IsVisited = isVisited;

            DoDisplay = doDisplay;
        }

        public override void Update(GameTime gameTime)
        {
            if(IsActive)
            {
                _color = Color.Beige;
            }
            else if(IsUsed)
            {
                _color = Color.AliceBlue;
            }
            else if(BoundingBox.Contains(Mouse.GetState().Position.ToVector2()) && IsClickable)
            {
                _color = Color.Brown;
            }
            else if(IsVisited)
            {
                _color = Color.Gray;
            }
            else
            {
                _color = Color.DarkGreen;
            }
        }

        public void SetPosition(float x, float y)
        {
            Position = new Vector2(x, y);
        }
    }
}