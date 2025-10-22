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
        private RoomManager _roomManager;
        public bool SequenceFinished;

        public Sequence(List<Command> commands, RoomManager roomManager = null)
        {
            _currentIndex = -1;
            _roomManager = roomManager;
            _commands = commands;
            SequenceFinished = false;
        }

        public void PlaySequence(GameTime gameTime, Thing initThing)
        {
            if(_currentIndex == -1 || _commands[_currentIndex].CommandFinished)
                _currentIndex = _currentIndex + 1;
            if (_currentIndex < _commands.Count)
            {
                Command command = _commands[_currentIndex];
                Thing thing = initThing;
                if (command._thingId != 0)
                {
                    thing = _roomManager.currentRoom.GetThingInRoom(command._thingId);
                }
                command.ExecuteCommand(gameTime, thing);
            }
            else
            {
                SequenceFinished = true;
                Command command = _commands[_currentIndex-1];
                // Notify scripting API about sequence finished
                EventBus.Publish(this, new SequenceFinishedEvent
                {
                    sequenceCommandThingId = command._thingId,
                });
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