using Microsoft.Xna.Framework;
using System;

namespace conscious
{
    /// <summary>Class <c>AnimateCommand</c> holds data and logic
    /// to execute an animation.
    /// </summary>
    ///
    public class AnimateCommand : Command
    {
        Vector2 _startPosition;
        PlayerState _animState;
        float _scaleSize = 1f;
        bool _isAnimationFinished = false;

        public AnimateCommand(Vector2 startPosition, PlayerState animState, float scaleSize) : base()
        {
            _startPosition = startPosition;
            _animState = animState;
            _scaleSize = scaleSize;
        }

        public override void ExecuteCommand(GameTime gameTime, Thing thing)
        {

            if (_startPosition != Vector2.Zero)
                thing.Position = _startPosition;

            if (GlobalData.IsSameOrSubclass(typeof(Player), thing.GetType()))
            {
                Player player = (Player)thing;
                if (_animState == PlayerState.Sleep && player.PlayerState != PlayerState.Sleep)
                    player.GoToSleep();
                else if (_animState == PlayerState.Sleep && player.PlayerState == PlayerState.Sleep)
                    player.WakeUp();
                _isAnimationFinished = true;
            }
            else if (GlobalData.IsSameOrSubclass(typeof(Entity), thing.GetType()))
            {
                Entity entity = (Entity)thing;
                entity.Position = _startPosition;
                if (_animState == PlayerState.Scale)
                {
                    float totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    entity.Scale = Lerp(entity.Scale, _scaleSize, 0.1f, totalSeconds);
                    if(entity.Scale >= _scaleSize - 0.01f)
                    {
                        // entity.Scale = _scaleSize;
                        _isAnimationFinished = true;
                    }
                }
            }
            else
            {
                throw new ArgumentException("Thing is not a Player or Entity");
            }
            
            if(_isAnimationFinished == true)
                CommandFinished = true;
        }

        private float Lerp(float start, float end, float amount, float timedelta)
        {
            return start + ((end - start) * amount * timedelta);
        }

        public override DataHolderCommand GetDataHolderCommand()
        {
            DataHolderAnimateCommand dataHolderCommand = new DataHolderAnimateCommand();
            dataHolderCommand = (DataHolderAnimateCommand)base.GetDataHolderCommand(dataHolderCommand);
            dataHolderCommand.StartPositionX = _startPosition.X;
            dataHolderCommand.StartPositionY = _startPosition.Y;
            dataHolderCommand.AnimState = _animState.ToString();
            dataHolderCommand.ScaleSize = _scaleSize;
            return dataHolderCommand;
        }
        
        public DataHolderCommand GetDataHolderCommand(DataHolderAnimateCommand dataHolderCommand)
        {
            dataHolderCommand = (DataHolderAnimateCommand)base.GetDataHolderCommand(dataHolderCommand);
            dataHolderCommand.StartPositionX = _startPosition.X;
            dataHolderCommand.StartPositionY = _startPosition.Y;
            dataHolderCommand.AnimState = _animState.ToString();
            dataHolderCommand.ScaleSize = _scaleSize;
            return dataHolderCommand;
        }

    }
}