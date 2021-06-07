using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    public class UIArea : UIComponent
    {
        public UIArea(string name, Texture2D texture, Vector2 position) : base(name, texture, position)
        {
            Collidable = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(EntityTexture, Position,
                             null, Color.White, Rotation, 
                             new Vector2(Width/2, Height/2),
                             new Vector2(2, 2),
                             SpriteEffects.None, 0f);
        }
    }
}