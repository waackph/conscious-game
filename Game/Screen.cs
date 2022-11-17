using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;

namespace conscious
{
    /// <summary>Abstrract class <c>Screen</c> implements the basic screen system.
    /// </summary>
    ///
    public abstract class Screen
    {
        protected EventHandler _screenEvent;
        protected ContentManager _content;
        protected GraphicsDevice _graphicsDevice;
        protected Vuerbaz _game;
        public bool EnteredScreen;

        public Screen(Vuerbaz game, GraphicsDevice graphicsDevice, ContentManager content, EventHandler screenEvent){
            _game = game;
            _graphicsDevice = graphicsDevice;
            _content = content;
            _screenEvent = screenEvent;
            EnteredScreen = false;
        }

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch spriteBatch);

        public abstract void InitilizeEntityManager();
    }
}