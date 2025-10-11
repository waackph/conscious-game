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
        public int CollisionBoxHeight;
        public string Name { get; protected set; }
        public bool FixedDrawPosition { get; set; }
        public float Rotation { get; protected set; }
        public float Scale { get; set; }
        public bool IsActive { get; set; }
        public virtual int Width
        {
            get { return EntityTexture.Width; }
        }
        public virtual int Height 
        {
            get { return EntityTexture.Height; }
        }

        private bool _collidable;
        public bool Collidable
        {
            get { return _collidable && IsActive; }
            set { _collidable = value; }
        }
        
        public virtual Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)Position.X - Width / 2,
                                     (int)Position.Y - Height / 2,
                                     Width, Height);
            }
        }
        public virtual Rectangle CollisionBox
        {
            get
            {
                return new Rectangle((int)Position.X - Width/2, 
                                     (int)Position.Y + Height/2 - CollisionBoxHeight,
                                     Width, CollisionBoxHeight);
            }
        }

        public Entity(string name, Texture2D texture, Vector2 position, int drawOrder,
                      bool collidable = false, int collBoxHeight = 20, bool isActive = true)
        {
            EntityTexture = texture;
            Rotation = 0f;
            _sprite = new Sprite(EntityTexture, Width, Height, Rotation);
            Position = position;
            CollisionBoxHeight = collBoxHeight;
            Name = name;
            FixedDrawPosition = false;
            Collidable = collidable;
            DrawOrder = drawOrder;
            IsActive = isActive;
        }

        public virtual void Update(GameTime gameTime)
        {
            // Default: do nothing
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(IsActive)
                _sprite.Draw(spriteBatch, Position, SpriteEffects.None);
        }

        public void UpdateTexture(Texture2D texture)
        {
            EntityTexture = texture;
            _sprite = new Sprite(EntityTexture, Width, Height, Rotation);
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
            dataHolderEntity.IsActive = IsActive;
            dataHolderEntity.CollisionBoxHeight = CollisionBoxHeight;
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
            dataHolderEntity.IsActive = IsActive;
            dataHolderEntity.CollisionBoxHeight = CollisionBoxHeight;
            return dataHolderEntity;
        }
    }
}