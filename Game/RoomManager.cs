using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using System.Collections.Generic;
using System;

using conscious.Sequences;

namespace conscious
{
    public class RoomManager : IComponent
    {
        private Dictionary<int, Room> _rooms = new Dictionary<int, Room>();
        private EntityManager _entityManager;
        private DialogManager _dialogManager;
        private SequenceManager _sequenceManager;
        private ContentManager _content;
        private int _preferredBackBufferWidth;
        private int _preferredBackBufferHeight;
        private Vector2 _centerPosition;
        private Player _player;
        private Cursor _cursor;
        private int _currentRoomIndex;

        public Room currentRoom;

        public RoomManager(ContentManager content, 
                           Player player,
                           Cursor cursor,
                           EntityManager entityManager,
                           DialogManager dialogManager,
                           SequenceManager sequenceManager,
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

            _player = player;
            _cursor = cursor;

            _currentRoomIndex = 2;

            // LoadRooms();
        }
        
        public void LoadRooms(){
            Vector2 itemPosition;
            
            // Room 1
            Texture2D bg = _content.Load<Texture2D>("Backgrounds/debug/background");
            Room room = new Room(bg.Width, _entityManager, null);
            room.addThing("Background", bg, new Vector2(bg.Width/2, bg.Height/2));
            itemPosition = new Vector2(1058, 570);
            room.addDoor(1, "Door", false, true, false, false, true, "It's a door", 
                        _content.Load<Texture2D>("Objects/debug/door_closed"), itemPosition, 4, 2, false);
            itemPosition = new Vector2(448, 786+4);
            Key combinedItem = new Key(4, "Oily Key", true, true, false, false, true, "The key is smooth now", 1, 
                                       _content.Load<Texture2D>("Objects/debug/key_oily"), Vector2.Zero);
            room.addCombinable(2, "Key", true, false, true, true, false, "It's a key", 
                        _content.Load<Texture2D>("Objects/debug/key"), itemPosition, 3, combinedItem);
            itemPosition = new Vector2(200, 786+4);
            room.addCombinable(3, "Oil Bottle", true, false, true, false, false, "It's a bottle", 
                        _content.Load<Texture2D>("Objects/debug/oil_bottle"), itemPosition, 2, null);
            _rooms.Add(1, room);

            // room 2
            bg = _content.Load<Texture2D>("Backgrounds/debug/background_double");
            room = new Room(bg.Width, _entityManager, null);
            room.addThing("Background", bg, new Vector2(bg.Width/2, bg.Height/2));
            itemPosition = new Vector2(1058, 575+4);
            room.addDoor(1, "Door", false, true, false, false, false, "It's a door", 
                        _content.Load<Texture2D>("Objects/debug/door_opened"), itemPosition, 2, 1, true);
            itemPosition = new Vector2(1400, 786+4);
            // Start Initilizing dialog tree
            List<Node> dialogTree = new List<Node>();
            List<Edge> edges = new List<Edge>();
            edges.Add(new Edge(2, "True dat!"));
            edges.Add(new Edge(3, "Whatever."));
            Node root = new Node(1, "Bib bup bip", edges);
            dialogTree.Add(root);
            edges = new List<Edge>();
            edges.Add(new Edge(4, "Tell me more"));
            Node node2 = new Node(2, "Bab bip", edges);
            dialogTree.Add(node2);
            edges = new List<Edge>();
            edges.Add(new Edge(4, "Like I said, whatever."));
            Node node3 = new Node(3, "Bub bap", edges);
            dialogTree.Add(node3);
            edges = new List<Edge>();
            edges.Add(new Edge(0, "It was nice to talk to you too Sir."));
            Node node4 = new Node(4, "Bib bip bup bip", edges);
            dialogTree.Add(node4);
            // End Initilizing dialog tree
            room.addCharacter(5, "Robo", "Hen", "Bib bup bip", true, 
                              _content.Load<Texture2D>("NPCs/debug/npc"), itemPosition, dialogTree, _dialogManager);
            _rooms.Add(2, room);

            // room 3
            bg = _content.Load<Texture2D>("Backgrounds/Layer0_0");
            room = new Room(bg.Width, _entityManager, null);
            room.addThing("Background 1", bg, Vector2.Zero);
            room.addThing("Background 2", _content.Load<Texture2D>("Backgrounds/Layer1_0"), Vector2.Zero);
            itemPosition = new Vector2(_centerPosition.X, _centerPosition.Y);
            itemPosition.X += _preferredBackBufferHeight*.25f;
            itemPosition.Y += _preferredBackBufferHeight*.25f;
            room.addItem(1, "Ball", true, false, false, false, false, "It's a ball", 
                        _content.Load<Texture2D>("Objects/ball"), itemPosition);
            _rooms.Add(3, room);
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
