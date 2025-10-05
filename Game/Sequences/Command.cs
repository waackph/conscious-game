using Microsoft.Xna.Framework;

namespace conscious
{
    /// <summary>Abstract Class <c>Command</c> holds data and logic
    /// to execute a command.
    /// </summary>
    ///
    public abstract class Command
    {
        public bool CommandFinished;
        public int _thingId;
        
        public Command(int thingId = 0)
        {
            _thingId = thingId;
            CommandFinished = false;
        }

        public abstract void ExecuteCommand(GameTime gameTime, Thing thing);

        public virtual DataHolderCommand GetDataHolderCommand()
        {
            DataHolderCommand dataHolderCommand = new DataHolderCommand();
            dataHolderCommand.ThingId = _thingId;
            return dataHolderCommand;
        }

        public virtual DataHolderCommand GetDataHolderCommand(DataHolderCommand dataHolderCommand)
        {
            dataHolderCommand.ThingId = _thingId;
            return dataHolderCommand;
        }
    }
}