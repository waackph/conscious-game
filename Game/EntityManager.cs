using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Linq;


namespace conscious
{
    public class EntityManager : IComponent
    {
        private readonly List<Entity> _entities = new List<Entity>();
        private readonly List<Entity> _entitiesToAdd = new List<Entity>();
        private readonly List<Entity> _entitiesToRemove = new List<Entity>();
        
        public Matrix ViewportTransformation;

        // Lighting
        public Texture2D LightMap;
        private BlendState _multiplicativeBlend;

        public IEnumerable<Entity> Entities => new ReadOnlyCollection<Entity>(_entities);

        // Debugging
        private Texture2D _pixel;
        private bool _debuggingMode;

        public EntityManager(Matrix viewportTransformation, Texture2D lightMap, BlendState multiplicativeBlend, Texture2D pixel)
        {
            _debuggingMode = false;
            _pixel = pixel;
            ViewportTransformation = viewportTransformation;
            LightMap = lightMap;
            _multiplicativeBlend = multiplicativeBlend;
        }

        public void Update(GameTime gameTime){
            foreach(Entity entity in _entities){
                if(_entitiesToRemove.Contains(entity))
                    continue;
                entity.Update(gameTime);
            }

            foreach(Entity entity in _entitiesToAdd){
                _entities.Add(entity);
            }

            foreach(Entity entity in _entitiesToRemove){
                _entities.Remove(entity);
            }

            _entitiesToAdd.Clear();
            _entitiesToRemove.Clear();
        }

        public void Draw(SpriteBatch spriteBatch){
            // TODO: Maybe split up entities into different types to have better control over draw order and batchmode
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null,null, ViewportTransformation);
            foreach(Entity entity in _entities.OrderBy(e => e.DrawOrder))
            {
                if(!entity.FixedDrawPosition)
                {
                    entity.Draw(spriteBatch);
                    if(_debuggingMode && entity.Name != "Background")
                    {
                        spriteBatch.Draw(_pixel, entity.CollisionBox, Color.White);
                    }
                }
                    
            }
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, _multiplicativeBlend);
            spriteBatch.Draw(LightMap, new Rectangle(0, 0, 1920, 1080), Color.White);
            spriteBatch.End();

            spriteBatch.Begin();
            foreach(Entity entity in _entities.OrderBy(e => e.DrawOrder))
            {
                if(entity.FixedDrawPosition)
                {
                    entity.Draw(spriteBatch);
                    if(entity.Name == "moodText")
                        entity.ToString();
                    if(_debuggingMode && entity.Name != "Background")
                    {
                        spriteBatch.Draw(_pixel, entity.CollisionBox, Color.White);
                    }
                }
            }
            spriteBatch.End();
        }

        public void AddEntity(Entity entity){
            if(entity is null)
                throw new ArgumentNullException(nameof(entity), "Null cannot be added as an entity.");
            if(!_entitiesToAdd.Contains(entity))
                _entitiesToAdd.Add(entity);
        }

        public void RemoveEntity(Entity entity){
            if(entity is null)
                throw new ArgumentNullException(nameof(entity), "Null cannot be added as an entity.");

            _entitiesToRemove.Add(entity);
        }

        public void Clear(){
            _entitiesToRemove.AddRange(_entities);
        }

        public IEnumerable<T> GetEntitiesOfType<T>() where T : Entity
        {
            return _entities.OfType<T>();
        }

        public string NumberOfEntities()
        {
            return _entities.Count.ToString();
        }
    }
}