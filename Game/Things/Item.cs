using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    /// <summary>Class <c>Item</c> holds data and logic of an Item.
    /// an Item is interactable and acts upon data about if it is 
    /// pickupable, useable, combineable, giveable, or can be used with some other item.
    /// </summary>
    ///
    public class Item : Thing
    {
        protected string _examineText;

        public bool PickUpAble { get; set; }
        public bool UseAble { get; set; }
        public bool CombineAble { get; set; }
        public bool GiveAble { get; set; }
        public bool UseWith { get; set; }

        public Item(int id,
                    string name, 
                    bool pickUpAble, 
                    bool useAble, 
                    bool combineAble, 
                    bool giveAble, 
                    bool useWith, 
                    string examineText,
                    ThoughtNode thought, 
                    MoodStateManager moodStateManager, 
                    Texture2D texture, Vector2 position, int drawOrder, bool collidable = true, int collBoxHeight = 20,
                    ThoughtNode eventThought = null)
                    : base(id, thought, moodStateManager, name, texture, position, drawOrder, collidable, collBoxHeight, eventThought)
        {
            PickUpAble = pickUpAble;
            UseAble = useAble;
            CombineAble = combineAble;
            GiveAble = giveAble;
            UseWith = useWith;
            _examineText = examineText;

            DrawOrder = drawOrder;
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
            return dataHolderEntity;
        }    
    }
}