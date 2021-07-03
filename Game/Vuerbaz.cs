#region File Description
//-----------------------------------------------------------------------------
// Vuerbaz.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace conscious
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Vuerbaz : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private TitleScreen _titleScreen;
        private GameScreen _gameScreen;
        private Screen _currentScreen;
        private EntityManager _entityManager;
        private Texture2D _pixel;
        private Matrix _viewportTransformation;
        private Cursor _cursor;


        public Vuerbaz()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            System.Console.WriteLine("Screen Size - Width[" + _graphics.PreferredBackBufferWidth + "] Height [" + _graphics.PreferredBackBufferHeight + "]");
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // debugging
            _pixel = new Texture2D(GraphicsDevice,1,1);
            Color[] colourData = new Color[1];
            colourData[0] = Color.Wheat; //The Colour of the rectangle
            _pixel.SetData<Color>(colourData);

            _viewportTransformation = Matrix.CreateTranslation(0, 0, 0);

            _entityManager = new EntityManager(_viewportTransformation, _pixel);

            _cursor= new Cursor(Content.Load<SpriteFont>("Font/Hud"),
                                Matrix.Invert(_viewportTransformation),
                                "Cursor",
                                Content.Load<Texture2D>("Cursor/Gem"), 
                                new Vector2(Mouse.GetState().X, Mouse.GetState().Y));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your C.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _titleScreen = new TitleScreen(new EventHandler(TitleNewEvent), new EventHandler(TitleSaveEvent), 
                                           this, this.GraphicsDevice, this.Content, 
                                           new EventHandler(TitleContinueEvent), _entityManager);

            _gameScreen = new GameScreen(_graphics.PreferredBackBufferWidth, 
                                        _graphics.PreferredBackBufferHeight,
                                        _pixel,
                                        _cursor,
                                        this,
                                        this.GraphicsDevice,
                                        this.Content, 
                                        new EventHandler(GameMenuEvent), _entityManager);
            
            _currentScreen = _titleScreen;
            _currentScreen.EnteredScreen = true;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            _currentScreen.Update(gameTime);

            // TODO: Add your update logic here
            base.Update(gameTime);
        }


        /// <summary>
        /// Draws the game from background to foreground.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _currentScreen.Draw(_spriteBatch);

            base.Draw(gameTime);
        }

        public void GameMenuEvent(object obj, EventArgs e){
            _entityManager.Clear();
            IsMouseVisible = true;
            _currentScreen = _titleScreen;
            _currentScreen.EnteredScreen = true;
        }

        public void TitleContinueEvent(object obj, EventArgs e){
            _entityManager.Clear();
            IsMouseVisible = false;
            _currentScreen = _gameScreen;
            _currentScreen.EnteredScreen = true;
            if(!_titleScreen.GameLoaded)
            {
                _gameScreen.LoadGame(newGame:false);
                _titleScreen.GameLoaded = true;
            }
        }

        public void TitleNewEvent(object obj, EventArgs e)
        {
            _entityManager.Clear();
            IsMouseVisible = false;
            _currentScreen = _gameScreen;
            _currentScreen.EnteredScreen = true;
            // _gameScreen.LoadGame(newGame:true);
            _titleScreen.GameLoaded = true;
        }
        
        public void TitleSaveEvent(object obj, EventArgs e)
        {
            if(_titleScreen.GameLoaded)
                _gameScreen.SaveGame();
        }
    }
}
