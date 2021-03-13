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
                    Texture2D texture, Vector2 position) : base(name, texture, position){
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
            DataHolderItem dataHolderItem = new DataHolderItem();
            dataHolderItem.Name = Name;
            dataHolderItem.PositionX  = Position.X;
            dataHolderItem.PositionY = Position.Y;
            dataHolderItem.Rotation = Rotation;
            dataHolderItem.texturePath = EntityTexture.ToString(); //ItemTexture.Name;
            // Item
            dataHolderItem.Id = Id;
            dataHolderItem.PickUpAble = PickUpAble;
            dataHolderItem.UseAble = UseAble;
            dataHolderItem.UseWith = UseWith;
            dataHolderItem.CombineAble = CombineAble;
            dataHolderItem.GiveAble = GiveAble;
            dataHolderItem.ExamineText = _examineText;
            dataHolderItem.MoodChange = MoodChange;
            return dataHolderItem;
        }
    }
}