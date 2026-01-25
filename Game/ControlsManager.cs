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
        private EntityManager _entityManager;
        private KeyboardState _previousKeyState = Keyboard.GetState();

        public Vector2 Direction { get; set; }

        public Vector2 MousePosition { get; set; }

        public ControlsManager(Player player, EntityManager entityManager)
        {
            _player = player;
            _entityManager = entityManager;
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            MouseState currentMouseState = Mouse.GetState();
            float totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (IsKeyPressed(Keys.Up, currentKeyboardState, _previousKeyState, false))
                _player.MoveUp(totalSeconds);

            if (IsKeyPressed(Keys.Down, currentKeyboardState, _previousKeyState, false))
                _player.MoveDown(totalSeconds);

            if (IsKeyPressed(Keys.Left, currentKeyboardState, _previousKeyState, false))
                _player.MoveLeft(totalSeconds);

            if (IsKeyPressed(Keys.Right, currentKeyboardState, _previousKeyState, false))
                _player.MoveRight(totalSeconds);

            if (MousePosition != Vector2.Zero)
            {
                _player.MoveToPoint(MousePosition, totalSeconds);
            }
            else if (Direction != Vector2.Zero)
            {
                _player.MoveToDirection(Direction, totalSeconds);
            }

            if (IsKeyPressed(Keys.F, currentKeyboardState, _previousKeyState, true))
                _entityManager.ToggleFlashlight();

            _previousKeyState = currentKeyboardState;
        }

        public void Draw(SpriteBatch spriteBatch) { }
        
        public static bool IsKeyPressed(Keys key, KeyboardState currentState, KeyboardState previouseState, bool oneShot)
        {
            if (!oneShot) return currentState.IsKeyDown(key);
            return currentState.IsKeyDown(key) && !previouseState.IsKeyDown(key);
        }
    }
}