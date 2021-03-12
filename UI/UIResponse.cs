using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace conscious
{
    public class UIResponse : UIText
    {
        public Edge ResponseEdge { get; set; }

        public UIResponse(Edge responseEdge, SpriteFont font, string text, string name, Texture2D texture, Vector2 position) 
                         : base(font, text, name, texture, position)
        {
            ResponseEdge = responseEdge;
        }

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