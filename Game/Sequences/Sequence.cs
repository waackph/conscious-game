using Microsoft.Xna.Framework;

using System.Collections.Generic;

namespace conscious
{
    /// <summary>Class <c>Sequence</c> holds data and logic
    /// to play a sequence of commands a character in the Room executes.
    /// </summary>
    ///
    public class Sequence
    {
        private List<Command> _commands;
        private int _currentIndex;
        public bool SequenceFinished;

        public Sequence(List<Command> commands)
        {
            _currentIndex = -1;
            _commands = commands;
            SequenceFinished = false;
        }

        public void PlaySequence(GameTime gameTime, Thing thing)
        {
            if(_currentIndex == -1 || _commands[_currentIndex].CommandFinished)
                _currentIndex = _currentIndex + 1;
            if(_currentIndex < _commands.Count)
            {
                _commands[_currentIndex].ExecuteCommand(gameTime, thing);
            }
            else
            {
                SequenceFinished = true;
            }
        }

        public DataHolderSequence GetDataHolderSequence()
        {
            DataHolderSequence dataHolderSequence = new DataHolderSequence();
            List<DataHolderCommand> dhCommands = new List<DataHolderCommand>();
            foreach(Command command in _commands)
            {
                dhCommands.Add(command.GetDataHolderCommand());
            }
            dataHolderSequence.Commands = dhCommands;
            return dataHolderSequence;
        }
    }
}