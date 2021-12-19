using Microsoft.Xna.Framework;

namespace conscious
{
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
    }
}