using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace conscious
{
    public class UIThought : UIText
    {
        public bool IsClickable { get; set; }
        public bool IsActiveThought { get; set; }
        public bool IsUsed { get; set; }
        public bool IsVisited { get; set; }
        public bool IsRootThought { get; set; }
        public bool DoDisplay;
        public UIThought(bool isClickable,
                         bool isVisited,
                         bool doDisplay,
                         SpriteFont font, 
                         string text, 
                         string name, 
                         Texture2D texture, 
                         Vector2 position, int drawOrder,
                         bool isRootThought = false) 
                         : base(font, text, name, texture, position, drawOrder)
        {
            Collidable = true;
            IsActiveThought = false;
            IsClickable = isClickable;
            IsVisited = isVisited;
            IsRootThought = isRootThought;

            DoDisplay = doDisplay;
        }

        public override void Update(GameTime gameTime)
        {
            if(IsActiveThought)
            {
                _color = Color.Sienna;
            }
            else if(IsUsed)
            {
                _color = Color.DarkGray;
            }
            else if(BoundingBox.Contains(Mouse.GetState().Position.ToVector2()) && IsClickable)
            {
                _color = Color.DarkSlateGray;
            }
            else if(IsVisited)
            {
                _color = Color.Gray;
            }
            else
            {
                _color = Color.Black;
            }
        }

        public void SetPosition(float x, float y)
        {
            Position = new Vector2(x, y);
        }
    }
}