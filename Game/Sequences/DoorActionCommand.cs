using Microsoft.Xna.Framework;

namespace conscious
{
    /// <summary>Class <c>DoorActionCommand</c> holds data and logic
    /// for the protagonist to interact with a door as a command.
    /// </summary>
    ///
    public class DoorActionCommand : Command
    {
        private int _doorId;
        private EntityManager _entityManager;

        public DoorActionCommand(EntityManager entityManager, int doorId) : base()
        {
            _entityManager = entityManager;
            _doorId = doorId;
        }

        public override void ExecuteCommand(GameTime gameTime, Thing thing)
        {
            // We need to do that at runtime on execution because otherwise the Door is an object that references itself, 
            // since the door has a thought, and the finalThoughtLink has a Sequence with this particular DoorActionCommand 
            // which again would contain the door. 
            // Using the Id at initilization we still have the problem that the entity manager 
            // is not yet initilized and filled with all the Entities.
            // TODO: Maybe we need another way of initilizing everything...
            Door door = (Door)_entityManager.GetThingById(_doorId);
            if(door.IsClosed)
                door.OpenDoor();
            else
                door.CloseDoor();
            CommandFinished = true;
        }

        public override DataHolderCommand GetDataHolderCommand()
        {
            DataHolderDoorActionCommand dataHolderCommand = new DataHolderDoorActionCommand();
            dataHolderCommand = (DataHolderDoorActionCommand)base.GetDataHolderCommand(dataHolderCommand);
            dataHolderCommand.DoorId = _doorId;
            return dataHolderCommand;
        }
        
        public DataHolderCommand GetDataHolderCommand(DataHolderDoorActionCommand dataHolderCommand)
        {
            dataHolderCommand = (DataHolderDoorActionCommand)base.GetDataHolderCommand(dataHolderCommand);
            dataHolderCommand.DoorId = _doorId;
            return dataHolderCommand;
        }
    }
}