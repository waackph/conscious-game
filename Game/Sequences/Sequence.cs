using Microsoft.Xna.Framework;

using System.Collections.Generic;

namespace conscious
{
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

        public void PlaySequence(GameTime gameTime)
        {
            if(_currentIndex == -1 || _commands[_currentIndex].CommandFinished)
                _currentIndex = _currentIndex + 1;
            if(_currentIndex < _commands.Count)
            {
                _commands[_currentIndex].ExecuteCommand(gameTime);
            }
            else
            {
                SequenceFinished = true;
            }
        }
    }
}