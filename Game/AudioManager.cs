using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

using System;
// using System.TimeSpan;

namespace conscious
{
    public class AudioManager : IComponent
    {
        // Mood Dependent change: get the PlayPosition and multiply/devide by the factor it was slowed down or fastened
        public AudioManager(){ }

        public virtual void Update(GameTime gameTime){ }

        public virtual void Draw(SpriteBatch spriteBatch){ }

        public void PlayMusic(Song songFile)
        {
            if(songFile != null)
            {
                MediaPlayer.Play(songFile);
                MediaPlayer.IsRepeating = true;
            }
        }

        public void SwitchMusic(Song songFile, double factor)
        {
            if(songFile != null)
            {
                TimeSpan songPosition = MediaPlayer.PlayPosition;
                songPosition = songPosition.Multiply(factor);
                MediaPlayer.Play(songFile, songPosition*factor);
            }
        }
    }
}