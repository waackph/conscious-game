using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    public class Sprite
    {
        public Texture2D Texture{ get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Rotation;
        public Sprite(Texture2D texture, int width, int height, float rotation)
        {
            Texture = texture;
            Width = width;
            Height = height;
            Rotation = rotation;
        } 

        public virtual void Update(){}

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 location, SpriteEffects flip){

            spriteBatch.Draw(Texture, location,
                             null, Color.White, Rotation, 
                             new Vector2(Width/2, Height/2),
                             Vector2.One,
                             flip, 0f);
        }
    }
}