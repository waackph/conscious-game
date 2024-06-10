using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

using System;
using System.Collections.Generic;

namespace conscious
{
    /// <summary>Class <c>AudioManager</c> implements a basic audio player system to play music and sounds.
    /// </summary>
    ///
    public class AudioManager : IComponent
    {
        List<Tuple<string, SoundEffectInstance>> soundEffects;
        // Mood Dependent change: get the PlayPosition and multiply/devide by the factor it was slowed down or fastened
        public AudioManager()
        {
            soundEffects = new List<Tuple<string, SoundEffectInstance>>();
            MediaPlayer.Volume = 0.2f;
        }

        public virtual void Update(GameTime gameTime){ }

        public virtual void Draw(SpriteBatch spriteBatch){ }

        public void PlayMusic(Song songFile, bool isRepeating = true)
        {
            if(songFile != null)
            {
                MediaPlayer.Play(songFile);
                MediaPlayer.IsRepeating = isRepeating;
            }
        }

        public Enum GetPlayerState()
        {
            return MediaPlayer.State;
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

        public void PlaySoundEffect(SoundEffect sound, bool looped)
        {
            SoundEffectInstance tmpSound = sound.CreateInstance();
            tmpSound.IsLooped = looped;
            tmpSound.Play();
            soundEffects.Add(new Tuple<string, SoundEffectInstance>(sound.Name, tmpSound));
        }

        public void StopSoundEffect(SoundEffect sound)
        {
            foreach(Tuple<string, SoundEffectInstance> instance in soundEffects)
            {
                if(instance.Item1 == sound.Name)
                {
                    instance.Item2.Stop();
                    instance.Item2.Dispose();
                    soundEffects.Remove(instance);
                    break;
                }
            }
        }
    }
}