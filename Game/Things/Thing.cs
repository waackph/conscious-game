using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    public class Thing : Entity
    {
        protected UIThought _thought;
        public Thing(UIThought thought, string name, Texture2D texture, Vector2 position) : base(name, texture, position)
        {
            _thought = thought;
        }

        public override DataHolderEntity GetDataHolderEntity()
        {
            DataHolderThing dataHolderEntity = new DataHolderThing();
            dataHolderEntity = (DataHolderThing)base.GetDataHolderEntity(dataHolderEntity);
            dataHolderEntity.Thought = _thought;
            return dataHolderEntity;
        }
        
        public DataHolderEntity GetDataHolderEntity(DataHolderThing dataHolderEntity)
        {
            dataHolderEntity = (DataHolderThing)base.GetDataHolderEntity(dataHolderEntity);
            dataHolderEntity.Thought = _thought;
            return dataHolderEntity;
        }
    }
}