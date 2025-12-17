using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace conscious
{
    /// <summary>Class <c>TitleScreen</c> implements a menu system.
    /// It manages menu functionality and UI elements that trigger the functionality.
    /// </summary>
    ///
    public class TitleScreen : Screen
    {
        // fields
        private EntityManager _entityManager;
        private MoodStateManager _moodStateManager;
        private AudioManager _audioManager;
        private List<UIComponent> _uiComponents;

        // properties
        private SpriteFont _displayFont;

        private Song _titleSong;

        public bool GameLoaded { get; set; }
        private EventHandler _newEvent;
        private EventHandler _saveEvent;

        public TitleScreen(EventHandler newEvent, EventHandler saveEvent, Vuerbaz game, GraphicsDevice graphicsDevice, ContentManager content, EventHandler screenEvent, EntityManager entityManager, MoodStateManager moodStateManager, AudioManager audioManager) 
            : base(game, graphicsDevice, content, screenEvent)
        {
            // Initilize
            _displayFont = content.Load<SpriteFont>(GlobalData.MenuFontName);

            _entityManager = entityManager;
            _moodStateManager = moodStateManager;
            _audioManager = audioManager;

            _titleSong = _content.Load<Song>("Audio/wind-and-seaguls");

            GameLoaded = false;

            _newEvent = newEvent;
            _saveEvent = saveEvent;

            UIButton continueButton = new UIButton(new EventHandler(ContinueButton_Click),
                                                   _displayFont,
                                                   "Continue",
                                                   "Continue",
                                                   _content.Load<Texture2D>("clear_out/UI/button_background"),
                                                   new Vector2(350, 200), 1);

            UIButton newButton = new UIButton(new EventHandler(NewButton_Click),
                                                   _displayFont,
                                                   "New",
                                                   "New",
                                                   _content.Load<Texture2D>("clear_out/UI/button_background"),
                                                   new Vector2(350, 250), 1);

            // UIButton saveButton = new UIButton(new EventHandler(SaveButton_Click),
            //                                        _displayFont,
            //                                        "Save",
            //                                        "Save",
            //                                        _content.Load<Texture2D>("clear_out/UI/button_background"),
            //                                        new Vector2(350, 300), 1);

            UIButton quitButton = new UIButton(new EventHandler(QuitButton_Click),
                                               _displayFont,
                                               "Quit",
                                               "Quit",
                                               _content.Load<Texture2D>("clear_out/UI/button_background"),
                                               new Vector2(350, 300), 1);

            _uiComponents = new List<UIComponent>()
            {
                continueButton,
                newButton,
                // saveButton,
                quitButton
            };
        }

        public override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                _game.Exit();

            if(EnteredScreen)
            {
                InitilizeEntityManager();
                _audioManager.PlayMusic(_titleSong);
                EnteredScreen = false;
            }
            _entityManager.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _entityManager.Draw(spriteBatch);
        }

        private void QuitButton_Click(object sender, System.EventArgs e)
        {
            _game.Exit();
        }

        private void ContinueButton_Click(object sender, System.EventArgs e)
        {
            _screenEvent?.Invoke(this, new EventArgs());
        }

        private void NewButton_Click(object sender, System.EventArgs e)
        {
            _newEvent?.Invoke(this, new EventArgs());
        }

        private void SaveButton_Click(object sender, System.EventArgs e)
        {
            _saveEvent?.Invoke(this, new EventArgs());
        }

        public override void InitilizeEntityManager()
        {
            foreach (UIComponent uiComponent in _uiComponents)
            {
                // Show continue button only if game has already started
                if (uiComponent.Name == "Continue" && !GameLoaded)
                {
                    continue;
                }
                _entityManager.AddEntity(uiComponent);
            }

            // Add background
            Texture2D bg = _content.Load<Texture2D>("clear_out/menu/ocean_background");
            Thing background = new Thing(11, null, _moodStateManager, "Background", bg, new Vector2(bg.Width / 2, bg.Height / 2), 1);
            _entityManager.AddEntity(background);

            // Add ship
            Texture2D shipTexture = _content.Load<Texture2D>("clear_out/menu/ship_cut_small");
            AnimatedThing ship = new AnimatedThing(14238842, "Ship", null, _moodStateManager, shipTexture, new Vector2(450, 75), 2, 20, 1, 1, 100000, 0.75f);
            _entityManager.AddEntity(ship);

            // Add seagull
            Texture2D seagullTexture = _content.Load<Texture2D>("clear_out/menu/bird_animation_atlas");
            AnimatedThing seagull = new AnimatedThing(149232, "Seagull", null, _moodStateManager, seagullTexture, new Vector2(-100, 50), 2, 75, 1, 4, 200, 0.7f);
            _entityManager.AddEntity(seagull);

            Texture2D fg = _content.Load<Texture2D>("clear_out/menu/house_foreground");
            Thing foreground = new Thing(12, null, _moodStateManager, "Foreground", fg, new Vector2(fg.Width / 2, fg.Height / 2 - 1), 3);
            _entityManager.AddEntity(foreground);

            // Add swing
            Texture2D swingTexture = _content.Load<Texture2D>("clear_out/menu/swing_animation_atlas");
            AnimatedThing swing = new AnimatedThing(152123, "Swing", null, _moodStateManager, swingTexture, new Vector2(1735, 825), 4, 0, 1, 4, 1200);
            _entityManager.AddEntity(swing);

            Texture2D tumor = _content.Load<Texture2D>("clear_out/menu/tumor3_outlined");
            Thing tumorThing = new Thing(12, null, _moodStateManager, "tumorThing", tumor, new Vector2(958, 980), 4);
            _entityManager.AddEntity(tumorThing);
        }
    }
}
