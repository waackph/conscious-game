using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    //[JsonObject(MemberSerialization.OptIn)]
    public class Item : Thing
    {
        protected string _examineText;

        public int Id;
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
                    UIThought thought, 
                    Texture2D texture, Vector2 position) : base(thought, name, texture, position){
            Id = id;
            
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
            // Simplify code like this?
            // DataHolderEntity dataHolderBase = base.GetDataHolderEntity();
            // DataHolderItem dataHolderEntity = (DataHolderItem)dataHolderBase;

            DataHolderItem dataHolderEntity = new DataHolderItem();
            dataHolderEntity.Name = Name;
            dataHolderEntity.PositionX  = Position.X;
            dataHolderEntity.PositionY = Position.Y;
            dataHolderEntity.Rotation = Rotation;
            dataHolderEntity.texturePath = EntityTexture.ToString(); //ItemTexture.Name;
            // Thing
            dataHolderEntity.Thought = _thought;
            // Item
            dataHolderEntity.Id = Id;
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