using Microsoft.Xna.Framework;
using System;

namespace conscious
{
    /// <summary>Class <c>AnimateCommand</c> holds data and logic
    /// to execute an animation.
    /// </summary>
    ///
    public class AnimateCommand : Command
    {
        public override void ExecuteCommand(GameTime gameTime, Thing thing)
        {
            throw new NotImplementedException();
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