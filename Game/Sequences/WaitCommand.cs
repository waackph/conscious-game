using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace conscious
{
    /// <summary>Class <c>VanishCommand</c> holds data and logic
    /// for the protagonist to wait for a given amount of seconds.
    /// </summary>
    ///
    public class WaitCommand : Command
    {
        private int _timeSinceBeginning;
        private int _millisecondsToWait;
        
        public SoundEffect Sound = null;

        public WaitCommand(int millisecondsToWait, int thingId = 0) : base(thingId)
        {
            _timeSinceBeginning = 0;
            _millisecondsToWait = millisecondsToWait;
            _thingId = thingId;
        }

        public override void ExecuteCommand(GameTime gameTime, Thing thing)
        {
            if (Sound != null && _timeSinceBeginning == 0)
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
            dataHolderCommand.CmdSoundFilePath = Sound?.Name;
            return dataHolderCommand;
        }
        
        public DataHolderCommand GetDataHolderCommand(DataHolderWaitCommand dataHolderCommand)
        {
            dataHolderCommand = (DataHolderWaitCommand)base.GetDataHolderCommand(dataHolderCommand);
            dataHolderCommand.MillisecondsToWait = _millisecondsToWait;
            dataHolderCommand.CmdSoundFilePath = Sound?.Name;
            return dataHolderCommand;
        }        
    }
}