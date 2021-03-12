using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    public abstract class Entity : IComponent
    {
        protected Sprite _sprite;

        public Texture2D entityTexture { get; }
        public int DrawOrder { get; set; }
        public Vector2 Position;
        public string Name { get; }
        public bool FixedDrawPosition { get; set; }
        public float Rotation { get; set; }
        public virtual int Width 
        {
            get { return entityTexture.Width; }
        }
        public virtual int Height 
        {
            get { return entityTexture.Height; }
        }
        public bool Collidable { get; set; } 
        public virtual Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)Position.X - Width/2, 
                                     (int)Position.Y - Height/2,
                                     Width, Height);
            }
        }

        public Entity(string name, Texture2D texture, Vector2 position)
        {
            entityTexture = texture;
            Rotation = 0f;
            _sprite = new Sprite(entityTexture, Width, Height, Rotation);
            Position = position;
            Name = name;
            FixedDrawPosition = false;
            Collidable = false;
        }

        public virtual void Update(GameTime gameTime){ }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            _sprite.Draw(spriteBatch, Position, SpriteEffects.None);
        }

        public virtual DataHolderEntity GetDataHolderEntity()
        {
            DataHolderEntity dataHolderEntity = new DataHolderEntity();
            dataHolderEntity.Name = Name;
            dataHolderEntity.PositionX  = Position.X;
            dataHolderEntity.PositionY = Position.Y;
            dataHolderEntity.Rotation = Rotation;
            dataHolderEntity.texturePath = entityTexture.ToString(); //entityTexture.Name;
            return dataHolderEntity;
        }
    }
}