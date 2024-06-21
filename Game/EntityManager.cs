using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Linq;


namespace conscious
{
    /// <summary>Class <c>EntityManager</c> implements a rendering system for all Entities.
    /// </summary>
    ///
    public class EntityManager : IComponent
    {
        private readonly List<Entity> _entities = new List<Entity>();
        private readonly List<Entity> _entitiesToAdd = new List<Entity>();
        private readonly List<Entity> _entitiesToRemove = new List<Entity>();
        
        public Matrix ViewportTransformation;
        private Matrix _mainThoughtUITranslation = Matrix.Identity;

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

            // Draw game entities

            // TODO: Maybe split up entities into different types to have better control over draw order and batchmode
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null,null, ViewportTransformation);
            foreach(Entity entity in _entities.OrderBy(e => e.DrawOrder))
            {
                if(!entity.FixedDrawPosition)
                {
                    entity.Draw(spriteBatch);
                    if(_debuggingMode && entity.Width < 1900)
                    {
                        spriteBatch.Draw(_pixel, entity.CollisionBox, Color.White);
                    }
                }
                    
            }
            spriteBatch.End();

            // Draw lightmaps
            spriteBatch.Begin(SpriteSortMode.Immediate, _multiplicativeBlend);
            spriteBatch.Draw(LightMap, new Rectangle(0, 0, GlobalData.ScreenWidth, GlobalData.ScreenHeight), Color.White);
            spriteBatch.End();

            // Draw Thought UI Background (clipping background for UI Thoughts)
            UIComponent socBackground = GetUIByName("SoC Background");
            RasterizerState initRasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
            if(socBackground != null)
            {
                spriteBatch.Begin();
                socBackground.Draw(spriteBatch);
                if(_debuggingMode)
                {
                    spriteBatch.Draw(_pixel, socBackground.CollisionBox, Color.White);
                }
                spriteBatch.End();

                // Draw UI Thoughts (clipped, therefore set clipping rect options)
                Rectangle initRect = spriteBatch.GraphicsDevice.ScissorRectangle;
                RasterizerState _rasterizerState = new RasterizerState() { ScissorTestEnable = true };
                spriteBatch.GraphicsDevice.ScissorRectangle = socBackground.BoundingBox;

                spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, rasterizerState: _rasterizerState, null, transformMatrix: _mainThoughtUITranslation);
                foreach(UIThought thought in GetEntitiesOfType<UIThought>())
                {
                    if(!thought.IsRootThought)
                        continue;
                    thought.Draw(spriteBatch);
                    if(_debuggingMode)
                    {
                        spriteBatch.Draw(_pixel, thought.CollisionBox, Color.White);
                    }
                }
                spriteBatch.End();

                spriteBatch.GraphicsDevice.ScissorRectangle = initRect;
            }

            // Draw rest of UI
            spriteBatch.Begin(rasterizerState: initRasterizerState);
            foreach(Entity entity in _entities.OrderBy(e => e.DrawOrder))
            {
                if(entity.FixedDrawPosition && entity.Name != "SoC Background")
                {
                    if(GlobalData.IsSameOrSubclass(typeof(UIThought), entity.GetType()))
                    {
                        UIThought tmp = (UIThought)entity;
                        if(tmp.IsRootThought)
                            continue;
                    }
                    entity.Draw(spriteBatch);
                    if(entity.Name == "moodText")
                        entity.ToString();
                    if(_debuggingMode && GlobalData.IsNotBackgroundOrPlayer(entity))
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

        public void RemoveEntities(List<Entity> entities)
        {
            _entitiesToRemove.AddRange(entities);
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
        public Thing GetThingById(int Id)
        {
            foreach(Thing thing in GetEntitiesOfType<Thing>())
            {
                if(thing.Id == Id)
                {
                    return thing;
                }
            }
            return null;
        }
        public UIComponent GetUIByName(string name)
        {
            foreach(UIComponent uiComponent in GetEntitiesOfType<UIComponent>())
            {
                if(uiComponent.Name == name)
                {
                    return uiComponent;
                }
            }
            return null;
        }

        public void SetMainThoughtUITranslation(float scrollOffset)
        {
            _mainThoughtUITranslation = Matrix.CreateTranslation(new Vector3(0, scrollOffset, 0));
        }
    }
}