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
                           Item combinedItem,
                           int itemDependency,
                           Texture2D texture, Vector2 position)
                :base(id, name, pickUpAble, useAble, combineAble, giveAble, useWith, examineText, texture, position){
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
            DataHolderCombineItem dataHolderComDataHolderCombineItem = new DataHolderCombineItem();
            dataHolderComDataHolderCombineItem.Name = Name;
            dataHolderComDataHolderCombineItem.PositionX  = Position.X;
            dataHolderComDataHolderCombineItem.PositionY = Position.Y;
            dataHolderComDataHolderCombineItem.Rotation = Rotation;
            dataHolderComDataHolderCombineItem.texturePath = EntityTexture.ToString(); //ComDataHolderCombineItemTexture.Name;
            // Item
            dataHolderComDataHolderCombineItem.Id = Id;
            dataHolderComDataHolderCombineItem.PickUpAble = PickUpAble;
            dataHolderComDataHolderCombineItem.UseAble = UseAble;
            dataHolderComDataHolderCombineItem.UseWith = UseWith;
            dataHolderComDataHolderCombineItem.CombineAble = CombineAble;
            dataHolderComDataHolderCombineItem.GiveAble = GiveAble;
            dataHolderComDataHolderCombineItem.ExamineText = _examineText;
            // ComDataHolderCombineItem
            dataHolderComDataHolderCombineItem.ItemDependency = _itemDependency;
            if(_combinedItem == null)
            {
                dataHolderComDataHolderCombineItem.CombineItem = null;
            }
            else{
                dataHolderComDataHolderCombineItem.CombineItem = _combinedItem.GetDataHolderEntity();
            }
            return dataHolderComDataHolderCombineItem;
        }
    }
}