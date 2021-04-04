using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace conscious
{
    public class VerbManager : IComponent
    {
        private float _margin = 5f;
        private float _startWidth = 150f;
        private float _startHeight = 1050f;
        private EntityManager _entityManager;
        private List<UIVerb> _verbs;

        public VerbManager(EntityManager entityManager)
        {
            _entityManager = entityManager;
            _verbs = new List<UIVerb>();
        }

        public void LoadContent(Texture2D verbBackground, Texture2D examineTexture, 
                                Texture2D pickUpTexture, Texture2D useTexture,
                                Texture2D combineTexture, Texture2D talkToTexture,
                                Texture2D giveToTexture)
        {
            // Examine
            int iWidth = 0;
            int iHeight = 2;
            Vector2 verbPosition = new Vector2(_startWidth+verbBackground.Width*iWidth+_margin*iWidth, 
                                               _startHeight-verbBackground.Height*iHeight-_margin*iHeight);
            UIVerb action = new UIVerb(Verb.Examine, examineTexture, "Examine", verbBackground, verbPosition);
            action.UpdateDrawOrder(2);
            _verbs.Add(action);

            // Pickup
            iWidth = 1;
            iHeight = 2;
            verbPosition = new Vector2(_startWidth+verbBackground.Width*iWidth+_margin*iWidth, 
                                       _startHeight-verbBackground.Height*iHeight-_margin*iHeight);
            action = new UIVerb(Verb.PickUp, pickUpTexture, "Pick up", verbBackground, verbPosition);
            action.UpdateDrawOrder(2);
            _verbs.Add(action);

            // Use
            iWidth = 2;
            iHeight = 2;
            verbPosition = new Vector2(_startWidth+verbBackground.Width*iWidth+_margin*iWidth, 
                                       _startHeight-verbBackground.Height*iHeight-_margin*iHeight);
            action = new UIVerb(Verb.Use, useTexture, "Use", verbBackground, verbPosition);
            action.UpdateDrawOrder(2);
            _verbs.Add(action);

            // Combine
            iWidth = 0;
            iHeight = 1;
            verbPosition = new Vector2(_startWidth+verbBackground.Width*iWidth+_margin*iWidth, 
                                       _startHeight-verbBackground.Height*iHeight-_margin*iHeight);
            action = new UIVerb(Verb.Combine, combineTexture, "Combine", verbBackground, verbPosition);
            action.UpdateDrawOrder(2);
            _verbs.Add(action);

            // Talk to
            iWidth = 1;
            iHeight = 1;
            verbPosition = new Vector2(_startWidth+verbBackground.Width*iWidth+_margin*iWidth, 
                                       _startHeight-verbBackground.Height*iHeight-_margin*iHeight);
            action = new UIVerb(Verb.TalkTo, talkToTexture, "Talk to", verbBackground, verbPosition);
            action.UpdateDrawOrder(2);
            _verbs.Add(action);

            // Give to
            iWidth = 2;
            iHeight = 1;
            verbPosition = new Vector2(_startWidth+verbBackground.Width*iWidth+_margin*iWidth, 
                                       _startHeight-verbBackground.Height*iHeight-_margin*iHeight);
            action = new UIVerb(Verb.Give, giveToTexture, "Give", verbBackground, verbPosition);
            action.UpdateDrawOrder(2);
            _verbs.Add(action);
        }

        public virtual void Update(GameTime gameTime){ }

        public virtual void Draw(SpriteBatch spriteBatch){ }

        public void FillEntityManager()
        {
            foreach(UIVerb verb in _verbs)
            {
                _entityManager.AddEntity(verb);
            }
        }
    }
}