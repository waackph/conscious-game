using Microsoft.Xna.Framework;
using System;

namespace conscious
{
    /// <summary>Class <c>AnimateCommand</c> holds data and logic
    /// for the protagonist to say a line.
    /// </summary>
    ///
    public class SayCommand : Command
    {
        public override void ExecuteCommand(GameTime gameTime, Thing thing)
        {
            throw new NotImplementedException();
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