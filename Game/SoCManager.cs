using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace conscious
{
    public class SoCManager : IComponent
    {
        private EntityManager _entityManager;
        private Queue<UIThought> _thoughts;
        private float _bgX;
        private float _bgY;
        private float _offsetY;
        private float _offsetX;
        private int _maxThoughts;
        private UIArea _consciousnessBackground;

        public SoCManager(EntityManager entityManager)
        {
            _entityManager = entityManager;
            _thoughts = new Queue<UIThought>();
            _maxThoughts = 2;
            _bgX = 300f; // 150f;
            _bgY = 900f; // 500f;
            _offsetY = 20f;
            _offsetX = 250f;
        }

        public void LoadContent(Texture2D consciousnessBackground)
        {
            Vector2 bgPosition = new Vector2(_bgX, _bgY);
            _consciousnessBackground = new UIArea("SoC Background", consciousnessBackground, bgPosition);
        }

        public void Update(GameTime gameTime){ }

        public void Draw(SpriteBatch spriteBatch){ }

        public void AddThought(UIThought thought)
        {
            if(!_thoughts.Contains(thought))
            {
                if(_thoughts.Count + 1 > _maxThoughts)
                {
                    RemoveThought();
                }
                int thNumber = 0;
                //Update position of thoughts in queue
                foreach(UIThought th in _thoughts)
                {
                    _entityManager.RemoveEntity(th);
                    th.SetPosition(_bgX - _offsetX,
                                   _bgY + thNumber*_offsetY);
                    _entityManager.AddEntity(th);
                    thNumber++;
                }
                // add new thought
                thought.SetPosition(_bgX - _offsetX,
                                    _bgY + thNumber*_offsetY);
                _thoughts.Enqueue(thought);
                _entityManager.AddEntity(thought);
            }
        }

        public void RemoveThought()
        {
            UIThought thought = _thoughts.Dequeue();
            _entityManager.RemoveEntity(thought);
        }

        public void FillEntityManager()
        {
            _entityManager.AddEntity(_consciousnessBackground);
        }
    }
}
