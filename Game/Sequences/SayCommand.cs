using Microsoft.Xna.Framework;
using System;

namespace conscious
{
    /// <summary>Class <c>AnimateCommand</c> holds data and logic
    /// for the protagonist to say a line.
    /// </summary>

    public class SayCommand : Command
    {
        private SoCManager _socManager;
        private string _thoughtText;
;
        public SayCommand(SoCManager socManager, string thoughtText, int thingId = 0) : base(thingId)
        {
            _socManager = socManager;
            _thoughtText = thoughtText;
        }

        public override void ExecuteCommand(GameTime gameTime, Thing thing)
        {
            ThoughtNode thoughtNode = new ThoughtNode(124498, _thoughtText, 0, true, 0);
            _socManager.AddThought(thoughtNode);
            CommandFinished = true;
        }

        public override DataHolderCommand GetDataHolderCommand()
        {
            DataHolderSayCommand dataHolderCommand = new DataHolderSayCommand();
            dataHolderCommand = (DataHolderSayCommand)base.GetDataHolderCommand(dataHolderCommand);
            dataHolderCommand.ThoughtText = _thoughtText;
            return dataHolderCommand;
        }

        public DataHolderCommand GetDataHolderCommand(DataHolderSayCommand dataHolderCommand)
        {
            dataHolderCommand = (DataHolderSayCommand)base.GetDataHolderCommand(dataHolderCommand);
            dataHolderCommand.ThoughtText = _thoughtText;
            return dataHolderCommand;
        }
    }
}