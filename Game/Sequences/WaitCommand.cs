using Microsoft.Xna.Framework;

namespace conscious
{
    public class WaitCommand : Command
    {
        private int _timeSinceBeginning;
        private int _millisecondsToWait;

        public WaitCommand(int millisecondsToWait) : base()
        {
            _timeSinceBeginning = 0;
            _millisecondsToWait = millisecondsToWait;
        }

        public override void ExecuteCommand(GameTime gameTime, Thing thing)
        {
            _timeSinceBeginning += gameTime.ElapsedGameTime.Milliseconds;
            if (_timeSinceBeginning > _millisecondsToWait)
                CommandFinished = true;

        }
    }
}