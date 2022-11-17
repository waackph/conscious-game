using Microsoft.Xna.Framework;

namespace conscious
{
    /// <summary>Class <c>WalkCommand</c> holds data and logic
    /// for the protagonist to walk to a given destination point in the room.
    /// </summary>
    ///
    public class WalkCommand : Command
    {
        private float _destinationX;
        private float _destinationY;

        public WalkCommand(float destinationX, float destinationY) : base()
        {
            _destinationX = destinationX;
            _destinationY = destinationY;
        }

        public override void ExecuteCommand(GameTime gameTime, Thing thing)
        {
            Player player = (Player)thing;
            float xDist = MathHelper.Distance(player.Position.X, _destinationX);
            float yDist = MathHelper.Distance(player.Position.Y, _destinationY);
            if(xDist >= 15f || yDist >= 220f) // (xDist >= 15f || yDist >= 15f)
            {
                float totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                player.MoveToPoint(new Vector2(_destinationX, _destinationY), totalSeconds);
            }
            else
            {
                CommandFinished = true;
            }
        }

        public override DataHolderCommand GetDataHolderCommand()
        {
            DataHolderWalkCommand dataHolderCommand = new DataHolderWalkCommand();
            dataHolderCommand = (DataHolderWalkCommand)base.GetDataHolderCommand(dataHolderCommand);
            dataHolderCommand.DestinationX = _destinationX;
            dataHolderCommand.DestinationY = _destinationY;
            return dataHolderCommand;
        }
        
        public DataHolderCommand GetDataHolderCommand(DataHolderWalkCommand dataHolderCommand)
        {
            dataHolderCommand = (DataHolderWalkCommand)base.GetDataHolderCommand(dataHolderCommand);
            dataHolderCommand.DestinationX = _destinationX;
            dataHolderCommand.DestinationY = _destinationY;
            return dataHolderCommand;
        }        

    }
}