
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace conscious
{
    /// <summary>Class <c>Cursor</c> implements a UI representation of the cursor 
    /// to interact with Things in the Room.
    /// </summary>
    ///
    public class Cursor : UIComponent
    {
        private SpriteFont _cursorFont;
        private float scaleFactor = 0.05f;
        private Texture2D _pixel;
        
        public string InteractionLabel { get; set; }
        public Vector2 MouseCoordinates { get; set; }
        public Matrix InverseTransform { get; set; }
        public Texture2D LightMask { get; protected set; }

        public Cursor(SpriteFont cursorFont, Matrix inverseTransform, string name,
                      Texture2D texture, Vector2 position, int drawOrder, Texture2D lightMask = null, Texture2D pixel = null)
                    : base(name, texture, position, drawOrder)
        {
            _cursorFont = cursorFont;
            InverseTransform = inverseTransform;
            InteractionLabel = "";
            DrawOrder = 8;
            FixedDrawPosition = true;
            Rotation = 90f;
            _sprite = new Sprite(EntityTexture, Width, Height, Rotation);
            LightMask = lightMask;
            _pixel = pixel;
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
                             //  90f,
                             0f,
                             new Vector2(Width/2, Height/2), 
                             new Vector2(scaleFactor, scaleFactor), 
                             SpriteEffects.None, 
                             0f);
            
            if (InteractionLabel != "")
            {
                Vector2 textSize = _cursorFont.MeasureString(InteractionLabel);
                Color textColor = Color.MintCream;
                spriteBatch.Draw(_pixel, new Rectangle((int)Position.X, (int)(Position.Y + (Height/2)*scaleFactor), (int)textSize.X, (int)textSize.Y), new Color(0, 0, 0, 128)); // Semi-transparent black
                spriteBatch.DrawString(_cursorFont, InteractionLabel, new Vector2(Position.X, Position.Y + (Height/2)*scaleFactor), textColor);
            }
        }
    }
}