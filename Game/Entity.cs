using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    /// <summary>Abstract class <c>Entity</c> implements the most basic Entity in the game. 
    /// It can be a Thing in the Room or a UI Element.
    /// </summary>
    ///
    public abstract class Entity : IComponent
    {
        protected Sprite _sprite;

        public Texture2D EntityTexture { get; protected set; }
        public int DrawOrder { get; protected set; }
        public Vector2 Position;
        public string Name { get; protected set; }
        public bool FixedDrawPosition { get; set; }
        public float Rotation { get; protected set; }
        public float Scale { get; protected set; }
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
        public virtual Rectangle CollisionBox
        {
            get
            {
                return new Rectangle((int)Position.X - Width/2, 
                                     (int)Position.Y + Height/2 - 20,
                                     Width, 20);
            }
        }

        public Entity(string name, Texture2D texture, Vector2 position, int drawOrder, bool collidable = false)
        {
            EntityTexture = texture;
            Rotation = 0f;
            _sprite = new Sprite(EntityTexture, Width, Height, Rotation);
            Position = position;
            Name = name;
            FixedDrawPosition = false;
            Collidable = collidable;
            DrawOrder = drawOrder;
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

        public void UpdateDrawOrder(int order)
        {
            DrawOrder = order;
        }

        public virtual DataHolderEntity GetDataHolderEntity()
        {
            DataHolderEntity dataHolderEntity = new DataHolderEntity();
            dataHolderEntity.Name = Name;
            dataHolderEntity.PositionX  = Position.X;
            dataHolderEntity.PositionY = Position.Y;
            dataHolderEntity.Rotation = Rotation;
            dataHolderEntity.texturePath = EntityTexture.ToString(); //EntityTexture.Name;
            dataHolderEntity.DrawOrder = DrawOrder;
            dataHolderEntity.Collidable = Collidable;
            return dataHolderEntity;
        }

        public virtual DataHolderEntity GetDataHolderEntity(DataHolderEntity dataHolderEntity)
        {
            dataHolderEntity.Name = Name;
            dataHolderEntity.PositionX  = Position.X;
            dataHolderEntity.PositionY = Position.Y;
            dataHolderEntity.Rotation = Rotation;
            dataHolderEntity.texturePath = EntityTexture.ToString(); //EntityTexture.Name;
            dataHolderEntity.DrawOrder = DrawOrder;
            dataHolderEntity.Collidable = Collidable;
            return dataHolderEntity;
        }
    }
}