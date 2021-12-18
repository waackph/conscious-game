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
        
        public PlayerState PlayerState;
        public Vector2 LastPosition;
        public AnimatedSprite IdleAnimation;
        public AnimatedSprite MoveAnimation;
        public AnimatedSprite SleepAnimation;

        public Player(Texture2D moveTexture, 
                      Texture2D sleepTexture, 
                      int id,
                      ThoughtNode thought,
                      string name, 
                      Texture2D texture,
                      Vector2 position) : base(id, thought, name, texture, position)
        {  
            IdleAnimation = new AnimatedSprite(texture, 1, 2, (Width/2), Height, 0f, 800);
            MoveAnimation = new AnimatedSprite(moveTexture, 4, 2, (Width/2), Height, 0f, 60);
            SleepAnimation = new AnimatedSprite(sleepTexture, 1, 2, (Width/2), Height, 0f, 800);

            _flip = SpriteEffects.None;
            _playerSpeed = 400f;
            _isMoving = false;
            PlayerState = PlayerState.Sleep;
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
            if(PlayerState == PlayerState.Idle || PlayerState == PlayerState.Walk)
            {
                if(Position == LastPosition)
                {
                    IdleAnimation.Update(gameTime);
                    _isMoving = false;
                    PlayerState = PlayerState.Idle;
                }
                else
                {
                    MoveAnimation.Update(gameTime);
                    _isMoving = true;
                    PlayerState = PlayerState.Walk;
                }
                if(_lastIsMoving != _isMoving){
                    IdleAnimation.resetAnimation();
                    MoveAnimation.resetAnimation();
                }

                LastPosition = Position;
                _lastIsMoving = _isMoving;
            }
            else if(PlayerState == PlayerState.Sleep) 
            {
                SleepAnimation.Update(gameTime);
            }
            else if(PlayerState == PlayerState.None)
            {
                // make player vanish aka do nothing
            }
        }

        public void GoToSleep()
        {
            PlayerState = PlayerState.Sleep;
        }

        public void WakeUp()
        {
            PlayerState = PlayerState.Idle;
        }

        public void MoveUp(float totalSeconds)
        {
            if(PlayerState == PlayerState.Idle || PlayerState == PlayerState.Walk)
                Position.Y -= _playerSpeed * totalSeconds;
        }

        public void MoveDown(float totalSeconds)
        {
            if(PlayerState == PlayerState.Idle || PlayerState == PlayerState.Walk)
                Position.Y += _playerSpeed * totalSeconds;
        }

        public void MoveLeft(float totalSeconds)
        {
            if(PlayerState == PlayerState.Idle || PlayerState == PlayerState.Walk)
                Position.X -= _playerSpeed * totalSeconds;
        }

        public void MoveRight(float totalSeconds)
        {
            if(PlayerState == PlayerState.Idle || PlayerState == PlayerState.Walk)
                Position.X += _playerSpeed * totalSeconds;
        }

        public void MoveToPoint(Vector2 mousePosition, float totalSeconds)
        {
            if(PlayerState == PlayerState.Idle || PlayerState == PlayerState.Walk)
            {
                Vector2 posDelta = mousePosition - Position;
                posDelta.Normalize();
                posDelta = posDelta * (_playerSpeed);
                Position = Position + posDelta  * totalSeconds;
            }
        }

        public void MoveToDirection(Vector2 direction)
        {
            if(PlayerState == PlayerState.Idle || PlayerState == PlayerState.Walk)
                Position = Position + direction*5;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(PlayerState == PlayerState.Walk)
            {
                MoveAnimation.Draw(spriteBatch, Position, _flip);
            }
            else if(PlayerState == PlayerState.Idle)
            {
                IdleAnimation.Draw(spriteBatch, Position, _flip);
            }
            else if(PlayerState == PlayerState.Sleep)
            {
                SleepAnimation.Draw(spriteBatch, Position, _flip);
            }
        }

        public float GetDistance(Entity entity)
        {
            return Vector2.Distance(entity.Position, Position);
        }
    }
}
