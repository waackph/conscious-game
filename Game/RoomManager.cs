using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using System.Collections.Generic;

namespace conscious
{
    public class RoomManager : IComponent
    {
        private Dictionary<int, Room> _rooms = new Dictionary<int, Room>();
        private EntityManager _entityManager;
        private UiDialogManager _dialogManager;
        private SequenceManager _sequenceManager;
        private MoodStateManager _moodStateManager;
        private ContentManager _content;
        private int _preferredBackBufferWidth;
        private int _preferredBackBufferHeight;
        private Vector2 _centerPosition;
        private Player _player;
        private Cursor _cursor;
        private Texture2D _pixel;
        private int _currentRoomIndex;

        public Room currentRoom;

        public RoomManager(ContentManager content, 
                           Player player,
                           Cursor cursor,
                           Texture2D pixel,
                           EntityManager entityManager,
                           UiDialogManager dialogManager,
                           SequenceManager sequenceManager,
                           MoodStateManager moodStateManager,
                           int preferredBackBufferWidth, 
                           int preferredBackBufferHeight)
        {
            _content = content;

            _preferredBackBufferHeight = preferredBackBufferHeight;
            _preferredBackBufferWidth = preferredBackBufferWidth;

            _centerPosition = new Vector2(_preferredBackBufferWidth / 2,
                                         _preferredBackBufferHeight / 2);
            
            _entityManager = entityManager;
            _dialogManager = dialogManager;
            _sequenceManager = sequenceManager;
            _moodStateManager = moodStateManager;

            _player = player;
            _cursor = cursor;

            _pixel = pixel;

            _currentRoomIndex = 1;

            LoadRooms();
        }
        
        public void LoadRooms(){
            Vector2 itemPosition;
            
            // Room 1
            Texture2D bg = _content.Load<Texture2D>("Backgrounds/debug/background");
            Room room = new Room(bg.Width, _entityManager, null);
            Thing thing = new Thing(10, null, "Background", bg, new Vector2(bg.Width/2, bg.Height/2));
            room.addThing(thing);

            itemPosition = new Vector2(1058, 570);
            // UIThought thought = new UIThought(_content.Load<SpriteFont>("Font/Hud"),
            //                                   "The door to the outside world. \nI am not ready for this.", 
            //                                   "doorthought",
            //                                   _pixel, Vector2.One);

            ThoughtNode thought2 = new ThoughtNode(16, "First node", 0, false);
            thought2.AddLink(new ThoughtLink(13, 
                                            null, 
                                            "Fuck it. I'll do it anyway [use]", 
                                            false, 
                                            new MoodState[] {MoodState.None}));
            thought2.AddLink(new ThoughtLink(14, 
                                            null, 
                                            "I'll rather feel alone then [leave]", 
                                            false,
                                            new MoodState[] {MoodState.None}));
            ThoughtNode thought = new ThoughtNode(12, "The door to the outside world. \nI am not ready for this.", 0, true);
            thought.AddLink(new ThoughtLink(15,
                                            thought2,
                                            "First link",
                                            false,
                                            new MoodState[] {MoodState.None}));
            Thing door = new Door(1, "Door", false, true, false, false, true, "It's a door", 
                                  MoodState.None, 4, 2, false, thought, _content.Load<Texture2D>("Objects/debug/door_closed"), itemPosition);
            room.addThing(door);

            itemPosition = new Vector2(448, 786+4);
            Key combinedItem = new Key(4, "Oily Key", true, true, false, false, true, "The key is smooth now", MoodState.None,
                                         1, null, _content.Load<Texture2D>("Objects/debug/key_oily"), Vector2.Zero);

            Thing key = new CombineItem(2, "Key", true, false, true, true, false, "It's a key", MoodState.Regular, 
                                        combinedItem, 3, null, _content.Load<Texture2D>("Objects/debug/key"), itemPosition);
            room.addThing(key);

            itemPosition = new Vector2(200, 786+4);
            Thing combineItem = new CombineItem(3, "Oil Bottle", true, false, true, false, false, "It's a bottle", MoodState.Depressed, 
                                                null, 2, null, _content.Load<Texture2D>("Objects/debug/oil_bottle"), itemPosition);
            room.addThing(combineItem);

            // --------------------------- Morphing Item ---------------------------
            itemPosition = new Vector2(1058, 800);
            Dictionary<MoodState, Item> morphItems = new Dictionary<MoodState, Item>();
            Key morphItem1 = new Key(4, "Oily Key", true, true, false, false, true, "The key is smooth now", MoodState.None, 1, null,
                                       _content.Load<Texture2D>("Objects/debug/key_oily"), itemPosition);
            Key morphItem2 = new Key(4, "Oil Bottle", true, true, false, false, true, "The key is smooth now", MoodState.None, 1, null, 
                                       _content.Load<Texture2D>("Objects/debug/oil_bottle"), itemPosition);
            morphItems[MoodState.Regular] = morphItem1;
            morphItems[MoodState.Depressed] = morphItem2;
            Thing morph = new MorphingItem(_moodStateManager, morphItems, 
                                           6, "Morph", false, true, false, false, true, "It's morphing", MoodState.None, null,
                                           _content.Load<Texture2D>("Objects/debug/oil_bottle"), itemPosition);
            room.addThing(morph);

            _rooms.Add(1, room);

            // room 2
            bg = _content.Load<Texture2D>("Backgrounds/debug/background_double");
            room = new Room(bg.Width, _entityManager, null);
            Thing background = new Thing(11, null, "Background", bg, new Vector2(bg.Width/2, bg.Height/2));
            room.addThing(background);

            itemPosition = new Vector2(1058, 575+4);
            door = new Door(1, "Door", false, true, false, false, false, "It's a door", MoodState.None, 2, 1, true, null,
                                  _content.Load<Texture2D>("Objects/debug/door_opened"), itemPosition);
            room.addThing(door);
            
            itemPosition = new Vector2(1400, 786+4);
            // Start Initilizing dialog tree
            List<Node> dialogTree = new List<Node>();
            List<Edge> edges = new List<Edge>();
            edges.Add(new Edge(2, "True dat!", MoodState.None));
            edges.Add(new Edge(3, "Whatever.", MoodState.None));
            Node root = new Node(1, "Bib bup bip", edges);
            dialogTree.Add(root);
            edges = new List<Edge>();
            edges.Add(new Edge(4, "Tell me more", MoodState.None));
            Node node2 = new Node(2, "Bab bip", edges);
            dialogTree.Add(node2);
            edges = new List<Edge>();
            edges.Add(new Edge(4, "Like I said, whatever.", MoodState.None));
            Node node3 = new Node(3, "Bub bap", edges);
            dialogTree.Add(node3);
            edges = new List<Edge>();
            edges.Add(new Edge(0, "It was nice to talk to you too Sir.", MoodState.None));
            Node node4 = new Node(4, "Bib bip bup bip", edges);
            dialogTree.Add(node4);
            // End Initilizing dialog tree
            Thing character = new Character(5, "Robo", "Hen", "Bib bup bip", true, dialogTree, _dialogManager, 
                                            MoodState.None, null,
                                            _content.Load<Texture2D>("NPCs/debug/npc"), itemPosition);
            room.addThing(character);

            _rooms.Add(2, room);
        }

        public void changeRoom(int roomId)
        {
            if(currentRoom != null)
                currentRoom.ClearRoomEntityManager();
            currentRoom = _rooms[roomId];
            float xPos = 0f;
            Matrix transform = Matrix.CreateTranslation(xPos, 0, 0);
            _entityManager.ViewportTransformation = transform;
            _cursor.InverseTransform = Matrix.Invert(transform);
            // foreach(Thing thing in currentRoom.GetThings())
            // {
            //     thing.XPosOffset = 0;
            // }
            currentRoom.FillEntityManager();
            if(currentRoom.EntrySequence != null && !currentRoom.EntrySequence.SequenceFinished)
                _sequenceManager.StartSequence(currentRoom.EntrySequence);
        }

        public void Update(GameTime gameTime){

            if(currentRoom == null)
            {
                // Testing: Sequence
                _player.Position = new Vector2(10, 786);
                WalkCommand command = new WalkCommand(_player, new Vector2(1000, 786));
                List<Command> coms = new List<Command>()
                {
                    command
                };
                Sequence seq = new Sequence(coms);
                _rooms[_currentRoomIndex].EntrySequence = seq;

                // currentRoom = _rooms[_currentRoomIndex];
                changeRoom(_currentRoomIndex);
            }
 
            // Scroll room and thing positions
            if(currentRoom.RoomWidth != _preferredBackBufferWidth)
            {
                ScrollRoom();
            }

            LimitRoom();

            if(currentRoom.checkBoundingBoxes(_player.BoundingBox))
            {
                _player.Position = _player.LastPosition;
            }
            
            foreach(Door door in _entityManager.GetEntitiesOfType<Door>())
            {
                if(door.currentlyUsed == true)
                {
                    door.currentlyUsed = false;
                    changeRoom(door.RoomId);
                    break;
                }
            }
        }

        public void LimitRoom()
        {
            int roomEnding = currentRoom.RoomWidth;
            if(_player.Position.X > roomEnding - _player.Width / 2)
                _player.Position.X = roomEnding - _player.Width / 2;
            else if(_player.Position.X < _player.Width / 2)
                _player.Position.X = _player.Width / 2;
            if(_player.Position.Y > _preferredBackBufferHeight - _player.Height)
                _player.Position.Y = _preferredBackBufferHeight - _player.Height;
            else if(_player.Position.Y < _player.Height / 2f + _preferredBackBufferHeight * .64f)
                _player.Position.Y = _player.Height / 2f + _preferredBackBufferHeight *.64f;
        }

        public void ScrollRoom()
        {
            float horizontalMiddleBegin = (float)_preferredBackBufferWidth/2f;
            float horizontalMiddleEnd = (float)currentRoom.RoomWidth - horizontalMiddleBegin;
            if(_player.Position.X > horizontalMiddleBegin && _player.Position.X < horizontalMiddleEnd)
            {
                float xPos = -_player.Position.X+horizontalMiddleBegin;
                Matrix transform = Matrix.CreateTranslation(xPos, 0, 0);
                _entityManager.ViewportTransformation = transform;
                _cursor.InverseTransform = Matrix.Invert(transform);
                // _player.XPosOffset = (int)xPos;
                
                // foreach(Thing thing in _entityManager.GetEntitiesOfType<Thing>())
                // {
                //     if(!thing.FixedDrawPosition)
                //         thing.XPosOffset = (int)xPos;
                // }
            }
        }

        public void Draw(SpriteBatch spriteBatch){ }

        public void AddRoom(int index, Room room)
        {
            _rooms.Add(index, room);
        }

        public void ClearRooms()
        {
            _rooms.Clear();
        }

        public void SetCurrentRoomIndex(int roomIndex)
        {
            _currentRoomIndex = roomIndex;
        }

        public void ResetCurrentRoom()
        {
            // currentRoom = _rooms[_currentRoomIndex];
            
            // Testing: Sequence
            _player.Position = new Vector2(10, 900);
            WalkCommand command = new WalkCommand(_player, new Vector2(1000, 900));
            List<Command> coms = new List<Command>()
            {
                command
            };
            Sequence seq = new Sequence(coms);
            _rooms[_currentRoomIndex].EntrySequence = seq;

            // currentRoom = _rooms[_currentRoomIndex];
            changeRoom(_currentRoomIndex);

        }

        public Dictionary<int, DataHolderRoom> GetDataHolderRooms()
        {
            Dictionary<int, DataHolderRoom> dhRooms = new Dictionary<int, DataHolderRoom>();
            foreach(KeyValuePair<int, Room> entry in _rooms)
            {
                dhRooms.Add(entry.Key, entry.Value.GetDataHolderRoom());
            }
            return dhRooms;
        }
    }
}
