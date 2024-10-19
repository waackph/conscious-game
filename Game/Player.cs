using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace conscious
{
    /// <summary>Class <c>Player</c> implements the basic player mechanics,
    /// such as idle and moving animations and communication with controls class
    /// </summary>
    ///
    public class Player : Thing
    {
        private SpriteEffects _flip;
        private float _currentPositionDelta;
        public bool IsMoving;
        private bool _lastIsMoving;
        private float _playerSpeed;
        private Dictionary<MoodState, AnimatedSprite> _moodIdleAnimation;
        private Dictionary<MoodState, AnimatedSprite> _moodMoveAnimation;
        private Dictionary<MoodState, AnimatedSprite> _moodSleepAnimation;
        
        public PlayerState PlayerState;
        public Vector2 LastPosition;
        public AnimatedSprite IdleAnimation;
        public AnimatedSprite MoveAnimation;
        public AnimatedSprite SleepAnimation;

        public Player(Texture2D moveTexture,
                      Texture2D sleepTexture,
                      Texture2D idleDepressed,
                      Texture2D moveDepressed,
                      int id,
                      ThoughtNode thought,
                      MoodStateManager moodStateManager, 
                      string name, 
                      Texture2D texture,
                      Vector2 position, int drawOrder) 
                      : base(id, thought, moodStateManager, name, texture, position, drawOrder)
        {  
            IdleAnimation = new AnimatedSprite(texture, 1, 1, (Width/2), Height, 0f, 5000);
            MoveAnimation = new AnimatedSprite(moveTexture, 2, 2, (Width/2), Height, 0f, 100);
            SleepAnimation = new AnimatedSprite(sleepTexture, 1, 1, (Width/2), Height, 0f, 5000);

            _flip = SpriteEffects.None;
            _playerSpeed = 300f;
            IsMoving = false;
            PlayerState = PlayerState.Idle; // PlayerState.Sleep;
            _lastIsMoving = IsMoving;
            LastPosition = position;
            DrawOrder = GlobalData.PlayerDrawOrder;

            // Standard case for mood dependent animations
            _moodIdleAnimation = new Dictionary<MoodState, AnimatedSprite>();
            _moodIdleAnimation[MoodState.Depressed] = new AnimatedSprite(idleDepressed, 1, 1, (Width/2), Height, 0f, 5000);
            _moodIdleAnimation[MoodState.Regular] = new AnimatedSprite(texture, 1, 1, (Width/2), Height, 0f, 5000);
            _moodIdleAnimation[MoodState.Manic] = new AnimatedSprite(texture, 1, 1, (Width/2), Height, 0f, 5000);

            _moodMoveAnimation = new Dictionary<MoodState, AnimatedSprite>();
            _moodMoveAnimation[MoodState.Depressed] = new AnimatedSprite(moveDepressed, 2, 2, (Width/2), Height, 0f, 300);
            _moodMoveAnimation[MoodState.Regular] = new AnimatedSprite(moveTexture, 2, 2, (Width/2), Height, 0f, 200);
            _moodMoveAnimation[MoodState.Manic] = new AnimatedSprite(moveTexture, 2, 2, (Width/2), Height, 0f, 150);

            _moodSleepAnimation = new Dictionary<MoodState, AnimatedSprite>();
            _moodSleepAnimation[MoodState.Depressed] = new AnimatedSprite(sleepTexture, 1, 1, (Width/2), Height, 0f, 5000);
            _moodSleepAnimation[MoodState.Regular] = new AnimatedSprite(sleepTexture, 1, 1, (Width/2), Height, 0f, 5000);
            _moodSleepAnimation[MoodState.Manic] = new AnimatedSprite(sleepTexture, 1, 1, (Width/2), Height, 0f, 5000);

            _moodStateManager.MoodChangeEvent += changeAnimationOnMood;
            updateAnimationOnMood(_moodStateManager.moodState);
        }

        private void changeAnimationOnMood(object sender, MoodStateChangeEventArgs e)
        {
            MoodState state = e.CurrentMoodState;
            updateAnimationOnMood(state);
            if(state == MoodState.Depressed)
                _playerSpeed = 150f;
            else if(state == MoodState.Regular)
                _playerSpeed = 350f;
            else if(state == MoodState.Manic)
                _playerSpeed = 400;
        }

        private void updateAnimationOnMood(MoodState moodState)
        {
            IdleAnimation = _moodIdleAnimation[moodState];
            MoveAnimation = _moodMoveAnimation[moodState];
            SleepAnimation = _moodSleepAnimation[moodState];
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
                    IsMoving = false;
                    PlayerState = PlayerState.Idle;
                }
                else
                {
                    MoveAnimation.Update(gameTime);
                    IsMoving = true;
                    PlayerState = PlayerState.Walk;
                }
                if(_lastIsMoving != IsMoving){
                    IdleAnimation.resetAnimation();
                    MoveAnimation.resetAnimation();
                }

                LastPosition = Position;
                _lastIsMoving = IsMoving;
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
    }
}
