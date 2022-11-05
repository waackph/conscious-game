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
using Microsoft.Xna.Framework.Media;

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
        private MoodStateManager _moodStateManager;
        private AudioManager _audioManager;
        private Texture2D _pixel;
        private Matrix _viewportTransformation;
        private Cursor _cursor;

        public Song StreetAmbient;


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

            // add lighting
            Texture2D lightMap = Content.Load<Texture2D>("light/light_gimp_v2");
            // instatiate the blendState
            BlendState multiplicativeBlend = new BlendState();
            // deal with transparency
            multiplicativeBlend.AlphaBlendFunction = BlendFunction.ReverseSubtract;
            multiplicativeBlend.AlphaSourceBlend = Blend.SourceAlpha;
            multiplicativeBlend.AlphaDestinationBlend = Blend.Zero;
            // deal with color
            multiplicativeBlend.ColorBlendFunction = BlendFunction.Add;
            multiplicativeBlend.ColorSourceBlend = Blend.DestinationColor;
            multiplicativeBlend.ColorDestinationBlend = Blend.Zero;

            // Another blendstate to deal with the lightmap later:
            // BlendState LightBlend = new BlendState();
            // LightBlend.ColorBlendFunction = BlendFunction.Subtract;
            // LightBlend.ColorSourceBlend = Blend.DestinationColor;
            // LightBlend.ColorDestinationBlend = Blend.Zero;

            // TODO: Do I need the renderTarget2D? -> Do I maybe need one target for light and one for world?
            // RenderTarget2D GhostLayer = new RenderTarget2D(graphicsDevice, 250, 250);

            _entityManager = new EntityManager(_viewportTransformation, lightMap, multiplicativeBlend, _pixel);

            // TODO: Change Transition Texture to something meaningful (also not in moodstatemanager, see to do in that class)
            Texture2D transitionTexture = Content.Load<Texture2D>("light/light_gimp");
            _moodStateManager = new MoodStateManager(_entityManager, Content.Load<SpriteFont>("Font/Hud"), transitionTexture, _pixel);

            _audioManager = new AudioManager();

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
                                           new EventHandler(TitleContinueEvent), 
                                           _entityManager, _moodStateManager, _audioManager);

            _gameScreen = new GameScreen(_graphics.PreferredBackBufferWidth, 
                                        _graphics.PreferredBackBufferHeight,
                                        _pixel,
                                        _cursor,
                                        this,
                                        this.GraphicsDevice,
                                        this.Content, 
                                        new EventHandler(GameMenuEvent), 
                                        _entityManager, _moodStateManager, _audioManager);
            
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
            _gameScreen.LoadGame(newGame:true);
            _titleScreen.GameLoaded = true;
        }
        
        public void TitleSaveEvent(object obj, EventArgs e)
        {
            if(_titleScreen.GameLoaded)
                _gameScreen.SaveGame();
        }
    }
}
