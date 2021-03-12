using Microsoft.Xna.Framework;

namespace conscious.Sequences
{
    public abstract class Command
    {
        public bool CommandFinished;
        
        public Command()
        {
            CommandFinished = false;
        }

        public abstract void ExecuteCommand(GameTime gameTime);
    }
}