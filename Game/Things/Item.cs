using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    //[JsonObject(MemberSerialization.OptIn)]
    public class Item : Thing
    {
        protected string _examineText;

        public int Id;
        public bool PickUpAble { get; }
        public bool UseAble { get; }
        public bool CombineAble { get; }
        public bool GiveAble { get; }
        public bool UseWith { get; set; }

        public Item(int id,
                    string name, 
                    bool pickUpAble, 
                    bool useAble, 
                    bool combineAble, 
                    bool giveAble, 
                    bool useWith, 
                    string examineText,
                    Texture2D texture, Vector2 position) : base(name, texture, position){
            Id = id;
            
            PickUpAble = pickUpAble;
            UseAble = useAble;
            CombineAble = combineAble;
            GiveAble = giveAble;
            UseWith = useWith;
            _examineText = examineText;

            Collidable = true;
        }

        public string Examine(){
            return _examineText;
        }

        public bool PickUp(){
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
            dataHolderItem.texturePath = entityTexture.ToString(); //ItemTexture.Name;
            // Item
            dataHolderItem.Id = Id;
            dataHolderItem.PickUpAble = PickUpAble;
            dataHolderItem.UseAble = UseAble;
            dataHolderItem.UseWith = UseWith;
            dataHolderItem.CombineAble = CombineAble;
            dataHolderItem.GiveAble = GiveAble;
            dataHolderItem.ExamineText = _examineText;
            return dataHolderItem;
        }
    }
}