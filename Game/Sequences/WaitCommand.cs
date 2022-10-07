using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace conscious
{
    public class WaitCommand : Command
    {
        private int _timeSinceBeginning;
        private int _millisecondsToWait;
        
        public SoundEffect Sound = null;

        public WaitCommand(int millisecondsToWait) : base()
        {
            _timeSinceBeginning = 0;
            _millisecondsToWait = millisecondsToWait;
        }

        public override void ExecuteCommand(GameTime gameTime, Thing thing)
        {
            if (_timeSinceBeginning == 0)
            {
                Sound.Play();
            }
            _timeSinceBeginning += gameTime.ElapsedGameTime.Milliseconds;
            if (_timeSinceBeginning > _millisecondsToWait)
                CommandFinished = true;
        }

        public override DataHolderCommand GetDataHolderCommand()
        {
            DataHolderWaitCommand dataHolderCommand = new DataHolderWaitCommand();
            dataHolderCommand = (DataHolderWaitCommand)base.GetDataHolderCommand(dataHolderCommand);
            dataHolderCommand.MillisecondsToWait = _millisecondsToWait;
            dataHolderCommand.SoundFilePath = Sound?.Name;
            return dataHolderCommand;
        }
        
        public DataHolderCommand GetDataHolderCommand(DataHolderWaitCommand dataHolderCommand)
        {
            dataHolderCommand = (DataHolderWaitCommand)base.GetDataHolderCommand(dataHolderCommand);
            dataHolderCommand.MillisecondsToWait = _millisecondsToWait;
            dataHolderCommand.SoundFilePath = Sound?.Name;
            return dataHolderCommand;
        }        
    }
}