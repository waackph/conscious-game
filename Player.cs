using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    public class Player : Thing
    {
        private SpriteEffects _flip;
        private float _currentPositionDelta;
        private bool _isMoving;
        private bool _lastIsMoving;
        private float _playerSpeed;

        public Vector2 LastPosition;
        public AnimatedSprite IdleAnimation;
        public AnimatedSprite MoveAnimation;

        public Player(Texture2D moveTexture, string name, Texture2D texture, Vector2 position) : base(name, texture, position)
        {  
            IdleAnimation = new AnimatedSprite(texture, 1, 1, Width, Height, 0f);
            MoveAnimation = new AnimatedSprite(moveTexture, 1, 10, Width, Height, 0f);

            _flip = SpriteEffects.None;
            _playerSpeed = 500f;
            _isMoving = false;
            _lastIsMoving = _isMoving;
            LastPosition = position;
            DrawOrder = 2;
        }

        public override void Update(GameTime gameTime)
        {
            // Decide if Sprite should be flipped
            _currentPositionDelta = LastPosition.X - Position.X;
            if(_currentPositionDelta < 0){
                _flip = SpriteEffects.FlipHorizontally;
            }
            else if(_currentPositionDelta > 0){
                _flip = SpriteEffects.None;
            }

            // Update animations
            if(Position == LastPosition)
            {
                IdleAnimation.Update();
                _isMoving = false;
            }
            else
            {
                MoveAnimation.Update();
                _isMoving = true;
            }
            if(_lastIsMoving != _isMoving){
                IdleAnimation.resetAnimation();
                MoveAnimation.resetAnimation();
            }

            LastPosition = Position;
            _lastIsMoving = _isMoving;
        }

        public void MoveUp(float totalSeconds){
            Position.Y -= _playerSpeed * totalSeconds;
        }

        public void MoveDown(float totalSeconds){
            Position.Y += _playerSpeed * totalSeconds;
        }

        public void MoveLeft(float totalSeconds){
            Position.X -= _playerSpeed * totalSeconds;
        }

        public void MoveRight(float totalSeconds){
            Position.X += _playerSpeed * totalSeconds;
        }

        public void MoveToPoint(Vector2 mousePosition, float totalSeconds){
            Vector2 posDelta = mousePosition - Position;
            posDelta.Normalize();
            posDelta = posDelta * (_playerSpeed);
            Position = Position + posDelta  * totalSeconds;
        }

        public void MoveToDirection(Vector2 direction){
            Position = Position + direction*5;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(_isMoving)
            {
                MoveAnimation.Draw(spriteBatch, Position, _flip);
            }
            else
            {
                IdleAnimation.Draw(spriteBatch, Position, _flip);
            }
        }

        public float GetDistance(Entity entity){
            return Vector2.Distance(entity.Position, Position);
        }
    }
}
