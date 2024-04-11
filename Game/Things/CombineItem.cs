using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace conscious
{
    /// <summary>Class <c>CombineItem</c> holds data and logic of an Item 
    /// that can be combined with a specific other Item to create a locked Item (combinedItem object).
    /// </summary>
    ///
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
                           Item combinedItem,
                           int itemDependency,
                           ThoughtNode thought,
                           MoodStateManager moodStateManager, 
                           Texture2D texture, Vector2 position, int drawOrder)
                :base(id, name, pickUpAble, useAble, combineAble, giveAble, useWith, examineText, thought, moodStateManager, texture, position, drawOrder){
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