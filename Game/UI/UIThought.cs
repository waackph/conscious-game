using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace conscious
{
    public class UIThought : UIText
    {
        public UIThought(SpriteFont font, 
                        string text, 
                        string name, 
                        Texture2D texture, 
                        Vector2 position) : base(font, text, name, texture, position)
        { }

        public override void Update(GameTime gameTime)
        {
            if(BoundingBox.Contains(Mouse.GetState().Position.ToVector2()))
            {
                _color = Color.Brown;
            }
            else if(_color != Color.Gray)
            {
                _color = Color.Gray;
            }
        }
    }
}