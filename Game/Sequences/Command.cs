using Microsoft.Xna.Framework;

namespace conscious
{
    public abstract class Command
    {
        public bool CommandFinished;
        
        public Command()
        {
            CommandFinished = false;
        }

        public abstract void ExecuteCommand(GameTime gameTime, Thing thing);
    }
}