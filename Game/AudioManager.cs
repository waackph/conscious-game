using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

using System;

namespace conscious
{
    /// <summary>Class <c>AudioManager</c> implements a basic audio player system to play music and sounds.
    /// </summary>
    ///
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
                if(factor == 1d)
                {
                    MediaPlayer.Play(songFile);
                }
                else
                {
                    TimeSpan songPosition = MediaPlayer.PlayPosition;
                    songPosition = songPosition.Multiply(factor);
                    MediaPlayer.Play(songFile, songPosition);
                }
            }
        }
    }
}