using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    public abstract class UIComponent : Entity
    {
        public UIComponent(string name, Texture2D texture, Vector2 position, int drawOrder) 
        : base(name, texture, position, drawOrder)
        {
            FixedDrawPosition = true;
        }

    }
}