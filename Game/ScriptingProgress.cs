using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace conscious
{
    public class ScriptingProgress
    {
        private AudioManager _audioManager;
        private EntityManager _entityManager;
        private RoomInteractionManager _roomInteractionManager;
        private SoCManager _socManager;
        private SequenceManager _sequenceManager;
        private MoodStateManager _moodStateManager;
        private GameScreen _gameScreen;
        private RoomManager _roomManager;
        private Player _player;

        private bool _isHeartThrobDream = false;
        private bool _HeartThrobDreamHappend = false;
        private bool _isHeartThrobBasement = false;
        private bool _HeartThrobBasementHappend = false;

        private Song _throbHeartSong;
        private Song _standardSong;
        private Song _startAtmoSound;

        private SoundEffect _openFrontDoorSound;
        private SoundEffect _turnOnLightSound;

        private Thing _blackOverlay;

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

        public ScriptingProgress(GameScreen gameScreen, EntityManager entityManager, AudioManager audioManager, RoomInteractionManager roomInteractionManager, SoCManager socManager, SequenceManager sequenceManager, MoodStateManager moodStateManager, RoomManager roomManager, ContentManager content, Player player)
        {
            _gameScreen = gameScreen;
            _audioManager = audioManager;
            _entityManager = entityManager;
            _roomInteractionManager = roomInteractionManager;
            _socManager = socManager;
            _sequenceManager = sequenceManager;
            _moodStateManager = moodStateManager;
            _roomManager = roomManager;
            _player = player;
            EventBus.Subscribe<StartGameEvent>(OnStartGame);
            // EventBus.Subscribe<StartTutorialEvent>(OnTutorialStarted);
            EventBus.Subscribe<RoomChangeEvent>(OnRoomChange);
            EventBus.Subscribe<SequenceFinishedEvent>(OnSequenceFinished);
            EventBus.Subscribe<ContinueGameEvent>(OnContinueGame);
            EventBus.Subscribe<ThoughtEventTriggered>(OnThoughtEventTriggered);
            EventBus.Subscribe<ThoughtFinishedEvent>(OnThoughtFinishedEvent);

            _standardSong = content.Load<Song>("Audio/Red_Curtains");
            _throbHeartSong = content.Load<Song>("Audio/heartbeat_sound");

            _startAtmoSound = content.Load<Song>("Audio/wind-and-seaguls"); // -muted

            _openFrontDoorSound = content.Load<SoundEffect>("Audio/open-and-close-door");
            _turnOnLightSound = content.Load<SoundEffect>("Audio/switch-on-light");

            Texture2D _blackOverlayTexture = content.Load<Texture2D>("light/dream_room_small_light_mask");
            _blackOverlay = new Thing(11, null, _moodStateManager, "Background", _blackOverlayTexture, new Vector2(_blackOverlayTexture.Width/2, _blackOverlayTexture.Height/2), 5);
        }

        public void Update(GameTime gameTime)
        {
            // Currently no per-frame update logic needed
        }

        private void OnStartGame(object sender, StartGameEvent e)
        {
            // add black overlay screen for tutorial
            _entityManager.AddEntity(_blackOverlay);
            // play atmo sound (muted sound of the start screen)
            _audioManager.PlayMusic(_startAtmoSound);
            _audioManager.SetSoundVolume(.01f);

            VanishCommand vanish = new VanishCommand();
            WaitCommand wait = new WaitCommand(2000);
            SayCommand firstLine = new SayCommand(_socManager, "Das Haus meiner Mutter.");
            WaitCommand wait2 = new WaitCommand(2000);
            SayCommand secondLine = new SayCommand(_socManager, "Das Haus meiner Kindheit.");
            WaitCommand wait3 = new WaitCommand(2000);
            List<Command> coms = new List<Command>()
            {
                vanish,
                wait,
                firstLine,
                wait2,
                secondLine,
                wait3
            };
            Sequence seq = new Sequence(coms, sequenceName: "StartTutorialSequence");
            _sequenceManager.StartSequence(seq, _player, MoodState.None);
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

        private void OnRoomChange(object sender, RoomChangeEvent e)
        {
            checkHeartThrobDreamState(e.RoomId);
            if (_isHeartThrobBasement)
            {
                checkHeartThrobBasementExit(e.RoomId);
            }
            if (_isHeartThrobDream)
            {
                updateHeartThrobSoundVolume(e.RoomId, throbSoundVolumeDream);
            }
            else if (_isHeartThrobBasement)
            {
                updateHeartThrobSoundVolume(e.RoomId, throbSoundVolumeBasement);
            }
        }

        private void OnSequenceFinished(object sender, SequenceFinishedEvent e)
        {
            // Check for tutorial sequence finish
            if (e.sequenceName == "StartTutorialSequence")
            {
                addTutorialThought();
            }

            // Check for tutorial ending sequence finish
            if (e.sequenceName == "FinishTutorialSequence")
            {
                endTutorial();
            }

            // Check for Heart Throb Dream sequence trigger
            if (GlobalData.IsSameOrSubclass(typeof(WaitCommand), e.sequenceCommand.GetType()))
            {
                checkHeartThrobBasementStart(e.sequenceCommand);
            }

            if (GlobalData.IsSameOrSubclass(typeof(WaitCommand), e.sequenceCommand.GetType()))
            {
                WaitCommand waitCmd = (WaitCommand)e.sequenceCommand;
                if (waitCmd != null && waitCmd.Sound != null && waitCmd.Sound.Name == "Audio/walking-stairs" && _roomManager.CurrentRoomIndex == 6)
                {
                    _gameScreen.gameFinished = true;
                }
                if (waitCmd != null && waitCmd._millisecondsToWait == 3210 && _roomManager.CurrentRoomIndex == 6)
                {
                    _gameScreen.gameFinished = true;
                }
            }
        }

        private void OnThoughtFinishedEvent(object sender, ThoughtFinishedEvent e)
        {
            if (e.RootThoughtId == 46) // final edge of tutorial thought
            {
                initLastTutorialSequence();
            }
            // Phone ringing thought event finished
            else if (e.RootThoughtId == 1750)
            {
                _roomInteractionManager.isTriggerNewThoughtEnabled = true;
            }
        }

        private void initLastTutorialSequence()
        {
            WaitCommand wait = new WaitCommand(3000);
            wait.Sound = _openFrontDoorSound;
            WaitCommand wait2 = new WaitCommand(300);
            wait2.Sound = _turnOnLightSound;
            List<Command> coms = new List<Command>()
            {
                wait,
                wait2
            };
            Sequence seq = new Sequence(coms, sequenceName: "FinishTutorialSequence");
            _sequenceManager.StartSequence(seq, _player, MoodState.None);
        }
        
        private void endTutorial()
        {
            // remove black screen overlay
            _entityManager.RemoveEntity(_blackOverlay);
            VanishCommand vanish = new VanishCommand();
            List<Command> coms = new List<Command>()
            {
                vanish,
            };
            Sequence seq = new Sequence(coms, sequenceName: "EndTutorialMarlaAppears");
            _sequenceManager.StartSequence(seq, _player, MoodState.None);
            // start playing normal music
            _audioManager.PlayMusic(_standardSong);
            _audioManager.SetSoundVolume(.1f);
            // disable tutorial flag
            _gameScreen.isTutorialActive = false;
            _roomManager.triggerThought();
        }

        private void addTutorialThought()
        {
            ThoughtNode innerThought2 = new ThoughtNode(49,
                "Hier denke ich über Dinge nach, reflektiere und treffe Entscheidungen über mein Handeln. Durch einen Klick auf Objekte lenke ich meine Aufmerksamkeit auf das Objekt. Ich bin hier um das Haus meiner verstorbenen Mutter zu entrümpeln.",
                0, false, 0);
            innerThought2.AddLink(new FinalThoughtLink(MoodState.None,
                Verb.None,
                null,
                null,
                0,
                55,
                null,
                "Dann mal los.",
                false,
                new MoodState[] { MoodState.None },
                true));
            innerThought2.AddLink(new FinalThoughtLink(MoodState.None,
                Verb.None,
                null,
                null,
                0,
                55,
                null,
                "Gar kein Bock drauf.",
                false,
                new MoodState[] { MoodState.None },
                false));
            ThoughtNode innerThought = new ThoughtNode(46,
                "[...] Das hier ist mein Gedankenprotokoll. Gedanken mit einem [...] am Anfang kennzeichnen einen inneren Dialog, der durch anklicken ausgelöst werden kann.",
                0, true, 30);
            innerThought.AddLink(new ThoughtLink(45,
                innerThought2,
                "First link",
                false,
                new MoodState[] { MoodState.None }));
            _socManager.AddThought(innerThought);
        }

        private void checkHeartThrobDreamState(int roomId)
        {
            // Check for Heart Throb Dream sequence trigger
            if (roomId == 17 && !_isHeartThrobDream && !_HeartThrobDreamHappend) // roomId 17 is the dream living room
            {
                _isHeartThrobDream = true;
                _HeartThrobDreamHappend = true;
                _audioManager.PlayMusic(_throbHeartSong);
                updateHeartThrobSoundVolume(roomId, throbSoundVolumeDream);
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
            WaitCommand waitCmd = (WaitCommand)cmd;
            if (waitCmd != null && waitCmd.Sound != null && waitCmd.Sound.Name == "Audio/crash_porcelain" && !_isHeartThrobBasement && !_HeartThrobBasementHappend) // sequenceCommandThingId 6951 is shards of pot
            {
                _isHeartThrobBasement = true;
                _HeartThrobBasementHappend = true;
                _audioManager.PlayMusic(_throbHeartSong);
                updateHeartThrobSoundVolume(_roomManager.CurrentRoomIndex, throbSoundVolumeBasement);
                // Add thought that says that there is the heartbeat sound again
                WaitCommand wait = new WaitCommand(2000);
                SayCommand sayHeartbeat = new SayCommand(_socManager, "Schon wieder dieses Pochen. Woher kommt es nur?");
                List<Command> coms = new List<Command>()
                {
                    wait,
                    sayHeartbeat,
                };
                Sequence seq = new Sequence(coms, sequenceName: "sayHeartbeatInBasement");
                _sequenceManager.StartSequence(seq, _player, MoodState.None);

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
