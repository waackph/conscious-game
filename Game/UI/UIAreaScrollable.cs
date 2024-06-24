using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Data;
using System;


namespace conscious
{
    public class UIAreaScrollable : UIArea
    {
        float _topPadding;
        float _offsetY;
        float _scrollOffset = 0;
        Cursor _cursor;
        float _scrollAmount;
        MouseState _lastMouseState;

        // TODO: This must be similar to the list reference of EntityManager
        public List<UIThought> UIComponents;

        public UIAreaScrollable(List<UIThought> uIcomponents, float topPadding, float offsetY,
                                Cursor cursor, float scrollAmount,
                                string name, Texture2D texture, Vector2 position, int drawOrder) 
                                : base(name, texture, position, drawOrder)
        {
            UIComponents = uIcomponents;
            _topPadding = topPadding;
            _offsetY = offsetY;
            _cursor = cursor;
            _scrollAmount = scrollAmount;
            _lastMouseState = Mouse.GetState();
        }

        public override Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)Position.X - Width/2, 
                                     (int)Position.Y - Height/2,
                                     Width, Height);
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Check and update scroll of components in Area
            // ManageUIAreaScroll();
            _lastMouseState = Mouse.GetState();
            base.Update(gameTime);
        }

        public void ManageUIAreaScroll()
        {
            if(UIComponents == null || UIComponents.Count == 0)
                return;
            _scrollOffset = 0;
            // UI Area where components are visible
            float uiStartYPos = Position.Y - Height/2 + _topPadding;
            float uiEndYPos = Position.Y + Height/2 - _offsetY;

            // Area where components are positioned (first and last component yield limits)
            float topScrollPos = UIComponents.First<UIComponent>().Position.Y;
            float bottomScrollPos = UIComponents.Last<UIComponent>().Position.Y + UIComponents.Last<UIComponent>().BoundingBox.Height;

            float scrollAreaHeight = bottomScrollPos - topScrollPos;
            float uiAreaHeight = uiEndYPos - uiStartYPos;

            bool enableScrolling = false;
            if(scrollAreaHeight > uiAreaHeight && BoundingBox.Intersects(_cursor.BoundingBox))
                enableScrolling = true;

            // doScrollDown
            if(Mouse.GetState().ScrollWheelValue < _lastMouseState.ScrollWheelValue && enableScrolling)
            {
                // if we are at bottom, do nothing
                if(topScrollPos + _scrollAmount < uiStartYPos)
                {
                    _scrollOffset += _scrollAmount;
                }
            }
            // doScrollUp
            else if(Mouse.GetState().ScrollWheelValue > _lastMouseState.ScrollWheelValue && enableScrolling)
            {
                // if we are at top, do nothing
                if(bottomScrollPos - _scrollAmount > uiEndYPos)
                {
                    _scrollOffset -= _scrollAmount;
                }
            }
            // TODO: Find out why translation matrix does only update texture/string but not boundingbox...
            // _entityManager.SetMaincomponentUITranslation(_scrollOffset);
            // For now, quickfix: update all positions of components directly
            foreach(UIComponent comp in UIComponents)
            {
                comp.Position = new Vector2(comp.Position.X, comp.Position.Y + _scrollOffset);
            }
        }

        public void ScrollToNewestUIComponent()
        {
            if(UIComponents == null || UIComponents.Count == 0)
                return;
            float topScrollPos = UIComponents.First<UIComponent>().Position.Y;
            float bottomScrollPos = UIComponents.Last<UIComponent>().Position.Y + UIComponents.Last<UIComponent>().BoundingBox.Height;
            float scrollAreaHeight = bottomScrollPos - topScrollPos;

            float uiStartYPos = Position.Y - Height/2 + _topPadding;
            float uiEndYPos = Position.Y + Height/2 - _offsetY;
            float uiAreaHeight = uiEndYPos - uiStartYPos;

            bool enableScrolling = false;
            if(scrollAreaHeight > uiAreaHeight)
                enableScrolling = true;
            if(enableScrolling)
            {
                // We need to calculate positions backwards, starting with last component, 
                // arranged so the boundingbox bottom is aligned with uiEndYPos
                // and then, simply substract each component position
                int nComponents = 0;
                float startingPosition = 0;
                float heightOffset = 0;
                for (int j = UIComponents.Count-1; j >= 0; j--)
                {
                    UIComponent component = UIComponents[j];
                    if(nComponents == 0)
                    {
                        startingPosition = uiEndYPos - component.BoundingBox.Height;
                        component.Position = new Vector2(component.Position.X, startingPosition);
                    }
                    else
                    {
                        heightOffset += component.BoundingBox.Height;
                        component.Position = new Vector2(component.Position.X, startingPosition - nComponents * _offsetY - heightOffset);
                    }
                    nComponents++;
                }
            }
        }
    }
}