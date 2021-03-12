using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    public class AnimatedSprite : Sprite
    {
        private int _atlasRows;
        private int _atlasCols;
        private int _currentFrame;
        private int _totalFrames;

        public AnimatedSprite(Texture2D texture, int rows, int columns, int width, int height, float rotation)
          :base(texture, width, height, rotation)
        {
            Texture = texture;
            _atlasRows = rows;
            _atlasCols = columns;
            _currentFrame = 0;
            _totalFrames = rows * columns;
            Width = width;
            Height = height;
        } 
        public override void Update()
        {
            _currentFrame++;
            if(_currentFrame == _totalFrames)
            {
                _currentFrame = 0;
            }    
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 location, SpriteEffects flip){
            int width = Texture.Width / _atlasCols;
            int height = Texture.Height / _atlasRows;
            int row = (int)((float)_currentFrame / (float)_atlasCols);
            int column = _currentFrame % _atlasCols;

            Rectangle textureSpaceRectangle = new Rectangle(width*column, height*row, width, height);
            Rectangle worldSpacesRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);

            spriteBatch.Draw(Texture, 
                             worldSpacesRectangle, 
                             textureSpaceRectangle, 
                             Color.White, 
                             0f,
                             new Vector2(Width/2, Height/2), 
                             flip, 
                             0f);
        }

        public void resetAnimation(){
            _currentFrame = 0;
        }
    }
}