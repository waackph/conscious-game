using Microsoft.Xna.Framework;
using System;

namespace conscious
{
    public class WalkCommand : Command
    {
        private Player _player;
        private Vector2 _destinationPoint;

        public WalkCommand(Player player, Vector2 destinationPoint) : base()
        {
            _player = player;
            _destinationPoint = destinationPoint;
        }

        public override void ExecuteCommand(GameTime gameTime)
        {
            float xDist = MathHelper.Distance(_player.Position.X, _destinationPoint.X);
            float yDist = MathHelper.Distance(_player.Position.Y, _destinationPoint.Y);
            if(xDist >= 15f || yDist >= 15f)
            {
                float totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                _player.MoveToPoint(_destinationPoint, totalSeconds);
            }
            else
            {
                CommandFinished = true;
            }
        }
    }
}