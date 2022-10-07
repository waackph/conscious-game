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
        private int _timeSinceLastFrame = 0;
        private int _millisecondsPerFrame = 40;

        public AnimatedSprite(Texture2D texture, int rows, int columns, int width, int height, float rotation, int secPerFrame)
          :base(texture, width, height, rotation)
        {
            Texture = texture;
            _atlasRows = rows;
            _atlasCols = columns;
            _currentFrame = 0;
            _totalFrames = rows * columns;
            _millisecondsPerFrame = secPerFrame;
            Width = width;
            Height = height;
        } 
        public override void Update(GameTime gameTime)
        {
            _timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (_timeSinceLastFrame > _millisecondsPerFrame)
            {
                _timeSinceLastFrame = 0;
                _currentFrame++;
                if(_currentFrame == _totalFrames)
                {
                    _currentFrame = 0;
                }    
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

        public DataHolderAnimatedSprite GetDataHolderAnimatedSprite()
        {
            DataHolderAnimatedSprite dataHolderAnimatedSprite = new DataHolderAnimatedSprite();
            dataHolderAnimatedSprite.Texture = Texture.ToString();
            dataHolderAnimatedSprite.Rows = _atlasRows;
            dataHolderAnimatedSprite.Columns = _atlasCols;
            dataHolderAnimatedSprite.SecPerFrame = _millisecondsPerFrame;
            return dataHolderAnimatedSprite;
        }
        
        public DataHolderAnimatedSprite GetDataHolderAnimatedSprite(DataHolderAnimatedSprite dataHolderAnimatedSprite)
        {
            dataHolderAnimatedSprite.Texture = Texture.ToString();
            dataHolderAnimatedSprite.Rows = _atlasRows;
            dataHolderAnimatedSprite.Columns = _atlasCols;
            dataHolderAnimatedSprite.SecPerFrame = _millisecondsPerFrame;
            return dataHolderAnimatedSprite;
        }
    }
}