#region File Description
//-----------------------------------------------------------------------------
// Vuerbaz.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace conscious
{
    /// <summary>
    /// This is the main class of the Game. Here, the basic settings are set up 
    /// and menu and in-game screen is initialized.
    /// </summary>
    public class Vuerbaz : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private TitleScreen _titleScreen;
        private GameScreen _gameScreen;
        private EndingScreen _endingScreen;
        private Screen _currentScreen;
        private EntityManager _entityManager;
        private MoodStateManager _moodStateManager;
        private AudioManager _audioManager;
        private Texture2D _debugPixel;
        private Texture2D _textBackgroundPixel;
        private Matrix _viewportTransformation;
        private Cursor _cursor;

        RenderTarget2D renderTarget;
        RenderTarget2D lightTarget;
        RenderTarget2D worldTarget;
        List<RenderTarget2D> intermediateTargets;
        // Effect _moodTransitionEffect;


        public Song StreetAmbient;


        public Vuerbaz()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = GlobalData.ScreenWidth;
            _graphics.PreferredBackBufferHeight = GlobalData.ScreenHeight;
            _graphics.IsFullScreen = false;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            System.Console.WriteLine("Screen Size - Width[" + _graphics.PreferredBackBufferWidth + "] Height [" + _graphics.PreferredBackBufferHeight + "]");
        }

        protected override void Initialize()
        {

            // debugging
            _debugPixel = new Texture2D(GraphicsDevice,1,1);
            Color[] colourData = new Color[1];
            colourData[0] = Color.Wheat; //The Colour of the rectangle
            _debugPixel.SetData<Color>(colourData);

            _textBackgroundPixel = new Texture2D(GraphicsDevice,1,1);
            colourData[0] = Color.Black; //The Colour of the rectangle
            _textBackgroundPixel.SetData<Color>(colourData);

            _viewportTransformation = Matrix.CreateTranslation(0, 0, 0);

            // add lighting
            Texture2D lightMap = Content.Load<Texture2D>("light/light_map_default");
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

            // add render target to render scene to texture for post shaders
            renderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            worldTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            lightTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            intermediateTargets = new List<RenderTarget2D>();
            for(int i = 0; i < 3; i++)
            {
                intermediateTargets.Add(
                    new RenderTarget2D(
                        GraphicsDevice,
                        GraphicsDevice.PresentationParameters.BackBufferWidth,
                        GraphicsDevice.PresentationParameters.BackBufferHeight,
                        false,
                        GraphicsDevice.PresentationParameters.BackBufferFormat,
                        DepthFormat.Depth24)
                );
            }
            // Another blendstate to deal with the lightmap later:
            // BlendState LightBlend = new BlendState();
            // LightBlend.ColorBlendFunction = BlendFunction.Subtract;
            // LightBlend.ColorSourceBlend = Blend.DestinationColor;
            // LightBlend.ColorDestinationBlend = Blend.Zero;

            // TODO: Do I need the renderTarget2D? -> Do I maybe need one target for light and one for world?
            // RenderTarget2D GhostLayer = new RenderTarget2D(graphicsDevice, 250, 250);

            List<Texture2D> lights = new List<Texture2D>();
            lights.Add(lightMap);
            _entityManager = new EntityManager(_viewportTransformation, lights, //new List<Texture2D>{lightMap},
                                                // multiplicativeBlend, 
                                                GraphicsDevice, 
                                                worldTarget, lightTarget, renderTarget, intermediateTargets,
                                                Content.Load<Effect>("Effects/light-effect"),
                                                Content.Load<Effect>("Effects/mood-transition-effect"),
                                                Content.Load<Effect>("Effects/depressed-effect"),
                                                Content.Load<Effect>("Effects/manic-effect"),
                                                Content.Load<Effect>("Effects/entity-effect"),
                                                _debugPixel);

            // TODO: Change Transition Texture to something meaningful (also not in moodstatemanager, see to do in that class)
            Texture2D transitionTexture = Content.Load<Texture2D>("light/light_gimp");
            _moodStateManager = new MoodStateManager(_entityManager, Content.Load<SpriteFont>(GlobalData.HudFontName), transitionTexture, _debugPixel);

            _audioManager = new AudioManager();

            _cursor= new Cursor(Content.Load<SpriteFont>(GlobalData.HudFontName),
                                Matrix.Invert(_viewportTransformation),
                                "Cursor",
                                Content.Load<Texture2D>("Cursor/Dot"), 
                                new Vector2(Mouse.GetState().X, Mouse.GetState().Y), 1, 
                                Content.Load<Texture2D>("light/light_cursor"),
                                _textBackgroundPixel);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your C.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _titleScreen = new TitleScreen(new EventHandler(TitleNewEvent), new EventHandler(TitleSaveEvent), 
                                           this, this.GraphicsDevice, this.Content, 
                                           new EventHandler(TitleContinueEvent), 
                                           _entityManager, _moodStateManager, _audioManager);

            _gameScreen = new GameScreen(_graphics.PreferredBackBufferWidth, 
                                        _graphics.PreferredBackBufferHeight,
                                        _debugPixel,
                                        _cursor,
                                        this,
                                        this.GraphicsDevice,
                                        this.Content, 
                                        new EventHandler(GameMenuEvent), 
                                        new EventHandler(GameEndingEvent),
                                        _entityManager, _moodStateManager, _audioManager);
                                        
            _endingScreen = new EndingScreen(_entityManager, _audioManager, _moodStateManager,
                                             this, this.GraphicsDevice, this.Content,
                                             new EventHandler(GameMenuEvent));

            // _moodTransitionEffect = Content.Load<Effect>("Effects/mood-transition-effect");

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

            base.Update(gameTime);
        }


        /// <summary>
        /// Draws the game from background to foreground.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _currentScreen.Draw(_spriteBatch);

            base.Draw(gameTime);
        }

        public void GameMenuEvent(object obj, EventArgs e)
        {
            _entityManager.Clear();
            IsMouseVisible = true;
            _currentScreen = _titleScreen;
            _currentScreen.EnteredScreen = true;
            _entityManager.IsGameScreen = false;
        }

        public void GameEndingEvent(object obj, EventArgs e)
        {
            _entityManager.Clear();
            IsMouseVisible = true;
            _currentScreen = _endingScreen;
            _currentScreen.EnteredScreen = true;
            _entityManager.IsGameScreen = false;
            // Show only new button again to enforce new game start
            _titleScreen.GameLoaded = false;
        }

        public void TitleContinueEvent(object obj, EventArgs e)
        {
            _entityManager.Clear();
            IsMouseVisible = false;
            _currentScreen = _gameScreen;
            _currentScreen.EnteredScreen = true;
            _entityManager.IsGameScreen = true;
            if (!_titleScreen.GameLoaded)
            {
                _gameScreen.LoadGame(newGame: false);
                _titleScreen.GameLoaded = true;
            }
        }

        public void TitleNewEvent(object obj, EventArgs e)
        {
            _entityManager.Clear();
            IsMouseVisible = false;
            _currentScreen = _gameScreen;
            _currentScreen.EnteredScreen = true;
            _entityManager.IsGameScreen = true;
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
