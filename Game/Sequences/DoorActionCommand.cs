using Microsoft.Xna.Framework;

namespace conscious
{
    public class DoorActionCommand : Command
    {
        private Door _door;

        public DoorActionCommand(Door door) : base()
        {
            _door = door;
        }

        public override void ExecuteCommand(GameTime gameTime, Thing thing)
        {
            if(_door.IsClosed)
                _door.OpenDoor();
            else
                _door.CloseDoor();
            CommandFinished = true;
        }
    }
}