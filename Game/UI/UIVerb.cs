using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    public class UIVerb : UIComponent
    {
        private Sprite _verbSprite;
        public Verb VerbText { get; set; }

        public UIVerb(Verb verbText, Texture2D verbFont, string name, Texture2D texture, Vector2 position) : base(name, texture, position)
        {
            _verbSprite = new Sprite(verbFont, verbFont.Width, verbFont.Height, 0f);
            VerbText = verbText;
            Collidable = true;
        }

        public void DoAction(Thing thing)
        {
            if(thing != null){
                Console.WriteLine("---" + VerbText + " " + thing.ToString() + "---");
            }
            else{
                Console.WriteLine(VerbText);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            _verbSprite.Draw(spriteBatch, Position, SpriteEffects.None);
        }
    }
}
