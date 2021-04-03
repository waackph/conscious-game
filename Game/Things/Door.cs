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
                    MoodState moodChange,
                    int itemDependency,
                    int roomId,
                    bool isUnlocked,
                    UIThought thought,
                    Texture2D texture, Vector2 position)
                    :base(id, name, pickUpAble, useAble, combineAble, giveAble, useWith, examineText, moodChange, thought, texture, position){
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
            DataHolderDoor dataHolderEntity = new DataHolderDoor();
            dataHolderEntity = (DataHolderDoor)base.GetDataHolderEntity(dataHolderEntity);
            // Door
            dataHolderEntity.ItemDependency = _itemDependency;
            dataHolderEntity.IsUnlocked = IsUnlocked;
            dataHolderEntity.RoomId = RoomId;
            return dataHolderEntity;
        }

        public DataHolderEntity GetDataHolderEntity(DataHolderDoor dataHolderEntity)
        {
            dataHolderEntity = (DataHolderDoor)base.GetDataHolderEntity(dataHolderEntity);
            // Door
            dataHolderEntity.ItemDependency = _itemDependency;
            dataHolderEntity.IsUnlocked = IsUnlocked;
            dataHolderEntity.RoomId = RoomId;
            return dataHolderEntity;
        }
    }
}