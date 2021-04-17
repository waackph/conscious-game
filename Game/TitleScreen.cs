using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace conscious
{
    public class TitleScreen : Screen
    {
        // fields
        private EntityManager _entityManager;
        private List<UIComponent> _uiComponents;

        // properties
        private SpriteFont _displayFont;

        public bool GameLoaded { get; set; }
        private EventHandler _newEvent;
        private EventHandler _saveEvent;

        public TitleScreen(EventHandler newEvent, EventHandler saveEvent, Vuerbaz game, GraphicsDevice graphicsDevice, ContentManager content, EventHandler screenEvent, EntityManager entityManager) 
            : base(game, graphicsDevice, content, screenEvent)
        {
            // Initilize
            _displayFont = content.Load<SpriteFont>("Font/Hud");

            _entityManager = entityManager;

            GameLoaded = false;

            _newEvent = newEvent;
            _saveEvent = saveEvent;

            UIButton continueButton = new UIButton(new EventHandler(ContinueButton_Click),
                                                   _displayFont,
                                                   "Continue",
                                                   "Continue",
                                                   _content.Load<Texture2D>("UI/debug_sprites/ui_button"),
                                                   new Vector2(350, 200));

            UIButton newButton = new UIButton(new EventHandler(NewButton_Click),
                                                   _displayFont,
                                                   "New",
                                                   "New",
                                                   _content.Load<Texture2D>("UI/debug_sprites/ui_button"),
                                                   new Vector2(350, 250));

            UIButton saveButton = new UIButton(new EventHandler(SaveButton_Click),
                                                   _displayFont,
                                                   "Save",
                                                   "Save",
                                                   _content.Load<Texture2D>("UI/debug_sprites/ui_button"),
                                                   new Vector2(350, 300));

            UIButton quitButton = new UIButton(new EventHandler(QuitButton_Click),
                                               _displayFont,
                                               "Quit",
                                               "Quit",
                                               _content.Load<Texture2D>("UI/debug_sprites/ui_button"),
                                               new Vector2(350, 350));

            _uiComponents = new List<UIComponent>()
            {
                continueButton,
                newButton,
                saveButton,
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

        public override void InitilizeEntityManager(){
            foreach(UIComponent uiComponent in _uiComponents)
            {
                _entityManager.AddEntity(uiComponent);
            }
        }
    }
}