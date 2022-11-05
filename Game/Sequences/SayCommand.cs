using Microsoft.Xna.Framework;

namespace conscious
{
    public class SayCommand : Command
    {
        public override void ExecuteCommand(GameTime gameTime, Thing thing)
        {

        }
        public override DataHolderCommand GetDataHolderCommand()
        {
            DataHolderSayCommand dataHolderCommand = new DataHolderSayCommand();
            dataHolderCommand = (DataHolderSayCommand)base.GetDataHolderCommand(dataHolderCommand);
            return dataHolderCommand;
        }
        
        public DataHolderCommand GetDataHolderCommand(DataHolderSayCommand dataHolderCommand)
        {
            dataHolderCommand = (DataHolderSayCommand)base.GetDataHolderCommand(dataHolderCommand);
            return dataHolderCommand;
        }        
    }
}