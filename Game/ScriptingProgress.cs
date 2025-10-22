using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace conscious
{
    public class ScriptingProgress
    {
        private AudioManager _audioManager;
        private EntityManager _entityManager;

        private bool _isHeartThrobDream = false;
        private bool _HeartThrobDreamHappend = false;
        private bool _isHeartThrobBasement = false;
        private bool _HeartThrobBasementHappend = false;

        private Song _throbHeartSong;
        private Song _standardSong;

        private Dictionary<int, float> throbSoundVolumeDream = new Dictionary<int, float>{
        { 17, 0.6f }, // living room
        { 13, 0.8f }, // corridor eg
        { 14, 1.0f }, // stairs eg
        { 15, 1.2f }, // stairs og
        { 16, 1.4f }, // childroom
        };
        private Dictionary<int, float> throbSoundVolumeBasement = new Dictionary<int, float>{
        { 8, 0.6f}, // childroom
        { 7, 0.6f }, // bedroom

        { 9, 0.8f }, // corridor og
        { 5, 0.8f }, // dining room
        { 4, 0.8f }, // living room

        { 3, 1.0f }, // corridor eg
        { 11, 1.0f }, // stairs og

        { 10, 1.2f }, // stairs eg
        { 12, 1.4f }, // storage room
        { 6, 1.6f }, // basement
        };

        public ScriptingProgress(EntityManager entityManager, AudioManager audioManager, ContentManager content)
        {
            _audioManager = audioManager;
            _entityManager = entityManager;
            EventBus.Subscribe<RoomChangeEvent>(OnRoomChange);
            EventBus.Subscribe<SequenceFinishedEvent>(OnEventHappened);
            EventBus.Subscribe<StartGameEvent>(OnStartGame);

            _standardSong = content.Load<Song>("Audio/Red_Curtains");
            _throbHeartSong = content.Load<Song>("Audio/heartbeat_sound");
        }

        public void Update(GameTime gameTime)
        {
            // Currently no per-frame update logic needed
        }

        private void OnStartGame(object sender, StartGameEvent e)
        {
            _audioManager.PlayMusic(_standardSong);
            _audioManager.SetSoundVolume(1f);
        }

        private void OnRoomChange(object sender, RoomChangeEvent e)
        {
            checkHeartThrobDreamState(e.RoomId);
            if (_isHeartThrobBasement)
            {
                checkHeartThrobBasementExit(e.RoomId);
            }
            else if (_isHeartThrobDream)
            {
                updateHeartThrobSoundVolume(e.RoomId, throbSoundVolumeDream);
            }
            else if (_isHeartThrobBasement)
            {
                updateHeartThrobSoundVolume(e.RoomId, throbSoundVolumeBasement);
            }
        }

        private void OnEventHappened(object sender, SequenceFinishedEvent e)
        {
            checkHeartThrobBasementStart(e.sequenceCommandThingId);
        }

        private void checkHeartThrobDreamState(int roomId)
        {
            // Check for Heart Throb Dream sequence trigger
            if (roomId == 17 && !_isHeartThrobDream && !_HeartThrobDreamHappend) // roomId 17 is the dream living room
            {
                _isHeartThrobDream = true;
                _HeartThrobDreamHappend = true;
                _audioManager.PlayMusic(_throbHeartSong);
            }
            // Check for exiting Heart Throb Dream
            else if (_isHeartThrobDream && roomId == 4)
            {
                _isHeartThrobDream = false;
                _audioManager.PlayMusic(_standardSong);
                _audioManager.SetSoundVolume(1f);
            }
        }

        private void checkHeartThrobBasementStart(int sequenceCommandThingId)
        {
            // Check for Heart Throb Dream sequence trigger
            if (sequenceCommandThingId == 6951 && !_isHeartThrobBasement && !_HeartThrobBasementHappend) // sequenceCommandThingId 6951 is shards of pot
            {
                _isHeartThrobBasement = true;
                _HeartThrobBasementHappend = true;
                _audioManager.PlayMusic(_throbHeartSong);
            }
        }

        private void checkHeartThrobBasementExit(int roomId)
        {
            if (_isHeartThrobBasement && roomId == 6) // roomId 6 is basement
            {
                _isHeartThrobBasement = false;
                _audioManager.PlayMusic(_standardSong);
                _audioManager.SetSoundVolume(1f);
            }
        }

        private void updateHeartThrobSoundVolume(int roomId, Dictionary<int, float> throbSoundVolume)
        {
            if (throbSoundVolume.ContainsKey(roomId))
            {
                float volume = throbSoundVolume[roomId];
                _audioManager.SetSoundVolume(volume);
            }
            else
            {
                // throw new Exception("Room ID not found in volume dictionary");
                _audioManager.SetSoundVolume(0.1f); // default low volume
            }
        }
    }
}
