using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace conscious
{
    public class Door : Item
    {
        private int _itemDependency;
        private bool IsUnlocked;

        public bool currentlyUsed = false;
        public int RoomId;
        public Door(int id,
                    string name, 
                    bool pickUpAble, 
                    bool useAble, 
                    bool combineAble, 
                    bool giveAble, 
                    bool useWith, 
                    string examineText,
                    int itemDependency,
                    int roomId,
                    bool isUnlocked,
                    Texture2D texture, Vector2 position)
                    :base(id, name, pickUpAble, useAble, combineAble, giveAble, useWith, examineText, texture, position){
            _itemDependency = itemDependency;
            RoomId = roomId;
            IsUnlocked = isUnlocked;
        }

        public override bool Use(Room room, InventoryManager inventory, Player player, Item item){
            if(UseAble == true){

                if(IsUnlocked != true && item != null && item.Id == _itemDependency){
                    IsUnlocked = true;
                    UseWith = false;
                }
                if(IsUnlocked == true){
                    currentlyUsed = true;
                }
            }
            return false;
        }

        public override bool Use(Room room, InventoryManager inventory, Player player)
        {
            if(UseAble == true){
                if(IsUnlocked == true){
                    currentlyUsed = true;
                }
            }
            return false;
        }
        public override DataHolderEntity GetDataHolderEntity()
        {
            DataHolderDoor dataHolderDoor = new DataHolderDoor();
            dataHolderDoor.Name = Name;
            dataHolderDoor.PositionX  = Position.X;
            dataHolderDoor.PositionY = Position.Y;
            dataHolderDoor.Rotation = Rotation;
            dataHolderDoor.texturePath = EntityTexture.ToString(); //DoorTexture.Name;
            // Item
            dataHolderDoor.Id = Id;
            dataHolderDoor.PickUpAble = PickUpAble;
            dataHolderDoor.UseAble = UseAble;
            dataHolderDoor.UseWith = UseWith;
            dataHolderDoor.CombineAble = CombineAble;
            dataHolderDoor.GiveAble = GiveAble;
            dataHolderDoor.ExamineText = _examineText;
            // Door
            dataHolderDoor.ItemDependency = _itemDependency;
            dataHolderDoor.IsUnlocked = IsUnlocked;
            dataHolderDoor.RoomId = RoomId;
            return dataHolderDoor;
        }
    }
}