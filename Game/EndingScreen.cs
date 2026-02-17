using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace conscious
{
    /// <summary>Class <c>EndingScreen</c> implements final screen after finishing the game.
    /// </summary>
    ///
    public class EndingScreen : Screen
    {
        private EntityManager _entityManager;
        private AudioManager _audioManager;
        private MoodStateManager _moodStateManager;
        private List<UIComponent> _uiComponents;
        private SpriteFont _displayFont;
        private SpriteFont _buttonFont;
        private Song _endingSong;

        public EndingScreen(EntityManager entityManager, AudioManager audioManager, MoodStateManager moodStateManager, Vuerbaz game, GraphicsDevice graphicsDevice, ContentManager content, EventHandler screenEvent) 
            : base(game, graphicsDevice, content, screenEvent)
        {
            _displayFont = content.Load<SpriteFont>(GlobalData.ThoughtFontName);
            _buttonFont = content.Load<SpriteFont>(GlobalData.MenuFontName);

            _entityManager = entityManager;
            _audioManager = audioManager;
            _moodStateManager = moodStateManager;

            _endingSong = _content.Load<Song>("Audio/Lounge001");

            UIText endingText = new UIText(_displayFont, "Danke, dass du das Spiel durchgespielt hast!", "endingtext", new Texture2D(graphicsDevice, 1, 1), new Vector2(1000, 500), 1, Color.Wheat);
            endingText.Position = new Vector2(endingText.Position.X - endingText.GetStringWidth()/2, endingText.Position.Y - endingText.GetStringHeight()/2);


            UIButton quitButton = new UIButton(new EventHandler(QuitButton_Click),
                                               _displayFont,
                                               "Beenden",
                                               "Quit",
                                               _content.Load<Texture2D>("clear_out/UI/button_background"),
                                               new Vector2(1000, 800), 1);
            quitButton.Position = new Vector2(quitButton.Position.X - quitButton.BoundingBox.Width/2, quitButton.Position.Y - quitButton.BoundingBox.Height/2);

            _uiComponents = new List<UIComponent>()
            {
                quitButton,
                endingText,
            };
        }

        public override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                _screenEvent.Invoke(this, new EventArgs());

            if(EnteredScreen)
            {
                InitilizeEntityManager();
                _audioManager.PlayMusic(_endingSong);
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

        public override void InitilizeEntityManager(){
            foreach(UIComponent uiComponent in _uiComponents)
            {
                _entityManager.AddEntity(uiComponent);
            }

            // Texture2D bg = _content.Load<Texture2D>("Backgrounds/480_270_Room_double_Concept_Draft");
            // Thing background = new Thing(11, null, _moodStateManager, "Background", bg, new Vector2(bg.Width/2, bg.Height/2), 1);
            // _entityManager.AddEntity(background);
        }
    }
}
