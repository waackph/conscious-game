using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    //[JsonObject(MemberSerialization.OptIn)]
    public class Item : Thing
    {
        protected string _examineText;

        public bool PickUpAble { get; set; }
        public bool UseAble { get; set; }
        public bool CombineAble { get; set; }
        public bool GiveAble { get; set; }
        public bool UseWith { get; set; }
        public MoodState MoodChange { get; set; }

        public Item(int id,
                    string name, 
                    bool pickUpAble, 
                    bool useAble, 
                    bool combineAble, 
                    bool giveAble, 
                    bool useWith, 
                    string examineText,
                    MoodState moodChange,
                    ThoughtNode thought, 
                    Texture2D texture, Vector2 position) : base(id, thought, name, texture, position){            
            PickUpAble = pickUpAble;
            UseAble = useAble;
            CombineAble = combineAble;
            GiveAble = giveAble;
            UseWith = useWith;
            MoodChange = moodChange;
            _examineText = examineText;

            Collidable = true;
        }

        public virtual string Examine(){
            return _examineText;
        }

        public virtual bool PickUp(){
            return PickUpAble;
        }

        public virtual bool Use(Room room, InventoryManager inventory, Player player){
            return UseAble;
        }

        public virtual bool Use(Room room, InventoryManager inventory, Player player, Item item){
            return UseAble;
        }

        public virtual Item Combine(Item item){
            return null;
        }

        public override DataHolderEntity GetDataHolderEntity()
        {
            DataHolderItem dataHolderEntity = new DataHolderItem();
            dataHolderEntity = (DataHolderItem)base.GetDataHolderEntity(dataHolderEntity);
            // Item
            dataHolderEntity.PickUpAble = PickUpAble;
            dataHolderEntity.UseAble = UseAble;
            dataHolderEntity.UseWith = UseWith;
            dataHolderEntity.CombineAble = CombineAble;
            dataHolderEntity.GiveAble = GiveAble;
            dataHolderEntity.ExamineText = _examineText;
            dataHolderEntity.MoodChange = MoodChange;
            return dataHolderEntity;
        }

        public DataHolderEntity GetDataHolderEntity(DataHolderItem dataHolderEntity)
        {
            dataHolderEntity = (DataHolderItem)base.GetDataHolderEntity(dataHolderEntity);
            // Item
            dataHolderEntity.PickUpAble = PickUpAble;
            dataHolderEntity.UseAble = UseAble;
            dataHolderEntity.UseWith = UseWith;
            dataHolderEntity.CombineAble = CombineAble;
            dataHolderEntity.GiveAble = GiveAble;
            dataHolderEntity.ExamineText = _examineText;
            dataHolderEntity.MoodChange = MoodChange;
            return dataHolderEntity;
        }    
    }
}