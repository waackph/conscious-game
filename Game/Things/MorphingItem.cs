using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;

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
                             Texture2D texture, Vector2 position) : base(id, name, pickUpAble, useAble, combineAble, giveAble, useWith, examineText, texture, position)
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
                this.Name = item.Name;
                this.PickUpAble = item.PickUpAble;
                this.UseAble = item.UseAble;
                this.CombineAble = item.CombineAble;
                this.GiveAble = item.GiveAble;
                this.UseWith = item.UseWith;
                this.EntityTexture = item.EntityTexture;
                this.Position = item.Position;
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
    }
}