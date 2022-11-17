using Microsoft.Xna.Framework;

namespace conscious
{
    /// <summary>Class <c>VanishCommand</c> holds data and logic
    /// for the protagonist to vanish from or appear to the scene by changing the player state to none.
    /// </summary>
    ///
    public class VanishCommand : Command
    {
        public override void ExecuteCommand(GameTime gameTime, Thing thing)
        {
            Player player = (Player)thing;
            if(player.PlayerState == PlayerState.None)
                player.PlayerState = PlayerState.Idle;
            else
                player.PlayerState = PlayerState.None;
            CommandFinished = true;
        }

        public override DataHolderCommand GetDataHolderCommand()
        {
            DataHolderVanishCommand dataHolderCommand = new DataHolderVanishCommand();
            dataHolderCommand = (DataHolderVanishCommand)base.GetDataHolderCommand(dataHolderCommand);
            return dataHolderCommand;
        }
        
        public DataHolderCommand GetDataHolderCommand(DataHolderVanishCommand dataHolderCommand)
        {
            dataHolderCommand = (DataHolderVanishCommand)base.GetDataHolderCommand(dataHolderCommand);
            return dataHolderCommand;
        }
    }
}