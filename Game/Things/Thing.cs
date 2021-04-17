using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    public class Thing : Entity
    {
        public ThoughtNode Thought { get; protected set; }
        public int Id { get; protected set; }
        public Thing(int id, ThoughtNode thought, string name, Texture2D texture, Vector2 position) : base(name, texture, position)
        {
            Thought = thought;
            Id = id;
        }

        public override DataHolderEntity GetDataHolderEntity()
        {
            DataHolderThing dataHolderEntity = new DataHolderThing();
            dataHolderEntity = (DataHolderThing)base.GetDataHolderEntity(dataHolderEntity);
            dataHolderEntity.Thought = Thought;
            return dataHolderEntity;
        }
        
        public DataHolderEntity GetDataHolderEntity(DataHolderThing dataHolderEntity)
        {
            dataHolderEntity = (DataHolderThing)base.GetDataHolderEntity(dataHolderEntity);
            dataHolderEntity.Id = Id;
            dataHolderEntity.Thought = Thought;
            return dataHolderEntity;
        }
    }
}