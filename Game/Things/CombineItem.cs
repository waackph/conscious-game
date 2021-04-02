using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace conscious
{
    public class CombineItem : Item
    {
        private Item _combinedItem;
        private int _itemDependency;
        public CombineItem(int id,
                           string name, 
                           bool pickUpAble, 
                           bool useAble, 
                           bool combineAble, 
                           bool giveAble, 
                           bool useWith, 
                           string examineText,
                           MoodState moodChange,
                           Item combinedItem,
                           int itemDependency,
                           UIThought thought,
                           Texture2D texture, Vector2 position)
                :base(id, name, pickUpAble, useAble, combineAble, giveAble, useWith, examineText, moodChange, thought, texture, position){
            _combinedItem = combinedItem;
            _itemDependency = itemDependency;
        }

        public override Item Combine(Item item)
        {
            if(item.Id == _itemDependency){
                if(_combinedItem != null){
                    return _combinedItem;
                }
                else{
                    CombineItem otherItem = (CombineItem)item;
                    return otherItem._combinedItem;
                }
            }
            else{
                return null;
            }
        }
        public override DataHolderEntity GetDataHolderEntity()
        {
            DataHolderCombineItem dataHolderEntity = new DataHolderCombineItem();
            dataHolderEntity.Name = Name;
            dataHolderEntity.PositionX  = Position.X;
            dataHolderEntity.PositionY = Position.Y;
            dataHolderEntity.Rotation = Rotation;
            dataHolderEntity.texturePath = EntityTexture.ToString(); //ComDataHolderCombineItemTexture.Name;
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
            // ComDataHolderCombineItem
            dataHolderEntity.ItemDependency = _itemDependency;
            if(_combinedItem == null)
            {
                dataHolderEntity.CombineItem = null;
            }
            else{
                dataHolderEntity.CombineItem = _combinedItem.GetDataHolderEntity();
            }
            return dataHolderEntity;
        }
    }
}