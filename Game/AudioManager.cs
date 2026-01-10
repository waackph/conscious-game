using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;


using System;
using System.Collections.Generic;
using System.Linq;

namespace conscious
{
    /// <summary>Class <c>AudioManager</c> implements a basic audio player system to play music and sounds.
    /// </summary>
    ///
    public class AudioManager : IComponent
    {
        List<Tuple<string, SoundEffectInstance>> soundEffects;
        public Song CurrentSong { get; private set; }

        private ContentManager _content;

        // Mood Dependent change: get the PlayPosition and multiply/devide by the factor it was slowed down or fastened
        public AudioManager(ContentManager content)
        {
            soundEffects = new List<Tuple<string, SoundEffectInstance>>();
            _content = content;
            MediaPlayer.Volume = 0.1f;
            MediaPlayer.ActiveSongChanged += OnActiveSongChanged;
            EventBus.Subscribe<MoodTransitionStartedEvent>(onMoodTransitionStarted);
        }

        private void OnActiveSongChanged(object sender, EventArgs e)
        {
            if (CurrentSong.Name == GlobalData.StandardSongSlowTransition.Split("/").Last())
            {
                PlayMusic(_content.Load<Song>(GlobalData.StandardSongSlow), true);
                MediaPlayer.Volume = 0.1f;
            }
            else if (CurrentSong.Name == GlobalData.StandardSongTransition.Split("/").Last())
            {
                PlayMusic(_content.Load<Song>(GlobalData.StandardSong), true);
                MediaPlayer.Volume = 0.1f;
            }
        }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(SpriteBatch spriteBatch) { }

        public void PlayMusic(Song songFile, bool isRepeating = true)
        {
            if (songFile != null)
            {
                CurrentSong = songFile;
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
            if (songFile != null)
            {
                CurrentSong = songFile;
                if (factor == 1d)
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

        // This is only used for sound effects that are looped and need to be managed/stopped later
        public void PlaySoundEffect(SoundEffect sound, bool looped)
        {
            SoundEffectInstance tmpSound = sound.CreateInstance();
            tmpSound.IsLooped = looped;
            tmpSound.Play();
            soundEffects.Add(new Tuple<string, SoundEffectInstance>(sound.Name, tmpSound));
        }

        public void StopSoundEffect(SoundEffect sound)
        {
            foreach (Tuple<string, SoundEffectInstance> instance in soundEffects)
            {
                if (instance != null && sound != null && instance.Item1 == sound.Name)
                {
                    instance.Item2.Stop();
                    instance.Item2.Dispose();
                    soundEffects.Remove(instance);
                    break;
                }
            }
        }

        public void SetSoundVolume(float volume)
        {
            MediaPlayer.Volume = volume;
        }

        private void onMoodTransitionStarted(object sender, MoodTransitionStartedEvent e)
        {
            MoodState moodState = e.CurrentMoodState;
            if ((CurrentSong.Name == GlobalData.StandardSong.Split("/").Last()
                 || CurrentSong.Name == GlobalData.StandardSongTransition.Split("/").Last())
                && moodState == MoodState.Depressed)
            {
                PlayMusic(_content.Load<Song>(GlobalData.StandardSongSlowTransition), false);
                MediaPlayer.Volume = 0.1f;
            }
            else if ((CurrentSong.Name == GlobalData.StandardSongSlow.Split("/").Last()
                     || CurrentSong.Name == GlobalData.StandardSongSlowTransition.Split("/").Last())
                    && moodState == MoodState.Regular)
            {
                PlayMusic(_content.Load<Song>(GlobalData.StandardSongTransition), false);
                MediaPlayer.Volume = 0.1f;
            }
        }
    }
}