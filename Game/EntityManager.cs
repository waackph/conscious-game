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
        
        public Matrix ViewTransform;
        private Matrix _mainThoughtUITranslation = Matrix.Identity;

        // Lighting
        // public Texture2D LightMap;
        public List<Texture2D> Lights;
        // private BlendState _multiplicativeBlend;

        // Effects
        GraphicsDevice _graphicsDevice;
        private Effect _lightShader;
        private Effect _moodTransitionEffect;
        private Effect _depressedEffect;
        private Effect _manicEffect;
        private Effect _entityEffect;
        RenderTarget2D _worldTarget;
        RenderTarget2D _lightTarget;
        RenderTarget2D _renderTarget;
        List<RenderTarget2D> _intermediateTargets;
        float currentTime = 0f;

        public bool FlashlightOn { get; set; }
        public bool doTransition = false;
        public MoodState newMood = MoodState.None;
        bool isDepressed = false;
        bool isManic = false;

        bool useTransition = true;
        bool maxNoiseReached = false;
        bool transitionStarted = false;
        bool newRound = false;
        float nLast = 0f;

        public IEnumerable<Entity> Entities => new ReadOnlyCollection<Entity>(_entities);

        // Debugging
        private Texture2D _pixel;
        private bool _debuggingMode;

        public EntityManager(Matrix viewportTransformation, 
                             List<Texture2D> lights, 
                            //  BlendState multiplicativeBlend, 
                             GraphicsDevice graphicsDevice, 
                             RenderTarget2D worldTarget,
                             RenderTarget2D lightTarget,
                             RenderTarget2D renderTarget,
                             List<RenderTarget2D> intermediateTargets,
                             Effect lightShader,
                             Effect moodTransitionEffect,
                             Effect depressedEffect,
                             Effect manicEffect,
                             Effect entityEffect,
                             Texture2D pixel)
        {
            FlashlightOn = false;
            _debuggingMode = false;
            _pixel = pixel;
            ViewTransform = viewportTransformation;
            Lights = lights;
            // _multiplicativeBlend = multiplicativeBlend;
            _graphicsDevice = graphicsDevice;
            _worldTarget = worldTarget;
            _lightTarget = lightTarget;
            _renderTarget = renderTarget;
            _intermediateTargets = intermediateTargets;
            _moodTransitionEffect = moodTransitionEffect;
            _lightShader = lightShader;
            _depressedEffect = depressedEffect;
            _manicEffect = manicEffect;
            _entityEffect = entityEffect;
        }

        public void Update(GameTime gameTime){
            // set timer
            if(doTransition)
                currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds; //Time passed since last Update()

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

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the world with all its entities to a render target texture
            DrawGameWorldToTexture(_worldTarget, spriteBatch);
            
            // TODO: Bug - if there is no light on start screen and later there is light, the screen is black
            bool hasLights = DrawLightMap(_lightTarget, spriteBatch);
            // Console.WriteLine("Draw lights: " + hasLights.ToString());
            if(hasLights)
                CombineWorldWithLight(_worldTarget, _lightTarget, _renderTarget, spriteBatch);
            else
                _renderTarget = _worldTarget;

            List<Effect> effects = DecideEffects();

            // // Apply post processing shaders
            _graphicsDevice.Clear(Color.Black);

            // // use list of render targets instead
            List<RenderTarget2D> targets = new List<RenderTarget2D>();
            targets.Add(_renderTarget);

            int i = 0;
            foreach(Effect effect in effects)
            {
                targets.Add(_intermediateTargets[i]);
                DrawShaderEffect(spriteBatch, effect, targets[i+1], targets[i]);
                i += 1;
            }

            DrawShaderEffect(spriteBatch, null, null, targets[i]);

            // Draw UI that should be not affected by post shaders
            DrawUI(spriteBatch);
        }

        void DrawShaderEffect(SpriteBatch spriteBatch, Effect effect, RenderTarget2D targetToUse, RenderTarget2D targetToDraw)
        {
            if(targetToUse != null)
            {
                _graphicsDevice.SetRenderTarget(targetToUse);
                _graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
                _graphicsDevice.Clear(Color.Black);
            }

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, 
                    SamplerState.LinearClamp, DepthStencilState.Default, 
                    RasterizerState.CullNone, effect);
            spriteBatch.Draw(targetToDraw, 
                new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height), 
                Color.White);
            spriteBatch.End();

            if(targetToUse != null)
            {
                _graphicsDevice.SetRenderTarget(null);
            }
        }

        List<Effect> DecideEffects()
        {
            List<Effect> effects = new List<Effect>();

            if(isDepressed && !doTransition)
            {
                // _depressedEffect.Parameters["Brightness"].SetValue(1f);
                // _depressedEffect.Parameters["AmbientColor"].SetValue(Color.White.ToVector4());
                // _depressedEffect.Parameters["AmbientIntensity"].SetValue(0.5f);
                _depressedEffect.Parameters["UniformSat"].SetValue(-0.5f);
                _depressedEffect.Parameters["UniformVal"].SetValue(-0.0f);
                effects.Add(_depressedEffect);
            }

            if(isManic && !doTransition)
            {
                // _manicEffect.Parameters["BrightThreshold"].SetValue(1f);
                _manicEffect.Parameters["BlurDistance"].SetValue(0.001f);
                effects.Add(_manicEffect);
            }

            if(doTransition)
            {
                // DistortionTransition();
                if(isDepressed && newMood == MoodState.Regular)
                    SaturationTransition(-0.5f, 0f, saturateDown: false);
                else if(!isDepressed && !isManic && newMood == MoodState.Depressed)
                    SaturationTransition(-0.5f, 0f, saturateDown: true);

                // if transition still active
                if(doTransition)
                    effects.Add(_moodTransitionEffect);
            }

            return effects;
        }

        void SaturationTransition(float nMin, float nMax, float tMax = 5f, bool saturateDown = false)
        {
            float tMin = 0;
            float t = currentTime % tMax;
            float nRange = nMax - nMin;
            float tRange = tMax - tMin;
            float scaled = ( t - tMin) / tRange;
            float mapped = nMin + (scaled * nRange);
            float mapped_new;
            if( saturateDown )
                mapped_new = float.Parse((nMin - mapped).ToString("0.000"));
            else
                mapped_new = float.Parse((mapped).ToString("0.000"));

            if(!transitionStarted)
            {
                transitionStarted = true;
            }
            // terminate transition
            else if(maxNoiseReached)
            {
                doTransition = false;
                maxNoiseReached = false;
                transitionStarted = false;
                currentTime = 0f;
                setNewMood();
            }
            if(saturateDown && mapped_new < nMin+0.01 || !saturateDown && mapped_new > nMax-0.01)
                maxNoiseReached = true;

            _moodTransitionEffect.Parameters["UniformSat"].SetValue(mapped_new);
            _moodTransitionEffect.Parameters["UniformVal"].SetValue(-0.0f);
        }

        void DistortionTransition()
        {
            float mapped_new = 0f;
            if(useTransition)
            {
                // increase/decrease noise depending on time
                // TODO: Use time steps instead of contiuous (maybe effect looks nicer)
                float tMin = 0;
                float tMax = 3;
                float nMin = 0.001f;
                float nMax = 0.01f; //0.5f;
                float t = currentTime % tMax;
                float nRange = nMax - nMin;
                float tRange = tMax - tMin;
                float scaled = ( t - tMin) / tRange;
                float mapped = nMin + (scaled * nRange);
                mapped_new = float.Parse(mapped.ToString("0.000"));

                if(mapped < nLast)
                    newRound = true;

                if(!transitionStarted)
                {
                    transitionStarted = true;
                }
                // terminate transition
                else if(maxNoiseReached && newRound)
                {
                    doTransition = false;
                    maxNoiseReached = false;
                    transitionStarted = false;
                }
                else if(transitionStarted && !maxNoiseReached && newRound)
                {
                    maxNoiseReached = true;
                    setNewMood();
                }
                else if(maxNoiseReached)
                {
                    mapped_new = float.Parse((nMax - mapped).ToString("0.000"));
                }

                if(newRound)
                    newRound = false;

                if(mapped_new == 0)
                    mapped_new = nMin;

                nLast = mapped;
            }
            else
            {
                mapped_new = 0.01f;
            }

            // value between 0.001 and 0.5
            _moodTransitionEffect.Parameters["fNoiseAmount"].SetValue(mapped_new);
            _moodTransitionEffect.Parameters["fTimer"].SetValue(currentTime);
            _moodTransitionEffect.Parameters["iSeed"].SetValue(5);
        }

        void setNewMood()
        {
            if(newMood == MoodState.Depressed)
            {
                isDepressed = true;
                isManic = false;
            }
            else if(newMood == MoodState.Manic)
            {
                isManic = true;
                isDepressed = false;
            }
            else if(newMood == MoodState.Regular)
            {
                isDepressed = false;
                isManic = false;
            }
            newMood = MoodState.None;
        }

        void CombineWorldWithLight(RenderTarget2D worldTarget, RenderTarget2D lightTarget, RenderTarget2D renderTarget, SpriteBatch spriteBatch)
        {

            _graphicsDevice.SetRenderTarget(_renderTarget);
        
            _graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
        
            _graphicsDevice.Clear(Color.Black);
            _lightShader.Parameters["MaskTexture"].SetValue(lightTarget);

            spriteBatch.Begin(blendState: BlendState.AlphaBlend, effect: _lightShader);
            spriteBatch.Draw(worldTarget, Vector2.Zero, Color.Red);
            spriteBatch.End();

            _graphicsDevice.SetRenderTarget(null);
        }

        bool DrawLightMap(RenderTarget2D renderTarget, SpriteBatch spriteBatch)
        {
            bool hasLights = false;
            // Set the render target
            _graphicsDevice.SetRenderTarget(renderTarget);
        
            _graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
        
            // Draw the scene
            _graphicsDevice.Clear(Color.Black);
            
            //  Transform positions so correct lights are shown in viewport
            spriteBatch.Begin(blendState: BlendState.Additive, transformMatrix: ViewTransform);

            if (FlashlightOn)
            {
                Cursor cursor = (Cursor)GetUIByName("Cursor");
                if(cursor != null && cursor.LightMask != null)
                {
                    hasLights = true;
                    Texture2D lightMask = cursor.LightMask;
                    Vector2 texturePosition = new Vector2(cursor.MouseCoordinates.X - lightMask.Width/2, cursor.MouseCoordinates.Y - lightMask.Height/2);
                    spriteBatch.Draw(cursor.LightMask, texturePosition, Color.White);
                }
            }

            // TODO: add logic to draw lights stored in entities and rooms
            foreach (Thing thing in GetEntitiesOfType<Thing>())
            {
                if (thing.LightMask != null)
                {
                    hasLights = true;
                    spriteBatch.Draw(thing.LightMask, thing.Position, Color.White);
                }
            }

            Vector2 lightMaskPos = new Vector2(0, 0);
            foreach(Texture2D light in Lights)
            {
                hasLights = true;
                spriteBatch.Draw(light, lightMaskPos, Color.White);
            }

            spriteBatch.End();
        
            // Drop the render target
            _graphicsDevice.SetRenderTarget(null);
            return hasLights;
        }

        public void ToggleFalshlight()
        {
            FlashlightOn = !FlashlightOn;
        }

        void DrawGameWorldToTexture(RenderTarget2D renderTarget, SpriteBatch spriteBatch)
        {
            // Set the render target
            _graphicsDevice.SetRenderTarget(renderTarget);
        
            _graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
        
            // Draw the scene
            _graphicsDevice.Clear(Color.CornflowerBlue);
            DrawGameWorld(spriteBatch);
        
            // Drop the render target
            _graphicsDevice.SetRenderTarget(null);
        }

        public void DrawGameWorld(SpriteBatch spriteBatch){

            // The view transformation matrix
            Matrix view = ViewTransform;  // Matrix.Identity;

            // The projection transformation matrix
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, GlobalData.ScreenWidth, GlobalData.ScreenHeight, 0, 0, 1);

            _entityEffect.Parameters["view_projection"].SetValue(view * projection);
            // _entityEffect.Parameters["AmbientColor"].SetValue(Color.White.ToVector4());
            // _entityEffect.Parameters["AmbientIntensity"].SetValue(1f);

            // Draw game entities
            // spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null,null, ViewTransform);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, 
                SamplerState.LinearClamp, DepthStencilState.Default, 
                RasterizerState.CullNone, _entityEffect);  // effect: _entityEffect, samplerState: SamplerState.LinearClamp);
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

            // // Draw lightmaps
            // spriteBatch.Begin(SpriteSortMode.Immediate, _multiplicativeBlend);
            // spriteBatch.Draw(LightMap, new Rectangle(0, 0, GlobalData.ScreenWidth, GlobalData.ScreenHeight), Color.White);
            // spriteBatch.End();
        }

        public void DrawUI(SpriteBatch spriteBatch)
        {
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