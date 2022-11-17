using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace conscious
{
    /// <summary>Class <c>ControlsManagerr</c> implements a basic controls system 
    /// for the player to control the protagonist via mouse and keyboard.
    /// </summary>
    ///
    public class ControlsManager : IComponent
    {
        private Player _player;

        public Vector2 Direction { get; set; }

        public Vector2 MousePosition { get; set; }

        public ControlsManager(Player player)
        {
            _player = player;
        }

        public void Update(GameTime gameTime){
            KeyboardState currentKeyboardState = Keyboard.GetState();
            MouseState currentMouseState = Mouse.GetState();
            float totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (currentKeyboardState.IsKeyDown(Keys.Up))
                _player.MoveUp(totalSeconds);

            if(currentKeyboardState.IsKeyDown(Keys.Down))
                _player.MoveDown(totalSeconds);

            if (currentKeyboardState.IsKeyDown(Keys.Left))
                _player.MoveLeft(totalSeconds);

            if(currentKeyboardState.IsKeyDown(Keys.Right))
                _player.MoveRight(totalSeconds);

            if(MousePosition != Vector2.Zero)
            {
                _player.MoveToPoint(MousePosition, totalSeconds);
            }
            else if(Direction != Vector2.Zero)
            {
                _player.MoveToDirection(Direction);
            }
            
        }

        public void Draw(SpriteBatch spriteBatch){ }
    }
}