
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace conscious
{
    public class Cursor : UIComponent
    {
        private SpriteFont _cursorFont;
        
        public string InteractionLabel { get; set; }
        public Vector2 MouseCoordinates { get; set; }
        public Matrix InverseTransform { get; set; }

        public Cursor(SpriteFont cursorFont, Matrix inverseTransform, string name, Texture2D texture, Vector2 position) : base(name, texture, position)
        {
            _cursorFont = cursorFont;
            InverseTransform = inverseTransform;
            InteractionLabel = "";
            DrawOrder = 8;
            FixedDrawPosition = true;
            Rotation = 90f;
            _sprite = new Sprite(EntityTexture, Width, Height, Rotation);
        }

        public override void Update(GameTime gameTime)
        {
            Position = Mouse.GetState().Position.ToVector2();
            MouseCoordinates = Vector2.Transform(Position, InverseTransform);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(EntityTexture, 
                             Position, 
                             null, 
                             Color.White, 
                             90f, 
                             new Vector2(Width/1.8f, Height/1.2f), 
                             Vector2.One, 
                             SpriteEffects.None, 
                             0f);
            
            if (InteractionLabel != "")
            {
                spriteBatch.DrawString(_cursorFont, InteractionLabel, new Vector2(Position.X, Position.Y+Height/2), Color.Tomato);
            }
        }
    }
}