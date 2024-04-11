using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;

namespace conscious
{
    /// <summary>Class <c>EntityManager</c> implements an inventory system.
    /// If the player picks some Item up, it is shown and interactable via the Inventory.
    /// </summary>
    ///
    public class InventoryManager : IComponent
    {
        private float _marginWidth = 60f;
        private float _marginHeight = 20f;
        private int _startWidth = 1430;
        private int _startHeight = 1050;
        private int _nrSlotRows =  4;
        private int _nrSlotCols = 4;
        private KeyboardState _lastKeyboardState;
        private List<UIInventoryPlace> _slots;
        private UIArea _inventoryBackground;
        private List<Item> _items = new List<Item>();

        private EntityManager _entityManager;

        public bool InventoryActive { get; set; }

        public InventoryManager(EntityManager entityManager)
        {
            _entityManager = entityManager;
            _slots = new List<UIInventoryPlace>();
            InventoryActive = false;
        }

        public void LoadContent(Texture2D inventoryPlaceTexture, Texture2D inventoryBackground)
        {
            Vector2 bgPosition = new Vector2(780, 500);
            _inventoryBackground = new UIArea("Inventory Background", inventoryBackground, bgPosition, 1);
            for(int iHeight=_nrSlotRows; iHeight>=1; iHeight--)
            {
                for(int iWidth=_nrSlotCols; iWidth>=1; iWidth--)
                {
                    Vector2 placePosition = new Vector2(_startWidth - inventoryPlaceTexture.Width*iWidth - _marginWidth*iWidth, 
                                                        _startHeight - inventoryPlaceTexture.Height*iHeight - _marginHeight*iHeight);
                    UIInventoryPlace place = new UIInventoryPlace(iHeight.ToString()+"x"+iWidth.ToString(),
                                                                  inventoryPlaceTexture, 
                                                                  placePosition, 1);
                    place.UpdateDrawOrder(2);
                    _slots.Add(place);
                }
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            if(Keyboard.GetState().IsKeyUp(Keys.Tab) && _lastKeyboardState.IsKeyDown(Keys.Tab))
            {
                if(InventoryActive)
                {
                    CloseInventory();
                }
                else
                {
                    ShowInventory();
                }
            }
            _lastKeyboardState = Keyboard.GetState();
        }

        public virtual void Draw(SpriteBatch spriteBatch){ }

        public void AddItem(Item item)
        {
            item.IsInInventory = true;
            _items.Add(item);
            foreach(UIInventoryPlace inventoryPlace in _slots)
            {
                if (inventoryPlace.InventoryItem == null)
                {
                    inventoryPlace.InventoryItem = item;
                    break;
                }
            }
        }

        public void DeleteItem(Item item)
        {
            _items.Remove(item);
            foreach(UIInventoryPlace inventoryPlace in _slots)
            {
                if (inventoryPlace.InventoryItem == item)
                {
                    inventoryPlace.InventoryItem = null;
                    break;
                }
            }
        }

        public void ClearInventory()
        {
            foreach(Item item in _items)
            {
                foreach(UIInventoryPlace inventoryPlace in _slots)
                {
                    if (inventoryPlace.InventoryItem == item)
                    {
                        inventoryPlace.InventoryItem = null;
                        break;
                    }
                }
            }
            _items.Clear();
        }

        public void FillEntityManager()
        {
            _entityManager.AddEntity(_inventoryBackground);
            foreach(UIInventoryPlace slot in _slots)
            {
                _entityManager.AddEntity(slot);
            }
        }

        public void ShowInventory()
        {
            FillEntityManager();
            InventoryActive = true;
        }

        public void CloseInventory()
        {
            _entityManager.RemoveEntity(_inventoryBackground);
            foreach(UIInventoryPlace slot in _slots)
            {
                _entityManager.RemoveEntity(slot);
            }
            InventoryActive = false;
        }

        public List<Item> GetItems()
        {
            return _items;
        }

        public void SetItems(List<Item> items)
        {
            _items = items;
        }

        public List<DataHolderEntity> GetDataHolderItems()
        {
            List<DataHolderEntity> items = new List<DataHolderEntity>();
            if(_items.Count > 0)
            {
                foreach(Item item in _items)
                {
                    items.Add(item.GetDataHolderEntity());
                }
            }
            return items;
        }
    }
}