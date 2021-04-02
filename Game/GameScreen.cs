using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace conscious
{
    public class GameScreen : Screen
    {
        private KeyboardState _lastKeyboardState;
        private ControlsManager _controlsManager;
        private RoomManager _roomManager;
        private EntityManager _entityManager;
        private VerbManager _verbManager;
        private InventoryManager _inventoryManager;
        private DialogManager _dialogManager;
        private InteractionManager _interactionManager;
        private SequenceManager _sequenceManager;
        private MoodStateManager _moodStateManager;
        private Player _player;
        private Cursor _cursor;
        private int _preferredBackBufferWidth;
        private int _preferredBackBufferHeight;
        private Texture2D _pixel;

        private static JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

        public GameScreen(int preferredBackBufferWidth, int preferredBackBufferHeight, Texture2D pixel, Cursor cursor, Vuerbaz game, GraphicsDevice graphicsDevice, ContentManager content, EventHandler screenEvent, EntityManager entityManager) 
            : base(game, graphicsDevice, content, screenEvent)
        {
            // Initilize

            _preferredBackBufferHeight = preferredBackBufferHeight;
            _preferredBackBufferWidth = preferredBackBufferWidth;
            _pixel = pixel;

            _lastKeyboardState = Keyboard.GetState();

            Vector2 playerPosition = Vector2.Zero;  // new Vector2(_preferredBackBufferWidth / 2, _preferredBackBufferHeight / 2 + _preferredBackBufferHeight*.35f);
            _player = new Player(content.Load<Texture2D>("Player/Run"),
                                 null,
                                 "Player",
                                 content.Load<Texture2D>("Player/Idle"),
                                 playerPosition);

            _cursor = cursor;

            _entityManager = entityManager;
            _controlsManager = new ControlsManager(_player);
            _verbManager = new VerbManager(_entityManager);
            _verbManager.LoadContent(content.Load<Texture2D>("Verbs/debug/verb_background"),
                                     content.Load<Texture2D>("Verbs/debug/verb_examine"),
                                     content.Load<Texture2D>("Verbs/debug/verb_pick_up"),
                                     content.Load<Texture2D>("Verbs/debug/verb_use"),
                                     content.Load<Texture2D>("Verbs/debug/verb_combine"),
                                     content.Load<Texture2D>("Verbs/debug/verb_talk_to"),
                                     content.Load<Texture2D>("Verbs/debug/verb_give_to"));
            _inventoryManager = new InventoryManager(_entityManager);
            _inventoryManager.LoadContent(content.Load<Texture2D>("Inventory/debug/inventory_background"));
            _moodStateManager = new MoodStateManager(_entityManager);
            _dialogManager = new DialogManager(_entityManager, _moodStateManager, content.Load<SpriteFont>("Font/Hud"), _pixel);
            _sequenceManager = new SequenceManager();
            _roomManager = new RoomManager(content, _player, _cursor, entityManager, _dialogManager, _sequenceManager, _moodStateManager, _preferredBackBufferWidth, _preferredBackBufferHeight);
            _interactionManager = new InteractionManager(_player, _cursor, _controlsManager, _entityManager, _inventoryManager, _roomManager, _dialogManager, _moodStateManager);
        }

        public override void Update(GameTime gameTime)
        {
            if(Keyboard.GetState().IsKeyUp(Keys.Escape) && _lastKeyboardState.IsKeyDown(Keys.Escape))
            {
                _screenEvent.Invoke(this, new EventArgs());
            }

            _verbManager.Update(gameTime);
            _inventoryManager.Update(gameTime);
            if(!_dialogManager.DialogActive && !_sequenceManager.SequenceActive)
            {
                _controlsManager.Update(gameTime);
                _interactionManager.Update(gameTime);
            }
            _dialogManager.Update(gameTime);
            _roomManager.Update(gameTime);
            _moodStateManager.Update(gameTime);
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

        public override void InitilizeEntityManager()
        {
            _entityManager.AddEntity(_player);
            _entityManager.AddEntity(_cursor);
            _roomManager.currentRoom.FillEntityManager();
            _verbManager.FillEntityManager();
            _inventoryManager.FillEntityManager();
            _dialogManager.FillEntityManager();
        }

        public void SaveGame()
        {
            string date = DateTime.Now.ToString("yyyyMMdd-HHmm");
            string savePath = "save_states/" + date;

            // Inventory Data
            File.WriteAllText(savePath+"_inventory.json", JsonConvert.SerializeObject(_inventoryManager.GetDataHolderItems(), Formatting.Indented, settings));

            // Room Data
            File.WriteAllText(savePath+"_rooms.json", JsonConvert.SerializeObject(_roomManager.GetDataHolderRooms(), Formatting.Indented, settings));
        }

        // TODO: serialize data of player position and currentRoomIndex of the RoomManager
        public void LoadGame(bool newGame)
        {
            string savePath;
            if(newGame)
            {
                savePath = "new_states/20210313-2101";
            }
            else
            {
                savePath = "save_states/20210313-2110";
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
            Dictionary<int, DataHolderRoom> roomsData = JsonConvert.DeserializeObject<Dictionary<int, DataHolderRoom>>(File.ReadAllText(savePath+"_rooms.json"), settings);
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
                Room room = new Room(dhRoom.RoomWidth, _entityManager, null);
                room.SetThings(things);
                _roomManager.AddRoom(entry.Key, room);
            }
            _roomManager.ResetCurrentRoom();
        }

        public Thing InstatiateEntity(DataHolderEntity dh)
        {
            Thing entity;
            if(dh.GetType() == typeof(DataHolderThing))
            {
                DataHolderThing dhThing = (DataHolderThing)dh;
                entity = new Thing(dhThing.Thought, dhThing.Name, 
                                   _content.Load<Texture2D>(dhThing.texturePath), 
                                   new Vector2(dhThing.PositionX, dhThing.PositionY));
            }
            else if(dh.GetType() == typeof(DataHolderItem))
            {
                DataHolderItem dhItem = (DataHolderItem)dh;
                entity = new Item(dhItem.Id, dhItem.Name, 
                                  dhItem.PickUpAble, dhItem.UseAble, 
                                  dhItem.CombineAble, dhItem.GiveAble, 
                                  dhItem.UseWith, dhItem.ExamineText, dhItem.MoodChange,
                                  dhItem.Thought,
                                  _content.Load<Texture2D>(dhItem.texturePath), 
                                  new Vector2(dhItem.PositionX, dhItem.PositionY));
            }
            else if(dh.GetType() == typeof(DataHolderMorphingItem))
            {
                DataHolderMorphingItem dhMorph = (DataHolderMorphingItem)dh;
                Dictionary<MoodState, Item> items = new Dictionary<MoodState, Item>();
                foreach(KeyValuePair<MoodState, DataHolderEntity> entry in dhMorph.Items)
                {
                    items.Add(entry.Key, (Item)InstatiateEntity(entry.Value));
                }
                entity = new MorphingItem(_moodStateManager, items,
                                          dhMorph.Id, dhMorph.Name, dhMorph.PickUpAble,
                                          dhMorph.UseAble, dhMorph.CombineAble,
                                          dhMorph.GiveAble, dhMorph.UseWith, 
                                          dhMorph.ExamineText, dhMorph.MoodChange, dhMorph.Thought,
                                  _content.Load<Texture2D>(dhMorph.texturePath), 
                                  new Vector2(dhMorph.PositionX, dhMorph.PositionY));
            }
            else if(dh.GetType() == typeof(DataHolderDoor))
            {
                DataHolderDoor dhDoor = (DataHolderDoor)dh;
                entity = new Door(dhDoor.Id, dhDoor.Name, 
                                  dhDoor.PickUpAble, dhDoor.UseAble, 
                                  dhDoor.CombineAble, dhDoor.GiveAble, 
                                  dhDoor.UseWith, dhDoor.ExamineText, dhDoor.MoodChange,
                                  dhDoor.ItemDependency, dhDoor.RoomId, 
                                  dhDoor.IsUnlocked, dhDoor.Thought,
                                  _content.Load<Texture2D>(dhDoor.texturePath), 
                                  new Vector2(dhDoor.PositionX, dhDoor.PositionY));
            }
            else if(dh.GetType() == typeof(DataHolderKey))
            {
                DataHolderKey dhKey = (DataHolderKey)dh;
                entity = new Key(dhKey.Id, dhKey.Name, 
                                 dhKey.PickUpAble, dhKey.UseAble, 
                                 dhKey.CombineAble, dhKey.GiveAble, 
                                 dhKey.UseWith, dhKey.ExamineText, dhKey.MoodChange,
                                 dhKey.ItemDependency, dhKey.Thought,
                                 _content.Load<Texture2D>(dhKey.texturePath), 
                                 new Vector2(dhKey.PositionX, dhKey.PositionY));
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
                entity = new CombineItem(dhCombinable.Id, dhCombinable.Name, 
                                         dhCombinable.PickUpAble, dhCombinable.UseAble, 
                                         dhCombinable.CombineAble, dhCombinable.GiveAble, 
                                         dhCombinable.UseWith, dhCombinable.ExamineText, dhCombinable.MoodChange,
                                         combinedItem, dhCombinable.ItemDependency, dhCombinable.Thought,
                                         _content.Load<Texture2D>(dhCombinable.texturePath), 
                                         new Vector2(dhCombinable.PositionX, dhCombinable.PositionY));
            }
            else if(dh.GetType() == typeof(DataHolderCharacter))
            {
                DataHolderCharacter dhCharacter = (DataHolderCharacter)dh;
                entity = new Character(dhCharacter.Id, dhCharacter.Name, 
                                       dhCharacter.Pronoun, dhCharacter.CatchPhrase, 
                                       dhCharacter.GiveAble, dhCharacter.TreeStructure, 
                                       _dialogManager, dhCharacter.MoodChange, dhCharacter.Thought,
                                       _content.Load<Texture2D>(dhCharacter.texturePath), 
                                       new Vector2(dhCharacter.PositionX, dhCharacter.PositionY));
            }
            else if(dh.GetType() == typeof(DataHolderPuzzleCharacter))
            {
                DataHolderPuzzleCharacter dhPuzzleCharacter = (DataHolderPuzzleCharacter)dh;
                entity = new PuzzleCharacter(dhPuzzleCharacter.Id, dhPuzzleCharacter.Name, 
                                             dhPuzzleCharacter.Pronoun, dhPuzzleCharacter.CatchPhrase, 
                                             dhPuzzleCharacter.GiveAble, dhPuzzleCharacter.ItemDependency,
                                             dhPuzzleCharacter.DialogUnlocked, dhPuzzleCharacter.TreeStructure, 
                                             _dialogManager, dhPuzzleCharacter.MoodChange, dhPuzzleCharacter.Thought,
                                             _content.Load<Texture2D>(dhPuzzleCharacter.texturePath), 
                                             new Vector2(dhPuzzleCharacter.PositionX, dhPuzzleCharacter.PositionY));
            }
            else
            {
                entity = null;
            }
            return entity;
        }
    }
}