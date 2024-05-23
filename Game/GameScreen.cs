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
        private RoomGraph _roomGraph;
        private AStarShortestPath _pathFinder;
        private Player _player;
        private Cursor _cursor;
        private int _preferredBackBufferWidth;
        private int _preferredBackBufferHeight;
        private Texture2D _pixel;

        public bool gameFinished = false;

        private static JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

        public GameScreen(int preferredBackBufferWidth, int preferredBackBufferHeight, Texture2D pixel, Cursor cursor, Vuerbaz game, GraphicsDevice graphicsDevice, ContentManager content, EventHandler screenEvent, EntityManager entityManager, MoodStateManager moodStateManager, AudioManager audioManager) 
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

            Vector2 playerPosition = new Vector2(1000, 150);  //Vector2.Zero;  // new Vector2(_preferredBackBufferWidth / 2, _preferredBackBufferHeight / 2 + _preferredBackBufferHeight*.35f);
            _player = new Player(content.Load<Texture2D>("Player/128_character_animation_walking_draft"),
                                 content.Load<Texture2D>("Player/128_character_animation_sleeping_draft"),
                                 50, 
                                 null,
                                 _moodStateManager,
                                 "Player",
                                 content.Load<Texture2D>("Player/128_character_animation_idle_draft"),
                                 playerPosition, 10);

            _controlsManager = new ControlsManager(_player);

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

            _socManager = new SoCManager(_moodStateManager);
            _uiDisplayThoughtManager = new UiDisplayThoughtManager(_entityManager, _moodStateManager, _socManager, _cursor, content.Load<SpriteFont>("Font/Hud"), _pixel);
            _uiDisplayThoughtManager.LoadContent(content.Load<Texture2D>("UI/debug_sprites/soc_background_main"),
                                                 content.Load<Texture2D>("UI/debug_sprites/soc_background_sub"));

            _dialogManager = new UiDialogManager(_entityManager, _moodStateManager, _player, cursor, content.Load<SpriteFont>("Font/Hud"), _pixel);

            _sequenceManager = new SequenceManager(_moodStateManager);

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
                                           _preferredBackBufferWidth, _preferredBackBufferHeight);

            _roomManager.TerminateGameEvent += SetTerminateGame;

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
        }

        public override void Update(GameTime gameTime)
        {
            if(Keyboard.GetState().IsKeyUp(Keys.Escape) && _lastKeyboardState.IsKeyDown(Keys.Escape))
            {
                _screenEvent.Invoke(this, new EventArgs());
            }

            if(gameFinished)
            {
                // TODO: Add logic to reinitilize game (if continue is clicked afterwards)
                _screenEvent.Invoke(this, new EventArgs());
            }

            if(!_dialogManager.DialogActive && !_sequenceManager.SequenceActive)
            {
                _inventoryManager.Update(gameTime);
                _roomInteractionManager.Update(gameTime);
                if(!_inventoryManager.InventoryActive)
                {
                    _controlsManager.Update(gameTime);
                    _uiDisplayThoughtManager.Update(gameTime);
                }
            }
            _dialogManager.Update(gameTime);
            _roomManager.Update(gameTime);
            _moodStateManager.Update(gameTime);
            _socManager.Update(gameTime);
            if(_sequenceManager.SequenceActive)
            {
                _sequenceManager.Update(gameTime);
            }

            if(EnteredScreen)
            {
                EnteredScreen = false;
                InitilizeEntityManager();
            }

            _entityManager.Update(gameTime);

            _lastKeyboardState = Keyboard.GetState();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _entityManager.Draw(spriteBatch);
        }

        public void SetTerminateGame(object sender, bool e)
        {
                gameFinished = true;
        }

        public override void InitilizeEntityManager()
        {
            _entityManager.AddEntity(_player);
            _entityManager.AddEntity(_cursor);
            _roomManager.currentRoom.FillEntityManager();
            if(_inventoryManager.InventoryActive)
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
            File.WriteAllText(savePath+"_inventory.json", JsonConvert.SerializeObject(_inventoryManager.GetDataHolderItems(), Formatting.Indented, settings));

            // Room Data
            File.WriteAllText(savePath+"_rooms.json", JsonConvert.SerializeObject(_roomManager.GetDataHolderRooms(), Formatting.Indented, settings));

            // Player Data
            File.WriteAllText(savePath+"_player.json", JsonConvert.SerializeObject(GetDataHolderPlayer(), Formatting.Indented, settings));
        }

        public void LoadGame(bool newGame)
        {
            string savePath;
            if(newGame)
            {
                savePath = "new_states/20240507-1200";
            }
            else
            {
                savePath = "save_states/20221007-1758";
            }

            // Clear Data
            _inventoryManager.ClearInventory();
            _roomManager.ClearRooms();

            // Inventory Data
            List<DataHolderEntity> inventoryData = JsonConvert.DeserializeObject<List<DataHolderEntity>>(File.ReadAllText(savePath+"_inventory.json"), settings);
            foreach(DataHolderEntity dataHolderEntity in inventoryData)
            {
                Thing entity = InstatiateEntity(dataHolderEntity);
                if(entity != null) //&& entity.GetType() == typeof(Item))
                {
                    _inventoryManager.AddItem((Item)entity);
                }
            }

            // Room Data
            Dictionary<int, DataHolderRoom> roomsData = JsonConvert.DeserializeObject<Dictionary<int, DataHolderRoom>>(File.ReadAllText(savePath+"_rooms_gct_extended.json"), settings);
            foreach(KeyValuePair<int, DataHolderRoom> entry in roomsData)
            {
                DataHolderRoom dhRoom = entry.Value;
                List<Thing> things = new List<Thing>();
                foreach(DataHolderEntity dhThing in dhRoom.Things)
                {
                    Thing entity = InstatiateEntity(dhThing);
                    if(entity != null)
                    {
                        things.Add(entity);
                    }
                }
                DataHolderSequence dhSequence = dhRoom.EntrySequence;
                Sequence entrySequence = InstatiateSequence(dhSequence);
                DataHolderThoughtNode dhThought =  dhRoom.Thought;
                ThoughtNode thought = InstatiateThought(dhThought);
                Room room = new Room(dhRoom.RoomWidth, _entityManager, entrySequence, 
                                     _content.Load<Song>("Audio/" + dhRoom.SoundFilePath), 
                                     _content.Load<Texture2D>(dhRoom.LightMapPath), thought,
                                     dhRoom.xLimStart, dhRoom.xLimEnd,
                                     dhRoom.yLimStart, dhRoom.yLimEnd);
                room.SetThings(things);
                _roomManager.AddRoom(entry.Key, room);
            }

            // Player data
            DataHolderPlayer playerData = JsonConvert.DeserializeObject<DataHolderPlayer>(File.ReadAllText(savePath+"_player.json"), settings);
            _roomManager.CurrentRoomIndex = GlobalData.InitRoomId;

            if(newGame)
            {
                _player.Position = new Vector2(playerData.PlayerPositionX, playerData.PlayerPositionY);
                _roomManager.ResetCurrentRoom();
            }
            else
                _player.Position = new Vector2(playerData.PlayerPositionX, playerData.PlayerPositionY);
        }

        public Sequence InstatiateSequence(DataHolderSequence dhSequence)
        {
            if(dhSequence == null)
                return null;
            List<Command> cmds = new List<Command>();
            foreach(DataHolderCommand dhCommand in dhSequence.Commands)
            {
                Command cmd = InstatiateCommand(dhCommand);                
                if(cmd != null)
                {
                    cmds.Add(cmd);
                }
            }
            return new Sequence(cmds);
        }
        
        public Command InstatiateCommand(DataHolderCommand dhCommand)
        {
            Command cmd;
            if(dhCommand.GetType() == typeof(DataHolderWaitCommand))
            {
                // TODO: Sound File not accessible as command needs to be fixed
                DataHolderWaitCommand dhWait = (DataHolderWaitCommand)dhCommand;
                WaitCommand wCmd = new WaitCommand(dhWait.MillisecondsToWait);
                wCmd = new WaitCommand(dhWait.MillisecondsToWait);
                wCmd.Sound = _content.Load<SoundEffect>("Audio/" + dhWait.CmdSoundFilePath);
                cmd = wCmd;
            }
            else if(dhCommand.GetType() == typeof(DataHolderWalkCommand))
            {
                DataHolderWalkCommand dhWalk = (DataHolderWalkCommand)dhCommand;
                cmd = new WalkCommand(dhWalk.DestinationX, dhWalk.DestinationY);
            }
            else if(dhCommand.GetType() == typeof(DataHolderDoorActionCommand))
            {
                DataHolderDoorActionCommand dhDoorAction = (DataHolderDoorActionCommand)dhCommand;
                int doorId = dhDoorAction.DoorId;
                cmd = new DoorActionCommand(_entityManager, doorId);
            }
            else if(dhCommand.GetType() == typeof(DataHolderAnimateCommand))
            {
                cmd = new AnimateCommand();
            }
            else if(dhCommand.GetType() == typeof(DataHolderSayCommand))
            {
                cmd = new SayCommand();
            }
            else if(dhCommand.GetType() == typeof(DataHolderVanishCommand))
            {
                cmd = new VanishCommand();
            }
            else 
            {
                cmd = null;
            }
            return cmd;
        }

        public Thing InstatiateEntity(DataHolderEntity dh)
        {
            Thing entity;
            if(dh.GetType() == typeof(DataHolderThing))
            {
                DataHolderThing dhThing = (DataHolderThing)dh;
                ThoughtNode thought = InstatiateThought(dhThing.Thought);
                entity = new Thing(dhThing.Id, thought, _moodStateManager, dhThing.Name, 
                                   _content.Load<Texture2D>(dhThing.texturePath), 
                                   new Vector2(dhThing.PositionX, dhThing.PositionY), dhThing.DrawOrder, 
                                   dhThing.Collidable, dhThing.CollisionBoxHeight);
            }
            else if(dh.GetType() == typeof(DataHolderItem))
            {
                DataHolderItem dhItem = (DataHolderItem)dh;
                ThoughtNode thought = InstatiateThought(dhItem.Thought);
                entity = new Item(dhItem.Id, dhItem.Name, 
                                  dhItem.PickUpAble, dhItem.UseAble, 
                                  dhItem.CombineAble, dhItem.GiveAble, 
                                  dhItem.UseWith, dhItem.ExamineText,
                                  thought, _moodStateManager, 
                                  _content.Load<Texture2D>(dhItem.texturePath), 
                                  new Vector2(dhItem.PositionX, dhItem.PositionY), dhItem.DrawOrder,
                                  dhItem.Collidable, dhItem.CollisionBoxHeight);
            }
            else if(dh.GetType() == typeof(DataHolderMorphingItem))
            {
                DataHolderMorphingItem dhMorph = (DataHolderMorphingItem)dh;
                Dictionary<MoodState, Item> items = new Dictionary<MoodState, Item>();
                ThoughtNode thought = InstatiateThought(dhMorph.Thought);
                foreach(KeyValuePair<MoodState, DataHolderEntity> entry in dhMorph.Items)
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
                                          dhMorph.Collidable, dhMorph.CollisionBoxHeight);
            }
            else if(dh.GetType() == typeof(DataHolderDoor))
            {
                DataHolderDoor dhDoor = (DataHolderDoor)dh;
                ThoughtNode thought = InstatiateThought(dhDoor.Thought);
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
                                  dhDoor.Collidable, dhDoor.CollisionBoxHeight);
            }
            else if(dh.GetType() == typeof(DataHolderKey))
            {
                DataHolderKey dhKey = (DataHolderKey)dh;
                ThoughtNode thought = InstatiateThought(dhKey.Thought);
                entity = new Key(dhKey.Id, dhKey.Name, 
                                 dhKey.PickUpAble, dhKey.UseAble, 
                                 dhKey.CombineAble, dhKey.GiveAble, 
                                 dhKey.UseWith, dhKey.ExamineText,
                                 dhKey.ItemDependency, thought, _moodStateManager, 
                                 _content.Load<Texture2D>(dhKey.texturePath), 
                                 new Vector2(dhKey.PositionX, dhKey.PositionY), dhKey.DrawOrder,
                                 dhKey.Collidable, dhKey.CollisionBoxHeight);
            }
            else if(dh.GetType() == typeof(DataHolderCombineItem))
            {
                DataHolderCombineItem dhCombinable = (DataHolderCombineItem)dh;
                Item combinedItem;
                if(dhCombinable.CombineItem == null)
                {
                    combinedItem = null;
                }
                else
                {
                    combinedItem = (Item)InstatiateEntity(dhCombinable.CombineItem);
                }
                ThoughtNode thought = InstatiateThought(dhCombinable.Thought);
                entity = new CombineItem(dhCombinable.Id, dhCombinable.Name, 
                                         dhCombinable.PickUpAble, dhCombinable.UseAble, 
                                         dhCombinable.CombineAble, dhCombinable.GiveAble, 
                                         dhCombinable.UseWith, dhCombinable.ExamineText,
                                         combinedItem, dhCombinable.ItemDependency, thought, _moodStateManager, 
                                         _content.Load<Texture2D>(dhCombinable.texturePath), 
                                         new Vector2(dhCombinable.PositionX, dhCombinable.PositionY), dhCombinable.DrawOrder,
                                         dhCombinable.Collidable, dhCombinable.CollisionBoxHeight);
            }
            else if(dh.GetType() == typeof(DataHolderCharacter))
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
            else if(dh.GetType() == typeof(DataHolderPuzzleCharacter))
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
            if(dhThought == null)
                return null;
            ThoughtNode thought = new ThoughtNode(dhThought.Id,
                                                    dhThought.Thought,
                                                    dhThought.LinkageId,
                                                    dhThought.IsRoot,
                                                    dhThought.ThingId);
            foreach(DataHolderThoughtLink dhThoughtLink in dhThought.Links)
            {
                ThoughtLink link = InstatiateThoughtLink(dhThoughtLink);                
                if(link != null)
                {
                    thought.AddLink(link);
                }
            }
            return thought;
        }

        public ThoughtLink InstatiateThoughtLink(DataHolderThoughtLink dhLink)
        {
            ThoughtLink link;
            if(dhLink.GetType() == typeof(DataHolderThoughtLink))
            {
                ThoughtNode thought = InstatiateThought(dhLink.NextNode);
                link = new ThoughtLink(dhLink.Id, 
                                        thought,
                                        dhLink.Option,
                                        dhLink.IsLocked,
                                        dhLink.ValidMoods);
            }
            else if(dhLink.GetType() == typeof(DataHolderFinalThoughtLink))
            {
                DataHolderFinalThoughtLink dhFinalLink = (DataHolderFinalThoughtLink)dhLink;
                ThoughtNode thought = InstatiateThought(dhFinalLink.NextNode);
                Sequence sequence = InstatiateSequence(dhFinalLink.sequence);
                ThoughtNode eventThought = InstatiateThought(dhFinalLink.EventThought);
                AnimatedSprite animation = null;
                if(dhFinalLink.Animation != null)
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
                                            eventThought);
            }
            else 
            {
                link = null;
            }
            return link;
        }
    }
}