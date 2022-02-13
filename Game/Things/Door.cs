using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    public class Door : Item
    {
        private int _itemDependency;
        private bool _isUnlocked;
        private Texture2D _closeTexture;
        public bool IsClosed;

        public bool currentlyUsed = false;
        public int RoomId;
        public int DoorId;
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
                    int doorId,
                    Vector2 initPlayerPos,
                    Texture2D closeTexture,
                    bool isUnlocked,
                    ThoughtNode thought,
                    MoodStateManager moodStateManager, 
                    Texture2D texture, Vector2 position)
                    :base(id, name, pickUpAble, useAble, combineAble, giveAble, useWith, examineText, thought, moodStateManager, texture, position){
            _itemDependency = itemDependency;
            RoomId = roomId;
            DoorId = doorId;
            InitPlayerPos = initPlayerPos;
            _isUnlocked = isUnlocked;
            _closeTexture = closeTexture;
            this.CloseDoor();
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

        public void OpenDoor()
        {
            _sprite.Texture = EntityTexture;
            IsClosed = false;
        }

        public void CloseDoor()
        {
            _sprite.Texture = _closeTexture;
            IsClosed = true;
        }
        
        public override DataHolderEntity GetDataHolderEntity()
        {
            DataHolderDoor dataHolderEntity = new DataHolderDoor();
            dataHolderEntity = (DataHolderDoor)base.GetDataHolderEntity(dataHolderEntity);
            // Door
            dataHolderEntity.ItemDependency = _itemDependency;
            dataHolderEntity.IsUnlocked = _isUnlocked;
            dataHolderEntity.RoomId = RoomId;
            dataHolderEntity.DoorId = DoorId;
            dataHolderEntity.InitPlayerPosX = InitPlayerPos.X;
            dataHolderEntity.InitPlayerPosY = InitPlayerPos.Y;
            dataHolderEntity.CloseTexturePath = _closeTexture.ToString();
            return dataHolderEntity;
        }

        public DataHolderEntity GetDataHolderEntity(DataHolderDoor dataHolderEntity)
        {
            dataHolderEntity = (DataHolderDoor)base.GetDataHolderEntity(dataHolderEntity);
            // Door
            dataHolderEntity.ItemDependency = _itemDependency;
            dataHolderEntity.IsUnlocked = _isUnlocked;
            dataHolderEntity.RoomId = RoomId;
            dataHolderEntity.DoorId = DoorId;
            dataHolderEntity.InitPlayerPosX = InitPlayerPos.X;
            dataHolderEntity.InitPlayerPosY = InitPlayerPos.Y;
            dataHolderEntity.CloseTexturePath = _closeTexture.ToString();
            return dataHolderEntity;
        }
    }
}