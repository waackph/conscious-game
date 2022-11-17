using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    /// <summary>Interface <c>IComponent</c> is the basic class for a class that 
    /// implements MonoGame Update and Draw methods.
    /// </summary>
    ///
    public interface IComponent
    {
        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch);
    }
}