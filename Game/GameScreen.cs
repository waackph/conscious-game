using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace conscious
{
    /// <summary>Class <c>GameScreen</c> implements the in-game screen.
    /// Here, the content of the game is initilized from the JSON files and everything in-game related is set up.
    /// </summary>
    ///
    public class GameScreen : Screen
    {
        private KeyboardState _lastKeyboardState;
        private ControlsManager _controlsManager;
        private RoomManager _roomManager;
        private EntityManager _entityManager;
        private MoodStateManager _moodStateManager;
        // private VerbManager _verbManager;
        private SoCManager _socManager;
        private InventoryManager _inventoryManager;
        private UiDialogManager _dialogManager;
        private UiDisplayThoughtManager _uiDisplayThoughtManager;
        private RoomInteractionManager _roomInteractionManager;
        private SequenceManager _sequenceManager;
        private AudioManager _audioManager;
        private ScriptingProgress _scriptingProgress;
        private RoomGraph _roomGraph;
        private AStarShortestPath _pathFinder;
        private Player _player;
        private Cursor _cursor;
        private int _preferredBackBufferWidth;
        private int _preferredBackBufferHeight;
        private Texture2D _pixel;

        private bool _gameLoaded = false;
        public bool gameFinished = false;
        private EventHandler _gameEndingScreenEvent;

        private static JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

        public GameScreen(int preferredBackBufferWidth, int preferredBackBufferHeight, Texture2D pixel,
                          Cursor cursor, Vuerbaz game, GraphicsDevice graphicsDevice,
                          ContentManager content, EventHandler screenEvent, EventHandler gameEndingScreenEvent,
                          EntityManager entityManager, MoodStateManager moodStateManager, AudioManager audioManager)
            : base(game, graphicsDevice, content, screenEvent)
        {
            // Initilize

            _preferredBackBufferHeight = preferredBackBufferHeight;
            _preferredBackBufferWidth = preferredBackBufferWidth;
            _pixel = pixel;

            _lastKeyboardState = Keyboard.GetState();

            _cursor = cursor;

            _entityManager = entityManager;

            _moodStateManager = moodStateManager;
            _audioManager = audioManager;

            _gameEndingScreenEvent = gameEndingScreenEvent;

            Vector2 playerPosition = new Vector2(1000, 150);  //Vector2.Zero;  // new Vector2(_preferredBackBufferWidth / 2, _preferredBackBufferHeight / 2 + _preferredBackBufferHeight*.35f);

            _player = new Player(content.Load<Texture2D>("Player/walking_anim_regular"),
                                 content.Load<Texture2D>("Player/sleep_anim_616_outline"),
                                 content.Load<Texture2D>("Player/marla_throwing_pot_outlined"),
                                 content.Load<Texture2D>("Player/idle_anim_411_outline"),
                                 content.Load<Texture2D>("Player/walking_anim_atlas"),
                                 50,
                                 null,
                                 _moodStateManager,
                                 "Player",
                                 content.Load<Texture2D>("Player/marla_regular_idle_outline"),
                                 playerPosition, 10);

            _controlsManager = new ControlsManager(_player, _entityManager);

            // _verbManager = new VerbManager(_entityManager);
            // _verbManager.LoadContent(content.Load<Texture2D>("Verbs/debug/verb_background"),
            //                          content.Load<Texture2D>("Verbs/debug/verb_examine"),
            //                          content.Load<Texture2D>("Verbs/debug/verb_pick_up"),
            //                          content.Load<Texture2D>("Verbs/debug/verb_use"),
            //                          content.Load<Texture2D>("Verbs/debug/verb_combine"),
            //                          content.Load<Texture2D>("Verbs/debug/verb_talk_to"),
            //                          content.Load<Texture2D>("Verbs/debug/verb_give_to"));

            _inventoryManager = new InventoryManager(_entityManager);
            _inventoryManager.LoadContent(content.Load<Texture2D>("UI/debug_sprites/inventory_place_background_v2"),
                                          content.Load<Texture2D>("UI/debug_sprites/inventory_background_v2"));

            _roomGraph = new RoomGraph();
            _pathFinder = new AStarShortestPath(_roomGraph);

            _socManager = new SoCManager(_moodStateManager, audioManager);
            _uiDisplayThoughtManager = new UiDisplayThoughtManager(_entityManager, _moodStateManager, _socManager, _cursor, content.Load<SpriteFont>(GlobalData.ThoughtFontName), _pixel);
            _uiDisplayThoughtManager.LoadContent(content.Load<Texture2D>("clear_out/UI/UI_Thought_Canvas_scaled_500x250"),
                                                 content.Load<Texture2D>("UI/debug_sprites/soc_background_sub_beige"),
                                                 content.Load<Texture2D>("UI/debug_sprites/inventory_place_background_v2"));

            _dialogManager = new UiDialogManager(_entityManager, _moodStateManager, _player, cursor, content.Load<SpriteFont>(GlobalData.ThoughtFontName), _pixel);

            _sequenceManager = new SequenceManager(_moodStateManager);

            SoundEffect defaultWalkingSound = content.Load<SoundEffect>("Audio/default_walking_sound");
            SoundEffectInstance defaultWalkingSoundInst = defaultWalkingSound.CreateInstance();
            defaultWalkingSoundInst.IsLooped = true;
            _roomManager = new RoomManager(content,
                                           _player,
                                           _cursor,
                                           _pixel,
                                           _entityManager,
                                           _dialogManager,
                                           _sequenceManager,
                                           _moodStateManager,
                                           _audioManager,
                                           _socManager,
                                           _roomGraph,
                                           _preferredBackBufferWidth, _preferredBackBufferHeight,
                                           defaultWalkingSoundInst
                                           );

            _roomInteractionManager = new RoomInteractionManager(_entityManager,
                                                                 _socManager,
                                                                 _inventoryManager,
                                                                 _controlsManager,
                                                                 _roomManager,
                                                                 _dialogManager,
                                                                 _sequenceManager,
                                                                 _pathFinder,
                                                                 _cursor,
                                                                 _player);

            _scriptingProgress = new ScriptingProgress(this, _entityManager, _audioManager, _roomInteractionManager, _socManager, content);
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyUp(Keys.Escape) && _lastKeyboardState.IsKeyDown(Keys.Escape))
            {
                _screenEvent.Invoke(this, new EventArgs());
            }

            if (gameFinished)
            {
                _gameEndingScreenEvent.Invoke(this, new EventArgs());
            }

            if (!_dialogManager.DialogActive && !_sequenceManager.SequenceActive)
            {
                _inventoryManager.Update(gameTime);
                _roomInteractionManager.Update(gameTime);
                if (!_inventoryManager.InventoryActive)
                {
                    _controlsManager.Update(gameTime);
                    _uiDisplayThoughtManager.Update(gameTime);
                }
            }
            _dialogManager.Update(gameTime);
            _roomManager.Update(gameTime);
            _moodStateManager.Update(gameTime);
            _socManager.Update(gameTime);
            _scriptingProgress.Update(gameTime);
            if (_sequenceManager.SequenceActive)
            {
                _sequenceManager.Update(gameTime);
            }

            if (EnteredScreen)
            {
                EnteredScreen = false;
                InitilizeEntityManager();
                if (_gameLoaded)
                {
                    EventBus.Publish(this, new ContinueGameEvent { });
                }
            }

            _entityManager.Update(gameTime);

            _lastKeyboardState = Keyboard.GetState();

            // After first update, mark game as loaded
            if (!_gameLoaded)
                _gameLoaded = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _entityManager.Draw(spriteBatch);
        }

        public override void InitilizeEntityManager()
        {
            _entityManager.AddEntity(_player);
            _entityManager.AddEntity(_cursor);
            _roomManager.currentRoom.FillEntityManager();
            if (_inventoryManager.InventoryActive)
            {
                _inventoryManager.FillEntityManager();
            }
            _dialogManager.FillEntityManager();
            _uiDisplayThoughtManager.FillEntityManager();
            _moodStateManager.FillEntityManager();
        }

        public DataHolderPlayer GetDataHolderPlayer()
        {
            DataHolderPlayer dataHolder = new DataHolderPlayer();
            dataHolder.PlayerPositionX = _player.Position.X;
            dataHolder.PlayerPositionY = _player.Position.Y;
            dataHolder.RoomId = _roomManager.CurrentRoomIndex;
            return dataHolder;
        }

        public void SaveGame()
        {
            string date = DateTime.Now.ToString("yyyyMMdd-HHmm");
            string savePath = "save_states/" + date;

            // Inventory Data
            File.WriteAllText(savePath + "_inventory.json", JsonConvert.SerializeObject(_inventoryManager.GetDataHolderItems(), Formatting.Indented, settings));

            // Room Data
            File.WriteAllText(savePath + "_rooms.json", JsonConvert.SerializeObject(_roomManager.GetDataHolderRooms(), Formatting.Indented, settings));

            // Player Data
            File.WriteAllText(savePath + "_player.json", JsonConvert.SerializeObject(GetDataHolderPlayer(), Formatting.Indented, settings));
        }

        public void LoadGame(bool newGame)
        {
            string savePath;
            if (newGame)
            {
                savePath = "new_states/20250820-1200";
            }
            else
            {
                savePath = "save_states/20221007-1758";
            }

            // Clear Data
            _inventoryManager.ClearInventory();
            _roomManager.ClearRooms();

            // Inventory Data
            List<DataHolderEntity> inventoryData = JsonConvert.DeserializeObject<List<DataHolderEntity>>(File.ReadAllText(savePath + "_inventory.json"), settings);
            foreach (DataHolderEntity dataHolderEntity in inventoryData)
            {
                Thing entity = InstatiateEntity(dataHolderEntity);
                if (entity != null) //&& entity.GetType() == typeof(Item))
                {
                    _inventoryManager.AddItem((Item)entity);
                }
            }

            // Room Data
            Dictionary<int, DataHolderRoom> roomsData = JsonConvert.DeserializeObject<Dictionary<int, DataHolderRoom>>(File.ReadAllText(savePath + "_rooms_gct_extended.json"), settings);
            foreach (KeyValuePair<int, DataHolderRoom> entry in roomsData)
            {
                DataHolderRoom dhRoom = entry.Value;
                List<Thing> things = new List<Thing>();
                foreach (DataHolderEntity dhThing in dhRoom.Things)
                {
                    Thing entity = InstatiateEntity(dhThing);
                    if (entity != null)
                    {
                        things.Add(entity);
                    }
                }
                DataHolderSequence dhSequence = dhRoom.EntrySequence;
                Sequence entrySequence = InstatiateSequence(dhSequence);
                DataHolderThoughtNode dhThought = dhRoom.Thought;
                ThoughtNode thought = InstatiateThought(dhThought);

                Song roomSong = null;
                if (dhRoom.SoundFilePath != null && dhRoom.SoundFilePath != "")
                    roomSong = _content.Load<Song>("Audio/" + dhRoom.SoundFilePath);
                SoundEffectInstance roomAtmoSound = null;
                if (dhRoom.AtmoSoundFilePath != null && dhRoom.AtmoSoundFilePath != "")
                {
                    roomAtmoSound = _content.Load<SoundEffect>("Audio/" + dhRoom.AtmoSoundFilePath).CreateInstance();
                    roomAtmoSound.IsLooped = true;
                }
                SoundEffectInstance roomWalkingSound = null;
                if (dhRoom.WalkingSoundFilePath != null && dhRoom.WalkingSoundFilePath != "")
                {
                    roomWalkingSound = _content.Load<SoundEffect>("Audio/" + dhRoom.WalkingSoundFilePath).CreateInstance();
                    roomWalkingSound.IsLooped = true;
                }

                if (dhRoom.PlayerScale == 0f)
                {
                    dhRoom.PlayerScale = 1f;
                }

                Room room = new Room(dhRoom.RoomWidth, _entityManager, entrySequence,
                                     _content.Load<Texture2D>(dhRoom.LightMapPath), thought,
                                     roomSong, roomAtmoSound, roomWalkingSound,
                                     dhRoom.xLimStart, dhRoom.xLimEnd,
                                     dhRoom.yLimStart, dhRoom.yLimEnd, dhRoom.PlayerScale);
                room.SetThings(things);
                _roomManager.AddRoom(entry.Key, room);
            }

            // Player data
            DataHolderPlayer playerData = JsonConvert.DeserializeObject<DataHolderPlayer>(File.ReadAllText(savePath + "_player.json"), settings);
            _roomManager.CurrentRoomIndex = GlobalData.InitRoomId;

            if (newGame)
            {
                _player.Position = new Vector2(playerData.PlayerPositionX, playerData.PlayerPositionY);
                _roomManager.ResetCurrentRoom();
                EventBus.Publish(this, new StartGameEvent
                {
                });
            }
            else
                _player.Position = new Vector2(playerData.PlayerPositionX, playerData.PlayerPositionY);
        }

        public Sequence InstatiateSequence(DataHolderSequence dhSequence)
        {
            if (dhSequence == null)
                return null;
            List<Command> cmds = new List<Command>();
            foreach (DataHolderCommand dhCommand in dhSequence.Commands)
            {
                Command cmd = InstatiateCommand(dhCommand);
                if (cmd != null)
                {
                    cmds.Add(cmd);
                }
            }
            return new Sequence(cmds, _roomManager);
        }

        public Command InstatiateCommand(DataHolderCommand dhCommand)
        {
            Command cmd;
            if (dhCommand.GetType() == typeof(DataHolderWaitCommand))
            {
                // TODO: Sound File not accessible as command needs to be fixed
                DataHolderWaitCommand dhWait = (DataHolderWaitCommand)dhCommand;
                WaitCommand wCmd = new WaitCommand(dhWait.MillisecondsToWait, dhWait.ThingId);
                wCmd = new WaitCommand(dhWait.MillisecondsToWait);
                if (dhWait.CmdSoundFilePath != null && dhWait.CmdSoundFilePath != "")
                    wCmd.Sound = _content.Load<SoundEffect>("Audio/" + dhWait.CmdSoundFilePath);
                cmd = wCmd;
            }
            else if (dhCommand.GetType() == typeof(DataHolderWalkCommand))
            {
                DataHolderWalkCommand dhWalk = (DataHolderWalkCommand)dhCommand;
                cmd = new WalkCommand(dhWalk.DestinationX, dhWalk.DestinationY, dhWalk.ThingId);
            }
            else if (dhCommand.GetType() == typeof(DataHolderDoorActionCommand))
            {
                DataHolderDoorActionCommand dhDoorAction = (DataHolderDoorActionCommand)dhCommand;
                int doorId = dhDoorAction.DoorId;
                cmd = new DoorActionCommand(_entityManager, doorId, dhDoorAction.ThingId);
            }
            else if (dhCommand.GetType() == typeof(DataHolderAnimateCommand))
            {
                DataHolderAnimateCommand dhAnim = (DataHolderAnimateCommand)dhCommand;
                Vector2 startPos = new Vector2(dhAnim.StartPositionX, dhAnim.StartPositionY);
                PlayerState animState = (PlayerState)Enum.Parse(typeof(PlayerState), convertFirstLetterToUpper(dhAnim.AnimState));
                float scaleSize = dhAnim.ScaleSize;
                cmd = new AnimateCommand(startPos, animState, scaleSize, dhAnim.ThingId);
            }
            else if (dhCommand.GetType() == typeof(DataHolderChangeRoomCommand))
            {
                DataHolderChangeRoomCommand dhChangeRoom = (DataHolderChangeRoomCommand)dhCommand;
                Vector2 startPos = new Vector2(dhChangeRoom.StartPositionX, dhChangeRoom.StartPositionY);
                PlayerState animState = (PlayerState)Enum.Parse(typeof(PlayerState), convertFirstLetterToUpper(dhChangeRoom.AnimState));
                int nextRoomId = dhChangeRoom.NextRoomId;
                cmd = new ChangeRoomCommand(startPos, animState, nextRoomId, _roomManager, dhChangeRoom.ThingId);
            }
            else if (dhCommand.GetType() == typeof(DataHolderSayCommand))
            {
                DataHolderSayCommand dhSay = (DataHolderSayCommand)dhCommand;
                cmd = new SayCommand(_socManager, dhSay.ThoughtText, dhSay.ThingId);
            }
            else if (dhCommand.GetType() == typeof(DataHolderVanishCommand))
            {
                DataHolderVanishCommand dhVanish = (DataHolderVanishCommand)dhCommand;
                cmd = new VanishCommand(dhVanish.ThingId);
            }
            else
            {
                cmd = null;
            }
            return cmd;
        }

        public Texture2D GetTextureFromDataHolder(string filePath, string prefixPath = "")
        {
            Texture2D tex = null;
            if (filePath != null && filePath != "")
                tex = _content.Load<Texture2D>(prefixPath + filePath);
            return tex;
        }

        public Thing InstatiateEntity(DataHolderEntity dh)
        {
            Thing entity;
            if (dh.GetType() == typeof(DataHolderThing))
            {
                DataHolderThing dhThing = (DataHolderThing)dh;
                ThoughtNode thought = InstatiateThought(dhThing.Thought);
                ThoughtNode eventThought = InstatiateThought(dhThing.EventThought);

                Texture2D depressedTexture = GetTextureFromDataHolder(dhThing.DepressedTexture);
                Texture2D manicTexture = GetTextureFromDataHolder(dhThing.ManicTexture);
                Texture2D lightMask = GetTextureFromDataHolder(dhThing.LightMaskFilePath, "light/");

                entity = new Thing(dhThing.Id, thought, _moodStateManager, dhThing.Name,
                                   _content.Load<Texture2D>(dhThing.texturePath),
                                   new Vector2(dhThing.PositionX, dhThing.PositionY), dhThing.DrawOrder,
                                   dhThing.Collidable, dhThing.CollisionBoxHeight, eventThought, lightMask,
                                   depressedTexture, manicTexture, dhThing.IsActive);
            }
            else if (dh.GetType() == typeof(DataHolderItem))
            {
                DataHolderItem dhItem = (DataHolderItem)dh;
                ThoughtNode thought = InstatiateThought(dhItem.Thought);
                ThoughtNode eventThought = InstatiateThought(dhItem.EventThought);

                Texture2D depressedTexture = GetTextureFromDataHolder(dhItem.DepressedTexture);
                Texture2D manicTexture = GetTextureFromDataHolder(dhItem.ManicTexture);
                Texture2D lightMask = GetTextureFromDataHolder(dhItem.LightMaskFilePath, "light/");

                SoundEffectInstance useSound = null;
                if (dhItem.UseSoundFilePath != null && dhItem.UseSoundFilePath != "")
                    useSound = _content.Load<SoundEffect>("Audio/" + dhItem.UseSoundFilePath).CreateInstance();

                entity = new Item(dhItem.Id, dhItem.Name,
                                  dhItem.PickUpAble, dhItem.UseAble,
                                  dhItem.CombineAble, dhItem.GiveAble,
                                  dhItem.UseWith, dhItem.ExamineText,
                                  thought, _moodStateManager,
                                  _content.Load<Texture2D>(dhItem.texturePath),
                                  new Vector2(dhItem.PositionX, dhItem.PositionY), dhItem.DrawOrder,
                                  dhItem.Collidable, dhItem.CollisionBoxHeight, eventThought,
                                  useSound, lightMask, depressedTexture, manicTexture, dhItem.IsActive);
            }
            else if (dh.GetType() == typeof(DataHolderMorphingItem))
            {
                DataHolderMorphingItem dhMorph = (DataHolderMorphingItem)dh;
                Dictionary<MoodState, Item> items = new Dictionary<MoodState, Item>();
                ThoughtNode thought = InstatiateThought(dhMorph.Thought);
                ThoughtNode eventThought = InstatiateThought(dhMorph.EventThought);
                foreach (KeyValuePair<MoodState, DataHolderEntity> entry in dhMorph.Items)
                {
                    items.Add(entry.Key, (Item)InstatiateEntity(entry.Value));
                }
                entity = new MorphingItem(items,
                                          dhMorph.Id, dhMorph.Name, dhMorph.PickUpAble,
                                          dhMorph.UseAble, dhMorph.CombineAble,
                                          dhMorph.GiveAble, dhMorph.UseWith,
                                          dhMorph.ExamineText, thought, _moodStateManager,
                                          _content.Load<Texture2D>(dhMorph.texturePath),
                                          new Vector2(dhMorph.PositionX, dhMorph.PositionY), dhMorph.DrawOrder,
                                          dhMorph.Collidable, dhMorph.CollisionBoxHeight, eventThought);
            }
            else if (dh.GetType() == typeof(DataHolderDoor))
            {
                DataHolderDoor dhDoor = (DataHolderDoor)dh;
                ThoughtNode thought = InstatiateThought(dhDoor.Thought);
                ThoughtNode eventThought = InstatiateThought(dhDoor.EventThought);

                Texture2D depressedTexture = GetTextureFromDataHolder(dhDoor.DepressedTexture);
                Texture2D manicTexture = GetTextureFromDataHolder(dhDoor.ManicTexture);
                Texture2D lightMask = GetTextureFromDataHolder(dhDoor.LightMaskFilePath, "light/");

                SoundEffectInstance useSound = null;
                if (dhDoor.UseSoundFilePath != null && dhDoor.UseSoundFilePath != "")
                    useSound = _content.Load<SoundEffect>("Audio/" + dhDoor.UseSoundFilePath).CreateInstance();
                SoundEffectInstance closeSound = null;
                if (dhDoor.CloseSoundFilePath != null && dhDoor.CloseSoundFilePath != "")
                    closeSound = _content.Load<SoundEffect>("Audio/" + dhDoor.CloseSoundFilePath).CreateInstance();

                entity = new Door(dhDoor.Id, dhDoor.Name,
                                  dhDoor.PickUpAble, dhDoor.UseAble,
                                  dhDoor.CombineAble, dhDoor.GiveAble,
                                  dhDoor.UseWith, dhDoor.ExamineText,
                                  dhDoor.ItemDependency, dhDoor.RoomId, dhDoor.DoorId,
                                  new Vector2(dhDoor.InitPlayerPosX, dhDoor.InitPlayerPosY),
                                  _content.Load<Texture2D>(dhDoor.CloseTexturePath),
                                  dhDoor.IsRoomChangeDoor,
                                  dhDoor.IsUnlocked, thought, _moodStateManager,
                                  _content.Load<Texture2D>(dhDoor.texturePath),
                                  new Vector2(dhDoor.PositionX, dhDoor.PositionY), dhDoor.DrawOrder,
                                  closeSound,
                                  dhDoor.Collidable, dhDoor.CollisionBoxHeight, eventThought, useSound,
                                  lightMask, depressedTexture, manicTexture, dhDoor.IsActive);
            }
            else if (dh.GetType() == typeof(DataHolderKey))
            {
                DataHolderKey dhKey = (DataHolderKey)dh;
                ThoughtNode thought = InstatiateThought(dhKey.Thought);
                ThoughtNode eventThought = InstatiateThought(dhKey.EventThought);

                Texture2D depressedTexture = GetTextureFromDataHolder(dhKey.DepressedTexture);
                Texture2D manicTexture = GetTextureFromDataHolder(dhKey.ManicTexture);
                Texture2D lightMask = GetTextureFromDataHolder(dhKey.LightMaskFilePath, "light/");

                entity = new Key(dhKey.Id, dhKey.Name,
                                 dhKey.PickUpAble, dhKey.UseAble,
                                 dhKey.CombineAble, dhKey.GiveAble,
                                 dhKey.UseWith, dhKey.ExamineText,
                                 dhKey.ItemDependency, thought, _moodStateManager,
                                 _content.Load<Texture2D>(dhKey.texturePath),
                                 new Vector2(dhKey.PositionX, dhKey.PositionY), dhKey.DrawOrder,
                                 dhKey.Collidable, dhKey.CollisionBoxHeight, eventThought,
                                 null, lightMask, depressedTexture, manicTexture, dhKey.IsActive);
            }
            else if (dh.GetType() == typeof(DataHolderCombineItem))
            {
                DataHolderCombineItem dhCombinable = (DataHolderCombineItem)dh;
                Item combinedItem;
                if (dhCombinable.CombineItem == null)
                {
                    combinedItem = null;
                }
                else
                {
                    combinedItem = (Item)InstatiateEntity(dhCombinable.CombineItem);
                }
                ThoughtNode thought = InstatiateThought(dhCombinable.Thought);
                ThoughtNode eventThought = InstatiateThought(dhCombinable.EventThought);

                Texture2D depressedTexture = GetTextureFromDataHolder(dhCombinable.DepressedTexture);
                Texture2D manicTexture = GetTextureFromDataHolder(dhCombinable.ManicTexture);
                Texture2D lightMask = GetTextureFromDataHolder(dhCombinable.LightMaskFilePath, "light/");

                entity = new CombineItem(dhCombinable.Id, dhCombinable.Name,
                                         dhCombinable.PickUpAble, dhCombinable.UseAble,
                                         dhCombinable.CombineAble, dhCombinable.GiveAble,
                                         dhCombinable.UseWith, dhCombinable.ExamineText,
                                         combinedItem, dhCombinable.ItemDependency, thought, _moodStateManager,
                                         _content.Load<Texture2D>(dhCombinable.texturePath),
                                         new Vector2(dhCombinable.PositionX, dhCombinable.PositionY), dhCombinable.DrawOrder,
                                         dhCombinable.Collidable, dhCombinable.CollisionBoxHeight, eventThought,
                                         null, lightMask, depressedTexture, manicTexture, dhCombinable.IsActive);
            }
            else if (dh.GetType() == typeof(DataHolderCharacter))
            {
                DataHolderCharacter dhCharacter = (DataHolderCharacter)dh;
                ThoughtNode thought = InstatiateThought(dhCharacter.Thought);
                entity = new Character(dhCharacter.Id, dhCharacter.Name,
                                       dhCharacter.Pronoun, dhCharacter.CatchPhrase,
                                       dhCharacter.GiveAble, dhCharacter.TreeStructure,
                                       _dialogManager, thought, _moodStateManager,
                                       _content.Load<Texture2D>(dhCharacter.texturePath),
                                       new Vector2(dhCharacter.PositionX, dhCharacter.PositionY), dhCharacter.DrawOrder,
                                       dhCharacter.Collidable, dhCharacter.CollisionBoxHeight);
            }
            else if (dh.GetType() == typeof(DataHolderPuzzleCharacter))
            {
                DataHolderPuzzleCharacter dhPuzzleCharacter = (DataHolderPuzzleCharacter)dh;
                ThoughtNode thought = InstatiateThought(dhPuzzleCharacter.Thought);
                entity = new PuzzleCharacter(dhPuzzleCharacter.Id, dhPuzzleCharacter.Name,
                                             dhPuzzleCharacter.Pronoun, dhPuzzleCharacter.CatchPhrase,
                                             dhPuzzleCharacter.GiveAble, dhPuzzleCharacter.ItemDependency,
                                             dhPuzzleCharacter.DialogUnlocked, dhPuzzleCharacter.TreeStructure,
                                             _dialogManager, thought, _moodStateManager,
                                             _content.Load<Texture2D>(dhPuzzleCharacter.texturePath),
                                             new Vector2(dhPuzzleCharacter.PositionX, dhPuzzleCharacter.PositionY),
                                             dhPuzzleCharacter.DrawOrder,
                                             dhPuzzleCharacter.Collidable, dhPuzzleCharacter.CollisionBoxHeight);
            }
            else
            {
                entity = null;
            }
            return entity;
        }

        public ThoughtNode InstatiateThought(DataHolderThoughtNode dhThought)
        {
            if (dhThought == null)
                return null;

            SoundEffect eventSound = null;
            if (dhThought.SoundPath != null && dhThought.SoundPath != "")
                eventSound = _content.Load<SoundEffect>(dhThought.SoundPath);

            Texture2D thoughtPortrait = null;
            if (dhThought.ThoughtPortrait != null && dhThought.ThoughtPortrait != "")
                thoughtPortrait = _content.Load<Texture2D>(dhThought.ThoughtPortrait);

            ThoughtNode thought = new ThoughtNode(dhThought.Id,
                                                    dhThought.Thought,
                                                    dhThought.LinkageId,
                                                    dhThought.IsRoot,
                                                    dhThought.ThingId,
                                                    eventSound,
                                                    dhThought.RepeatedSound,
                                                    thoughtPortrait);
            foreach (DataHolderThoughtLink dhThoughtLink in dhThought.Links)
            {
                ThoughtLink link = InstatiateThoughtLink(dhThoughtLink);
                if (link != null)
                {
                    thought.AddLink(link);
                }
            }
            return thought;
        }

        public ThoughtLink InstatiateThoughtLink(DataHolderThoughtLink dhLink)
        {
            ThoughtLink link;
            if (dhLink.GetType() == typeof(DataHolderThoughtLink))
            {
                ThoughtNode thought = InstatiateThought(dhLink.NextNode);
                link = new ThoughtLink(dhLink.Id,
                                        thought,
                                        dhLink.Option,
                                        dhLink.IsLocked,
                                        dhLink.ValidMoods);
            }
            else if (dhLink.GetType() == typeof(DataHolderFinalThoughtLink))
            {
                DataHolderFinalThoughtLink dhFinalLink = (DataHolderFinalThoughtLink)dhLink;
                ThoughtNode thought = InstatiateThought(dhFinalLink.NextNode);
                Sequence sequence = InstatiateSequence(dhFinalLink.sequence);
                // ThoughtNode eventThought = InstatiateThought(dhFinalLink.EventThought);
                AnimatedSprite animation = null;
                if (dhFinalLink.Animation != null)
                {
                    DataHolderAnimatedSprite dhAnimation = dhFinalLink.Animation;
                    Texture2D texture = _content.Load<Texture2D>(dhAnimation.Texture);
                    animation = new AnimatedSprite(texture,
                                                   dhAnimation.Rows, dhAnimation.Columns,
                                                   texture.Width, texture.Height, 0f,
                                                   dhAnimation.SecPerFrame);
                }
                link = new FinalThoughtLink(dhFinalLink.moodChange,
                                            dhFinalLink.verb,
                                            animation,
                                            sequence,
                                            dhFinalLink.UnlockId,
                                            dhFinalLink.Id,
                                            thought,
                                            dhFinalLink.Option,
                                            dhFinalLink.IsLocked,
                                            dhFinalLink.ValidMoods,
                                            dhFinalLink.IsSuccessEdge,
                                            dhFinalLink.EventThoughtId);
            }
            else
            {
                link = null;
            }
            return link;
        }
        private string convertFirstLetterToUpper(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}