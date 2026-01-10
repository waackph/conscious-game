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
        private RoomInteractionManager _roomInteractionManager;
        private SoCManager _socManager;
        private GameScreen _gameScreen;

        private bool _isHeartThrobDream = false;
        private bool _HeartThrobDreamHappend = false;
        private bool _isHeartThrobBasement = false;
        private bool _HeartThrobBasementHappend = false;

        private Song _throbHeartSong;
        private Song _standardSong;

        private Dictionary<int, float> throbSoundVolumeDream = new Dictionary<int, float>{
        { 17, 0.8f }, // living room
        { 13, 1.0f }, // corridor eg
        { 14, 1.2f }, // stairs eg
        { 15, 1.4f }, // stairs og
        { 16, 1.6f }, // childroom
        };
        private Dictionary<int, float> throbSoundVolumeBasement = new Dictionary<int, float>{
        { 8, 0.8f}, // childroom
        { 7, 0.8f }, // bedroom

        { 9, 1.0f }, // corridor og
        { 5, 1.0f }, // dining room
        { 4, 1.0f }, // living room

        { 3, 1.2f }, // corridor eg
        { 11, 1.2f }, // stairs og

        { 10, 1.4f }, // stairs eg
        { 12, 1.6f }, // storage room
        { 6, 1.8f }, // basement
        };

        public ScriptingProgress(GameScreen gameScreen, EntityManager entityManager, AudioManager audioManager, RoomInteractionManager roomInteractionManager, SoCManager socManager, ContentManager content)
        {
            _gameScreen = gameScreen;
            _audioManager = audioManager;
            _entityManager = entityManager;
            _roomInteractionManager = roomInteractionManager;
            _socManager = socManager;
            EventBus.Subscribe<RoomChangeEvent>(OnRoomChange);
            EventBus.Subscribe<SequenceFinishedEvent>(OnEventHappened);
            EventBus.Subscribe<StartGameEvent>(OnStartGame);
            EventBus.Subscribe<ContinueGameEvent>(OnContinueGame);
            EventBus.Subscribe<ThoughtEventTriggered>(OnThoughtEventTriggered);
            EventBus.Subscribe<ThoughtEventFinished>(OnThoughtEventFinished);

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
            _audioManager.SetSoundVolume(.1f);
        }

        private void OnContinueGame(object sender, ContinueGameEvent e)
        {
            _audioManager.PlayMusic(_standardSong);
            _audioManager.SetSoundVolume(.1f);
        }

        private void OnThoughtEventTriggered(object sender, ThoughtEventTriggered e)
        {
            // Phone ringing thought event
            if (e.ThoughtEventId == 1750)
            {
                _roomInteractionManager.isTriggerNewThoughtEnabled = false;
            }
            // Tell player to go to phone if they try to do something else while it's ringing
            // else if (!_roomInteractionManager.isTriggerNewThoughtEnabled)
            // {
            //     _socManager.AddThought(new ThoughtNode(1420, "Ich muss zuerst ans Handy gehen...", 0, true, 0));
            // }
        }

        private void OnThoughtEventFinished(object sender, ThoughtEventFinished e)
        {
            // Phone ringing thought event finished
            if (e.ThoughtEventId == 1750)
            {
                _roomInteractionManager.isTriggerNewThoughtEnabled = true;
            }
            // Final thought event to end the game
            else if (e.ThoughtEventId == 5862)
            {
                // finalize game
                _gameScreen.gameFinished = true;
            }
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
            checkHeartThrobBasementStart(e.sequenceCommand);
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
                _audioManager.SetSoundVolume(.1f);
                if (_entityManager.FlashlightOn)
                    _entityManager.ToggleFlashlight(); // turn off flashlight when waking up
            }
        }

        private void checkHeartThrobBasementStart(Command cmd)
        {
            // Check for Heart Throb Dream sequence trigger
            if (GlobalData.IsSameOrSubclass(typeof(WaitCommand), cmd.GetType())) {
                WaitCommand waitCmd = (WaitCommand)cmd;
                if (waitCmd.Sound.Name == "Audio/crash_porcelain" && !_isHeartThrobBasement && !_HeartThrobBasementHappend) // sequenceCommandThingId 6951 is shards of pot
                {
                    _isHeartThrobBasement = true;
                    _HeartThrobBasementHappend = true;
                    _audioManager.PlayMusic(_throbHeartSong);
                }
            }
        }

        private void checkHeartThrobBasementExit(int roomId)
        {
            if (_isHeartThrobBasement && roomId == 6) // roomId 6 is basement
            {
                _isHeartThrobBasement = false;
                _audioManager.PlayMusic(_standardSong);
                _audioManager.SetSoundVolume(.1f);
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
