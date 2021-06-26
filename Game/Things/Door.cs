using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace conscious
{
    public class Door : Item
    {
        private int _itemDependency;
        private bool _isUnlocked;

        public bool currentlyUsed = false;
        public int RoomId;
        public Vector2 InitPlayerPos;
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
                    Vector2 initPlayerPos,
                    bool isUnlocked,
                    ThoughtNode thought,
                    Texture2D texture, Vector2 position)
                    :base(id, name, pickUpAble, useAble, combineAble, giveAble, useWith, examineText, thought, texture, position){
            _itemDependency = itemDependency;
            RoomId = roomId;
            InitPlayerPos = initPlayerPos;
            _isUnlocked = isUnlocked;
        }

        public override bool Use(Room room, InventoryManager inventory, Player player, Item item){
            if(UseAble == true){

                if(_isUnlocked != true && item != null && item.Id == _itemDependency){
                    _isUnlocked = true;
                    UseWith = false;
                    currentlyUsed = true;
                }
                if(_isUnlocked == true){
                    currentlyUsed = true;
                }
            }
            return false;
        }

        public override bool Use(Room room, InventoryManager inventory, Player player)
        {
            if(UseAble == true){
                if(_isUnlocked == true){
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
            dataHolderEntity.IsUnlocked = _isUnlocked;
            dataHolderEntity.RoomId = RoomId;
            dataHolderEntity.InitPlayerPosX = InitPlayerPos.X;
            dataHolderEntity.InitPlayerPosY = InitPlayerPos.Y;
            return dataHolderEntity;
        }

        public DataHolderEntity GetDataHolderEntity(DataHolderDoor dataHolderEntity)
        {
            dataHolderEntity = (DataHolderDoor)base.GetDataHolderEntity(dataHolderEntity);
            // Door
            dataHolderEntity.ItemDependency = _itemDependency;
            dataHolderEntity.IsUnlocked = _isUnlocked;
            dataHolderEntity.RoomId = RoomId;
            dataHolderEntity.InitPlayerPosX = InitPlayerPos.X;
            dataHolderEntity.InitPlayerPosY = InitPlayerPos.Y;
            return dataHolderEntity;
        }
    }
}