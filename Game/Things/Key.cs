using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
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
                  Texture2D texture, Vector2 position) 
                : base(id, name, pickUpAble, useAble, combineAble, giveAble, useWith, examineText, texture, position){
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
            DataHolderKey dataHolderKey = new DataHolderKey();
            dataHolderKey.Name = Name;
            dataHolderKey.PositionX  = Position.X;
            dataHolderKey.PositionY = Position.Y;
            dataHolderKey.Rotation = Rotation;
            dataHolderKey.texturePath = EntityTexture.ToString(); //KeyTexture.Name;
            // Item
            dataHolderKey.Id = Id;
            dataHolderKey.PickUpAble = PickUpAble;
            dataHolderKey.UseAble = UseAble;
            dataHolderKey.UseWith = UseWith;
            dataHolderKey.CombineAble = CombineAble;
            dataHolderKey.GiveAble = GiveAble;
            dataHolderKey.ExamineText = _examineText;
            // Key
            dataHolderKey.ItemDependency = _itemDependency;
            return dataHolderKey;
        }
    }
}