using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    public abstract class Entity : IComponent
    {
        protected Sprite _sprite;

        public Texture2D EntityTexture { get; set; }
        public int DrawOrder { get; set; }
        public Vector2 Position;
        public string Name { get; set; }
        public bool FixedDrawPosition { get; set; }
        public float Rotation { get; set; }
        public virtual int Width 
        {
            get { return EntityTexture.Width; }
        }
        public virtual int Height 
        {
            get { return EntityTexture.Height; }
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
            EntityTexture = texture;
            Rotation = 0f;
            _sprite = new Sprite(EntityTexture, Width, Height, Rotation);
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

        public void UpdateTexture(Texture2D texture)
        {
            _sprite.Texture = texture;
        }

        public virtual DataHolderEntity GetDataHolderEntity()
        {
            DataHolderEntity dataHolderEntity = new DataHolderEntity();
            dataHolderEntity.Name = Name;
            dataHolderEntity.PositionX  = Position.X;
            dataHolderEntity.PositionY = Position.Y;
            dataHolderEntity.Rotation = Rotation;
            dataHolderEntity.texturePath = EntityTexture.ToString(); //EntityTexture.Name;
            return dataHolderEntity;
        }
    }
}