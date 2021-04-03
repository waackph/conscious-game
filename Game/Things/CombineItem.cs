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
            dataHolderEntity = (DataHolderCombineItem)base.GetDataHolderEntity(dataHolderEntity);
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
        public DataHolderEntity GetDataHolderEntity(DataHolderCombineItem dataHolderEntity)
        {
            dataHolderEntity = (DataHolderCombineItem)base.GetDataHolderEntity(dataHolderEntity);
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