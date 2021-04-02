using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;
using System;
namespace conscious
{
    public class MorphingItem : Item
    {
        private MoodStateManager _moodStateManager;
        private Dictionary<MoodState, Item> _items;
        private Item _currentItem;

        public MorphingItem(MoodStateManager moodStateManager, 
                             Dictionary<MoodState, Item> items,
                             int id,
                             string name, 
                             bool pickUpAble, 
                             bool useAble, 
                             bool combineAble, 
                             bool giveAble, 
                             bool useWith, 
                             string examineText,
                             MoodState moodChange,
                             UIThought thought,
                             Texture2D texture, Vector2 position) : base(id, name, pickUpAble, useAble, combineAble, giveAble, useWith, examineText, moodChange, thought, texture, position)
        {
            _moodStateManager = moodStateManager;
            _items = items;
            setCurrentItem();
        }
        public void setCurrentItem()
        {
            if(_items.ContainsKey(_moodStateManager.moodState))
            {
                Item item = _items[_moodStateManager.moodState];
                _currentItem = item;
                this.Name = item.Name;
                this.PickUpAble = item.PickUpAble;
                this.UseAble = item.UseAble;
                this.CombineAble = item.CombineAble;
                this.GiveAble = item.GiveAble;
                this.UseWith = item.UseWith;
                this.Position = item.Position;
                this.EntityTexture = item.EntityTexture;
                this.UpdateTexture(this.EntityTexture);
            }
        }

        public Item getCurrentItem()
        {
            return _currentItem;
        }

        public override string Examine(){
            return _currentItem.Examine();
        }

        public override bool PickUp(){
            return _currentItem.PickUp();
        }

        public override bool Use(Room room, InventoryManager inventory, Player player){
            return _currentItem.Use(room, inventory, player);
        }

        public override bool Use(Room room, InventoryManager inventory, Player player, Item item){
            return _currentItem.Use(room, inventory, player, item);
        }

        public override Item Combine(Item item){
            return _currentItem.Combine(item);
        }

        public override DataHolderEntity GetDataHolderEntity()
        {
            DataHolderMorphingItem dataHolderEntity = new DataHolderMorphingItem();
            dataHolderEntity.Name = Name;
            dataHolderEntity.PositionX  = Position.X;
            dataHolderEntity.PositionY = Position.Y;
            dataHolderEntity.Rotation = Rotation;
            dataHolderEntity.texturePath = EntityTexture.ToString(); //ItemTexture.Name;
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
            // Morphing Item
            Dictionary<MoodState, DataHolderEntity> dhItems = new Dictionary<MoodState, DataHolderEntity>();
            foreach(KeyValuePair<MoodState, Item> entry in _items)
            {
                dhItems.Add(entry.Key, entry.Value.GetDataHolderEntity());
            }
            dataHolderEntity.Items = dhItems;
            return dataHolderEntity;
        }
    }
}