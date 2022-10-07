using Microsoft.Xna.Framework;

namespace conscious
{
    public class AnimateCommand : Command
    {
        public override void ExecuteCommand(GameTime gameTime, Thing thing)
        {

        }
        public override DataHolderCommand GetDataHolderCommand()
        {
            DataHolderAnimateCommand dataHolderCommand = new DataHolderAnimateCommand();
            dataHolderCommand = (DataHolderAnimateCommand)base.GetDataHolderCommand(dataHolderCommand);
            return dataHolderCommand;
        }
        
        public DataHolderCommand GetDataHolderCommand(DataHolderAnimateCommand dataHolderCommand)
        {
            dataHolderCommand = (DataHolderAnimateCommand)base.GetDataHolderCommand(dataHolderCommand);
            return dataHolderCommand;
        }

    }
}