using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    public class UIInventoryPlace : UIComponent
    {
        public Item InventoryItem { set; get; }

        public UIInventoryPlace(string name, Texture2D texture, Vector2 position, int drawOrder) 
        : base(name, texture, position, drawOrder){
            InventoryItem = null;
            Collidable = true;
        }

        public override void Draw(SpriteBatch spriteBatch){
            // base.Draw(spriteBatch);
            spriteBatch.Draw(EntityTexture, 
                            Position,
                            null, Color.White, 0f, 
                            new Vector2(Width/2, Height/2),
                            Vector2.One,
                            SpriteEffects.None, 
                            0f);

            // If a texture is assigned to the inventory place
            if(InventoryItem != null){
                spriteBatch.Draw(InventoryItem.EntityTexture, 
                                Position,
                                null, Color.White, 0f, 
                                new Vector2(InventoryItem.Width/2, InventoryItem.Height/2),
                                new Vector2(2f, 2f), 
                                SpriteEffects.None, 
                                0f);
            }
        }
    }
}