using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    public class Thing : Entity
    {
        public ThoughtNode Thought { get; protected set; }
        public int Id { get; protected set; }
        public bool IsInInventory { get; set; }

        public Thing(int id, ThoughtNode thought, string name, Texture2D texture, Vector2 position) : base(name, texture, position)
        {
            Thought = thought;
            if(Thought != null && name != "")
            {
                Thought.Thought = "[" + name + "] " + Thought.Thought;
            }
            Id = id;
            IsInInventory = false;
        }

        public virtual Rectangle CollisionBox
        {
            get
            {
                return new Rectangle((int)Position.X - Width/2, 
                                     (int)Position.Y + Height/2,
                                     Width, 20);
            }
        }

        public override DataHolderEntity GetDataHolderEntity()
        {
            DataHolderThing dataHolderEntity = new DataHolderThing();
            dataHolderEntity = (DataHolderThing)base.GetDataHolderEntity(dataHolderEntity);
            dataHolderEntity.Thought = Thought;
            dataHolderEntity.IsInInventory = IsInInventory;
            return dataHolderEntity;
        }
        
        public DataHolderEntity GetDataHolderEntity(DataHolderThing dataHolderEntity)
        {
            dataHolderEntity = (DataHolderThing)base.GetDataHolderEntity(dataHolderEntity);
            dataHolderEntity.Id = Id;
            dataHolderEntity.Thought = Thought;
            dataHolderEntity.IsInInventory = IsInInventory;
            return dataHolderEntity;
        }
    }
}