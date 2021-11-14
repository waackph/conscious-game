using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace conscious
{
    public class WalkCommand : Command
    {
        [JsonProperty]
        private float _destinationX;
        [JsonProperty]
        private float _destinationY;

        public WalkCommand(float destinationX, float destinationY) : base()
        {
            _destinationX = destinationX;
            _destinationY = destinationY;
        }

        public override void ExecuteCommand(GameTime gameTime, Thing thing)
        {
            Player player = (Player)thing;
            float xDist = MathHelper.Distance(player.Position.X, _destinationX);
            float yDist = MathHelper.Distance(player.Position.Y, _destinationY);
            if(xDist >= 15f || yDist >= 220f) // (xDist >= 15f || yDist >= 15f)
            {
                float totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                player.MoveToPoint(new Vector2(_destinationX, _destinationY), totalSeconds);
            }
            else
            {
                CommandFinished = true;
            }
        }
    }
}