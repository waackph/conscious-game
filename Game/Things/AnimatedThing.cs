using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace conscious
{
    /// <summary>Class <c>Item</c> holds data and logic of an Item.
    /// an Item is interactable and acts upon data about if it is 
    /// pickupable, useable, combineable, giveable, or can be used with some other item.
    /// </summary>
    ///
    public class AnimatedThing : Thing
    {

        private Dictionary<MoodState, AnimatedSprite> _moodMoveAnimation;
        public AnimatedSprite MoveAnimation;
        private float _moveSpeed = 0;

        public AnimatedThing(int id,
                            string name,
                            ThoughtNode thought,
                            MoodStateManager moodStateManager,
                            Texture2D texture, Vector2 position, int drawOrder,
                            float moveSpeed = 150, int atlasRows = 1, int atlasColumns = 2, int animSpeed = 200, float scale = 1.0f
                            )
                            : base(id, thought, moodStateManager, name, texture, position, drawOrder)
        {
            DrawOrder = drawOrder;
            _moveSpeed = moveSpeed;
            MoveAnimation = new AnimatedSprite(texture, atlasRows, atlasColumns, (Width / atlasColumns), (Height / atlasRows), 0f, animSpeed);
            Scale = scale;
            _moodMoveAnimation = new Dictionary<MoodState, AnimatedSprite>();
            _moodMoveAnimation[MoodState.None] = new AnimatedSprite(texture, atlasRows, atlasColumns, (Width / atlasColumns), (Height / atlasRows), 0f, animSpeed);
            // _moodMoveAnimation[MoodState.Depressed] = new AnimatedSprite(moveDepressed, 2, 2, (Width / 2), Height, 0f, 300);
            // _moodMoveAnimation[MoodState.Regular] = new AnimatedSprite(texture, 2, 2, (Width / 2), Height, 0f, 200);
            // _moodMoveAnimation[MoodState.Manic] = new AnimatedSprite(moveTexture, 2, 2, (Width / 2), Height, 0f, 150);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (_moveSpeed != 0)
            {
                float totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                Position.X += _moveSpeed * totalSeconds;
                if (Position.X > GlobalData.ScreenWidth + Width)
                {
                    Position.X = -Width;
                }
            }
            MoveAnimation.Update(gameTime);
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            // base.Draw(spriteBatch);
            MoveAnimation.Draw(spriteBatch, Position, SpriteEffects.None, Scale);
        }

        // public override DataHolderEntity GetDataHolderEntity()
        // {
        //     DataHolderThing dataHolderEntity = new DataHolderThing();
        //     dataHolderEntity = (DataHolderThing)base.GetDataHolderEntity(dataHolderEntity);
        //     // Animated Thing ...
        //     return dataHolderEntity;
        // }

        // public DataHolderEntity GetDataHolderEntity(DataHolderThing dataHolderEntity)
        // {
        //     dataHolderEntity = (DataHolderThing)base.GetDataHolderEntity(dataHolderEntity);
        //     // Animated Thing ...
        //     return dataHolderEntity;
        // }
    }
}