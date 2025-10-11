using Microsoft.Xna.Framework;
using System;

namespace conscious
{
    /// <summary>Class <c>VanishCommand</c> holds data and logic
    /// for the protagonist to vanish from or appear to the scene by changing the player state to none.
    /// </summary>
    ///
    public class VanishCommand : Command
    {
        public VanishCommand(int thingId = 0) : base(thingId)
        {
        }

        public override void ExecuteCommand(GameTime gameTime, Thing thing)
        {
            if (GlobalData.IsSameOrSubclass(typeof(Player), thing.GetType()))
            {
                Player player = (Player)thing;
                if (player.PlayerState == PlayerState.None)
                    player.PlayerState = PlayerState.Idle;
                else
                    player.PlayerState = PlayerState.None;
            }
            else if (GlobalData.IsSameOrSubclass(typeof(Thing), thing.GetType()))
            {
                thing.IsActive = !thing.IsActive;
            }
            else
            {
                throw new ArgumentException("Thing is not a Player or Thing");
            }
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