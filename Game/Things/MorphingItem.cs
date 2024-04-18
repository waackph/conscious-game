using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;

namespace conscious
{
    /// <summary>Class <c>MorphingItem</c> holds data and logic for an Item that morphs depending on mood state.
    /// It holds different Item types where only one is active, depending on the mood state.
    /// </summary>
    ///
    public class MorphingItem : Item
    {
        // private MoodStateManager _moodStateManager;
        private Dictionary<MoodState, Item> _items;
        private Item _currentItem;

        public MorphingItem(Dictionary<MoodState, Item> items,
                             int id,
                             string name, 
                             bool pickUpAble, 
                             bool useAble, 
                             bool combineAble, 
                             bool giveAble, 
                             bool useWith, 
                             string examineText,
                             ThoughtNode thought,
                             MoodStateManager moodStateManager, 
                             Texture2D texture, Vector2 position, int drawOrder, bool collidable = true)
                             : base(id, name, pickUpAble, useAble, combineAble, giveAble, useWith, examineText, thought, moodStateManager, texture, position, drawOrder, collidable)
        {
            // _moodStateManager = moodStateManager;
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
            dataHolderEntity = (DataHolderMorphingItem)base.GetDataHolderEntity(dataHolderEntity);
            // Morphing Item
            Dictionary<MoodState, DataHolderEntity> dhItems = new Dictionary<MoodState, DataHolderEntity>();
            foreach(KeyValuePair<MoodState, Item> entry in _items)
            {
                dhItems.Add(entry.Key, entry.Value.GetDataHolderEntity());
            }
            dataHolderEntity.Items = dhItems;
            return dataHolderEntity;
        }

        public DataHolderEntity GetDataHolderEntity(DataHolderMorphingItem dataHolderEntity)
        {
            dataHolderEntity = (DataHolderMorphingItem)base.GetDataHolderEntity(dataHolderEntity);
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