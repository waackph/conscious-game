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
        private PlayerState _playerState;

        public Vector2 LastPosition;
        public AnimatedSprite IdleAnimation;
        public AnimatedSprite MoveAnimation;

        public Player(Texture2D moveTexture, 
                      int id,
                      ThoughtNode thought,
                      string name, 
                      Texture2D texture,
                      Vector2 position) : base(id, thought, name, texture, position)
        {  
            IdleAnimation = new AnimatedSprite(texture, 1, 2, (Width/2), Height, 0f, 800);
            MoveAnimation = new AnimatedSprite(moveTexture, 4, 2, (Width/2), Height, 0f, 60);

            _flip = SpriteEffects.None;
            _playerSpeed = 400f;
            _isMoving = false;
            _playerState = PlayerState.Idle;
            _lastIsMoving = _isMoving;
            LastPosition = position;
            DrawOrder = 5;
        }

        public override Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)Position.X - (Width/2)/2, 
                                     (int)Position.Y - Height/2,
                                     (Width/2), Height);
            }
        }

        public override Rectangle CollisionBox
        {
            get
            {
                return new Rectangle((int)Position.X - (Width/6)/2, 
                                     (int)Position.Y + Height/2-20,
                                     (Width/6), 20);
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Decide if Sprite should be flipped
            _currentPositionDelta = LastPosition.X - Position.X;
            if(_currentPositionDelta > 0){
                _flip = SpriteEffects.FlipHorizontally;
            }
            else if(_currentPositionDelta < 0){
                _flip = SpriteEffects.None;
            }

            // Update animations
            if(_playerState == PlayerState.Idle || _playerState == PlayerState.Walk)
            {
                if(Position == LastPosition)
                {
                    IdleAnimation.Update(gameTime);
                    _isMoving = false;
                    _playerState = PlayerState.Idle;
                }
                else
                {
                    MoveAnimation.Update(gameTime);
                    _isMoving = true;
                    _playerState = PlayerState.Walk;
                }
                if(_lastIsMoving != _isMoving){
                    IdleAnimation.resetAnimation();
                    MoveAnimation.resetAnimation();
                }

                LastPosition = Position;
                _lastIsMoving = _isMoving;
            }
            else if(_playerState == PlayerState.Sleep) 
            {
                // add sleep logic here
            }
        }

        public void GoToSleep()
        {
            _playerState = PlayerState.Sleep;
        }

        public void WakeUp()
        {
            _playerState = PlayerState.Idle;
        }

        public void MoveUp(float totalSeconds)
        {
            if(_playerState == PlayerState.Idle || _playerState == PlayerState.Walk)
                Position.Y -= _playerSpeed * totalSeconds;
        }

        public void MoveDown(float totalSeconds)
        {
            if(_playerState == PlayerState.Idle || _playerState == PlayerState.Walk)
                Position.Y += _playerSpeed * totalSeconds;
        }

        public void MoveLeft(float totalSeconds)
        {
            if(_playerState == PlayerState.Idle || _playerState == PlayerState.Walk)
                Position.X -= _playerSpeed * totalSeconds;
        }

        public void MoveRight(float totalSeconds)
        {
            if(_playerState == PlayerState.Idle || _playerState == PlayerState.Walk)
                Position.X += _playerSpeed * totalSeconds;
        }

        public void MoveToPoint(Vector2 mousePosition, float totalSeconds)
        {
            if(_playerState == PlayerState.Idle || _playerState == PlayerState.Walk)
            {
                Vector2 posDelta = mousePosition - Position;
                posDelta.Normalize();
                posDelta = posDelta * (_playerSpeed);
                Position = Position + posDelta  * totalSeconds;
            }
        }

        public void MoveToDirection(Vector2 direction)
        {
            if(_playerState == PlayerState.Idle || _playerState == PlayerState.Walk)
                Position = Position + direction*5;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(_playerState == PlayerState.Walk)
            {
                MoveAnimation.Draw(spriteBatch, Position, _flip);
            }
            else if(_playerState == PlayerState.Idle)
            {
                IdleAnimation.Draw(spriteBatch, Position, _flip);
            }
            else if(_playerState == PlayerState.Sleep)
            {
                // Add Sleep Animation
            }
        }

        public float GetDistance(Entity entity)
        {
            return Vector2.Distance(entity.Position, Position);
        }
    }
}
