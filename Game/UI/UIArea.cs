using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    public class UIArea : UIComponent
    {
        public UIArea(string name, Texture2D texture, Vector2 position, int drawOrder) 
        : base(name, texture, position, drawOrder)
        {
            Collidable = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(EntityTexture, Position,
                             null, Color.White, Rotation, 
                             new Vector2(Width/2, Height/2),
                             Vector2.One,
                             SpriteEffects.None, 0f);
        }

        public override Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)Position.X - Width/2, 
                                     (int)Position.Y - Height/2,
                                     Width, Height);
            }
        }
    }
}