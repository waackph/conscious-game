using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    /// <summary>Class <c>Key</c> holds data and logic of an Item 
    /// that can be used with another object to unlock something in the Room.
    /// </summary>
    ///
    public class Key : Item
    {
        private int _itemDependency;

        public Key(int id,
                  string name, 
                  bool pickUpAble, 
                  bool useAble, 
                  bool combineAble, 
                  bool giveAble, 
                  bool useWith, 
                  string examineText,
                  int itemDependency,
                  ThoughtNode thought,
                  MoodStateManager moodStateManager, 
                  Texture2D texture, Vector2 position, int drawOrder) 
                : base(id, name, pickUpAble, useAble, combineAble, giveAble, useWith, examineText, thought, moodStateManager, texture, position, drawOrder){
            _itemDependency = itemDependency;
        }

        public override bool Use(Room room, InventoryManager inventory, Player player, Item item)
        {
            if(item.Id == _itemDependency){
                item.Use(room, inventory, player, this);
                return true;
            }
            return false;
        }

        public override DataHolderEntity GetDataHolderEntity()
        {
            DataHolderKey dataHolderEntity = new DataHolderKey();
            dataHolderEntity = (DataHolderKey)base.GetDataHolderEntity(dataHolderEntity);
            // Key
            dataHolderEntity.ItemDependency = _itemDependency;
            return dataHolderEntity;
        }
        public DataHolderEntity GetDataHolderEntity(DataHolderKey dataHolderEntity)
        {
            dataHolderEntity = (DataHolderKey)base.GetDataHolderEntity(dataHolderEntity);
            // Key
            dataHolderEntity.ItemDependency = _itemDependency;
            return dataHolderEntity;
        }
    }
}