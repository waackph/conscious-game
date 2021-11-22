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
        private RoomGraph _roomGraph;
        private ContentManager _content;
        private int _preferredBackBufferWidth;
        private int _preferredBackBufferHeight;
        private Vector2 _centerPosition;
        private Player _player;
        private Cursor _cursor;
        private Texture2D _pixel;
        private Door _doorEntered;

        public Room currentRoom;
        public int CurrentRoomIndex;

        public RoomManager(ContentManager content, 
                           Player player,
                           Cursor cursor,
                           Texture2D pixel,
                           EntityManager entityManager,
                           UiDialogManager dialogManager,
                           SequenceManager sequenceManager,
                           MoodStateManager moodStateManager,
                           RoomGraph roomGraph,
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
            _roomGraph = roomGraph;

            _player = player;
            _cursor = cursor;

            _pixel = pixel;

            CurrentRoomIndex = 2;
            _doorEntered = null;

            LoadRooms();
        }
        
        public void LoadRooms()
        {
            Vector2 itemPosition;
            
            // Room 1
            Texture2D bg = _content.Load<Texture2D>("Backgrounds/debug/background");
            Room room = new Room(bg.Width, _entityManager, null);
            Thing thing = new Thing(10, null, "Background", bg, new Vector2(bg.Width/2, bg.Height/2));
            room.addThing(thing);

            itemPosition = new Vector2(1058, 570);
            ThoughtNode thought3 = CreateSimpleThought(22, 
                                                      "The door to the outside world",
                                                      new string[]{"Its locked. Maybe I can open it somehow? [use]"}, 
                                                      new Verb[]{Verb.UseWith},
                                                      1,
                                                      MoodState.Depressed);
            Thing door = new Door(1, "Door", false, true, false, false, true, "It's a door", 
                                  4, 2, 30, 
                                  new Vector2(1058, 579+150+_player.Height), 
                                  _content.Load<Texture2D>("Objects/debug/door_closed"),
                                  false, thought3, 
                                  _content.Load<Texture2D>("Objects/debug/door_opened"), 
                                  itemPosition);
            room.addThing(door);

            ThoughtNode thought2 = CreateSimpleThought(17, 
                                                      "It's a key. There is nothing more mundane",
                                                      new string[]{"Let's keep it anyway [pick up]"}, 
                                                      new Verb[]{Verb.PickUp},
                                                      2,
                                                      MoodState.Regular);
            Key combinedItem = new Key(4, "Oily Key", true, true, false, false, true, "The key is smooth now",
                                         1, null, _content.Load<Texture2D>("Objects/debug/key_oily"), new Vector2(448, 786+4+50));
            itemPosition = new Vector2(448, 786+4);
            Thing key = new CombineItem(2, "Key", true, false, true, false, false, "It's a key", 
                                        combinedItem, 3, thought2, _content.Load<Texture2D>("Objects/debug/key"), itemPosition);
            room.addThing(key);

            ThoughtNode thought6 = CreateSimpleThought(41, 
                                                      "It's a bottle. Wow...",
                                                      new string[]{"Maybe I can use it for something [combine]"}, 
                                                      new Verb[]{Verb.Combine},
                                                      3,
                                                      MoodState.None);
            itemPosition = new Vector2(200, 786+4);
            Thing combineItem = new CombineItem(3, "Oil Bottle", true, false, true, false, false, "It's a bottle", 
                                                null, 2, thought6, _content.Load<Texture2D>("Objects/debug/oil_bottle"), itemPosition);
            room.addThing(combineItem);

            // --------------------------- Morphing Item ---------------------------
            itemPosition = new Vector2(1058, 850);
            Dictionary<MoodState, Item> morphItems = new Dictionary<MoodState, Item>();
            Key morphItem1 = new Key(45, "Oily Key", true, true, false, false, true, "The key is smooth now", 1, null,
                                       _content.Load<Texture2D>("Objects/debug/key_oily"), itemPosition);
            Key morphItem2 = new Key(46, "Oil Bottle", true, true, false, false, true, "The key is smooth now", 1, null, 
                                       _content.Load<Texture2D>("Objects/debug/oil_bottle"), itemPosition);
            morphItems[MoodState.Regular] = morphItem1;
            morphItems[MoodState.Depressed] = morphItem2;
            Thing morph = new MorphingItem(_moodStateManager, morphItems, 
                                           6, "Morph", false, true, false, false, true, "It's morphing", null,
                                           _content.Load<Texture2D>("Objects/debug/oil_bottle"), itemPosition);
            room.addThing(morph);

            _rooms.Add(1, room);

            // room 2
            bg = _content.Load<Texture2D>("Backgrounds/480_270_Room_double_Concept_Draft");
            room = new Room(bg.Width, _entityManager, null);
            Thing background = new Thing(11, null, "Background", bg, new Vector2(bg.Width/2, bg.Height/2));
            room.addThing(background);

            // ThoughtNode thought = CreateSimpleThought(12, 
            //                                           "The door to the outside world. \nI am not ready for this.",
            //                                           new string[]{"Fuck it. I'll do it anyway [use]"}, 
            //                                           new Verb[]{Verb.Use},
            //                                           30,
            //                                           MoodState.None);
            // ----- Inner Dialog Thought -----
            
            ThoughtNode innerThought2 = new ThoughtNode(49, "First node", 0, false, 0);
            innerThought2.AddLink(new FinalThoughtLink(MoodState.None,
                                                  Verb.Use,
                                                  null,
                                                  0,
                                                  55,
                                                  null, 
                                                  "Ok. Let's go! [use]", 
                                                  true,
                                                  new MoodState[] {MoodState.Regular},
                                                  true));
            innerThought2.AddLink(new FinalThoughtLink(MoodState.None,
                                                  Verb.None,
                                                  null,
                                                  0,
                                                  47,
                                                  null, 
                                                  "Nah. Im not ready for this. I wanna go to bed again!", 
                                                  false,
                                                  new MoodState[] {MoodState.None},
                                                  false));
            ThoughtNode innerThought = new ThoughtNode(46, "The door to the outside world.", 0, true, 30);
            innerThought.AddLink(new ThoughtLink(45,
                                            innerThought2,
                                            "First link",
                                            false,
                                            new MoodState[] {MoodState.None}));

            itemPosition = new Vector2(260, 475);
            door = new Door(30, "Front Door", false, true, false, false, false, "It's a door", 
                            2, 1, 1,
                            new Vector2(260, 475+140+_player.Height), 
                            _content.Load<Texture2D>("Objects/front_door"),
                            true, innerThought,
                            _content.Load<Texture2D>("Objects/front_door_open"), itemPosition);
            room.addThing(door);

            ThoughtNode innerThought12 = new ThoughtNode(49, "First node", 0, false, 0);
            // TODO: Add sequence going into bathroom, wait x seconds, go back to current room.
            innerThought12.AddLink(new FinalThoughtLink(MoodState.Regular,
                                                  Verb.None,  // Verb.Use,
                                                  null,
                                                  55,
                                                  94,
                                                  null, 
                                                  "Puh.. I need to take a shower [use]", 
                                                  false,
                                                  new MoodState[] {MoodState.None},
                                                  true));
            innerThought12.AddLink(new FinalThoughtLink(MoodState.None,
                                                  Verb.None,
                                                  null,
                                                  0,
                                                  93,
                                                  null, 
                                                  "Im to exhausted. Maybe back to bed?", 
                                                  false,
                                                  new MoodState[] {MoodState.None},
                                                  false));
            ThoughtNode innerThought11 = new ThoughtNode(92, "The door to the Bathroom.", 0, true, 80);
            innerThought11.AddLink(new ThoughtLink(91,
                                            innerThought12,
                                            "First link",
                                            false,
                                            new MoodState[] {MoodState.None}));
            itemPosition = new Vector2(2500, 475);
            door = new Door(80, "Bathroom Door", false, true, false, false, false, "It's a door", 
                            2, 1, 1,
                            new Vector2(2500, 475+140+_player.Height), 
                            _content.Load<Texture2D>("Objects/bath_door"),
                            true, innerThought11,
                            _content.Load<Texture2D>("Objects/bath_door_open"), itemPosition);
            room.addThing(door);
            
            itemPosition = new Vector2(858, 786+4);
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

            ThoughtNode thought4 = CreateSimpleThought(33, 
                                                       "He looks handsome. I feel insecure.",
                                                       new string[] {"Maybe he likes a gift [give]", "I shall talk to him [talk]"}, 
                                                       new Verb[] {Verb.Give, Verb.TalkTo},
                                                       5,
                                                       MoodState.None);
            Thing character = new PuzzleCharacter(5, "Robo", "Hen", "Bib bup bip", true, 32, 
                                                  false, dialogTree, _dialogManager, 
                                                  thought4,
                                                  _content.Load<Texture2D>("NPCs/debug/npc"), itemPosition);
            // room.addThing(character);

            ThoughtNode thought5 = CreateSimpleThought(38, 
                                                       "A key. It looks shiny.",
                                                       new string[] {"Maybe I can use that at some time [pickup]"}, 
                                                       new Verb[] {Verb.PickUp},
                                                       32,
                                                       MoodState.None);

            // thought5 = CreateInnerDialogThought(45, 
            //                                     "Its already 12 am. Fuck. I don't want to get up.",
            //                                     new Dictionary<string, object>()
            //                                     {
            //                                         {"But I should.", new Dictionary<string, object>()
            //                                             { 
            //                                                 {"Why? There is only struggle awaiting me.", new Dictionary<string, object>
            //                                                     {
            //                                                         {"Thats what makes it interessting!", "No. Exhausting is the right word. Maybe I try some time later again"},
            //                                                         {"If I don't get up now I struggle with myself the rest of the day that I can't get up. Is that better?", "Right. Just like in the last days. (Sigh) Ok. Here we go"}
            //                                                     }
            //                                                 },
            //                                             }
            //                                         },
            //                                         {"Right. I just continue laying. Maybe I fall asleep again.", ""}
            //                                     }, 
            //                                     32,
            //                                     MoodState.None);

            itemPosition = new Vector2(448, 786+4);
            Thing giveItem = new CombineItem(32, "Key", true, false, false, true, false, "It's a key", 
                                             combinedItem, 5, thought5, _content.Load<Texture2D>("Objects/debug/key"), itemPosition);
            // room.addThing(giveItem);

            ThoughtNode innerThought10 = new ThoughtNode(90, "Right. Just like in the last days. (Sigh) Ok. Here we go", 0, false, 0);
            ThoughtNode innerThought9 = new ThoughtNode(89, "No. Exhausting is the right word. Maybe I try some time later again.", 0, false, 0);
            ThoughtNode innerThought8 = new ThoughtNode(88, "But why? There is only struggle awaiting me.", 0, false, 0);
            innerThought8.AddLink(new FinalThoughtLink(MoodState.None,
                                                  Verb.None,
                                                  null,
                                                  0,
                                                  87,
                                                  innerThought9, 
                                                  "Thats what makes it interessting!", 
                                                  false,
                                                  new MoodState[] {MoodState.None},
                                                  false));
            innerThought8.AddLink(new FinalThoughtLink(MoodState.None,
                                                  Verb.WakeUp,
                                                  null,
                                                  55,
                                                  86,
                                                  innerThought10, 
                                                  "If I don't get up now I struggle with myself the rest of the day that I can't get up. Is that better?", 
                                                  false,
                                                  new MoodState[] {MoodState.None},
                                                  true));

            ThoughtNode innerThought7 = new ThoughtNode(85, "First node", 0, false, 0);
            innerThought7.AddLink(new ThoughtLink(84,
                                                  innerThought8,
                                                  "But I should!",
                                                  false,
                                                  new MoodState[] {MoodState.None}));
            // innerThought7.AddLink(new FinalThoughtLink(MoodState.None,
            //                                       Verb.Use,
            //                                       null,
            //                                       0,
            //                                       55,
            //                                       null, 
            //                                       "Ok. Let's go! [use]", 
            //                                       true,
            //                                       new MoodState[] {MoodState.None},
            //                                       true));
            innerThought7.AddLink(new FinalThoughtLink(MoodState.None,
                                                  Verb.None,
                                                  null,
                                                  0,
                                                  83,
                                                  null, 
                                                  "Right. I just continue laying. Maybe I fall asleep again.", 
                                                  false,
                                                  new MoodState[] {MoodState.None},
                                                  false));
            ThoughtNode innerThought6 = new ThoughtNode(83, "Its already 12 am. Fuck. I don't want to get up.", 0, true, 30);
            innerThought6.AddLink(new ThoughtLink(82,
                                            innerThought7,
                                            "First link",
                                            false,
                                            new MoodState[] {MoodState.None}));
            innerThought6.IsInnerDialog = true;
            itemPosition = new Vector2(1576, 578);
            Thing clock = new Item(81, "Alarm Clock", false, false, false, false, false, 
                                   "Its my alarm clock", innerThought6, 
                                   _content.Load<Texture2D>("Objects/alarm_clock_draft"), itemPosition);
            room.addThing(clock);

            // TODO: Add Phone with dialog and animation (picking up phone and holding it to ear while dialog!)

            _rooms.Add(2, room);
        }

        private ThoughtNode CreateSimpleThought(int minId, string thoughtText, string[] action, Verb[] verbAction, int containingThingId, MoodState state)
        {
            ThoughtNode thought2 = new ThoughtNode(minId, "First node", 0, false, 0);
            for(int i = 0; i < action.Length; i++)
            {
                thought2.AddLink(new FinalThoughtLink(state,
                                                    verbAction[i],
                                                    null,
                                                    0,
                                                    minId+i+1,
                                                    null, 
                                                    action[i], 
                                                    false, 
                                                    new MoodState[] {MoodState.None},
                                                    false));
            }
            thought2.AddLink(new FinalThoughtLink(MoodState.None,
                                                  Verb.None,
                                                  null,
                                                  0,
                                                  minId+action.Length+1,
                                                  null, 
                                                  "I'll rather feel alone then [leave it]", 
                                                  false,
                                                  new MoodState[] {MoodState.None},
                                                  false));
            ThoughtNode thought = new ThoughtNode(minId+action.Length+2, thoughtText, 0, true, containingThingId);
            thought.AddLink(new ThoughtLink(minId+action.Length+3,
                                            thought2,
                                            "First link",
                                            false,
                                            new MoodState[] {MoodState.None}));
            return thought;
        }

        // private ThoughtNode CreateInnerDialogThought(int minId, string thoughtText, Dictionary<string, object> action, int containingThingId, MoodState state)
        // {
        //     int idCount = minId;

        //     ThoughtNode thought = new ThoughtNode(idCount+1, thoughtText, 0, true, containingThingId);
        //     thought.AddLink(new ThoughtLink(idCount+1,
        //                                     thought2,
        //                                     "First link",
        //                                     false,
        //                                     new MoodState[] {MoodState.None}));
        //     return thought;
        // }

        public void changeRoom(int roomId, Vector2 newPlayerPosition, int doorId=0)
        {
            Room lastRoom = currentRoom;
            currentRoom = _rooms[roomId];

            float xPos = 0f;
            Matrix transform = Matrix.CreateTranslation(xPos, 0, 0);
            _entityManager.ViewportTransformation = transform;
            _cursor.InverseTransform = Matrix.Invert(transform);
            // foreach(Thing thing in currentRoom.GetThings())
            // {
            //     thing.XPosOffset = 0;
            // }

            if(lastRoom != null)
            {
                lastRoom.ClearRoomEntityManager();
                currentRoom.FillEntityManager();
            }

            // Create path graph of room here
            _roomGraph.GenerateRoomGraph(currentRoom.GetBoundingBoxes(), 
                                         0, currentRoom.RoomWidth, 
                                         0, _preferredBackBufferHeight);

            if(currentRoom.EntrySequence != null && !currentRoom.EntrySequence.SequenceFinished)
            {
                _sequenceManager.StartSequence(currentRoom.EntrySequence, _player);
            }
            else if(lastRoom != null && newPlayerPosition != Vector2.Zero)
            {
                if(doorId != 0)
                {
                    _doorEntered = (Door)currentRoom.GetThingInRoom(doorId);
                    _doorEntered.OpenDoor();
                }
                // Set player to middle of door (for now quick fix)
                _player.Position = newPlayerPosition;
                _player.Position.Y = _player.Position.Y-100f;

                WalkCommand command = new WalkCommand(newPlayerPosition.X, newPlayerPosition.Y);
                List<Command> coms = new List<Command>()
                {
                    command
                };
                Sequence seq = new Sequence(coms);
                _sequenceManager.StartSequence(seq, _player);
            }
        }

        public void Update(GameTime gameTime){

            if(currentRoom == null)
            {
                // Testing: Sequence
                if(_rooms[CurrentRoomIndex].EntrySequence == null && CurrentRoomIndex == 2)
                {
                    _player.Position = new Vector2(10, 786);
                    WalkCommand command = new WalkCommand(1000f, 786f);
                    List<Command> coms = new List<Command>()
                    {
                        command
                    };
                    Sequence seq = new Sequence(coms);
                    _rooms[CurrentRoomIndex].EntrySequence = seq;
                }

                // currentRoom = _rooms[CurrentRoomIndex];
                changeRoom(CurrentRoomIndex, Vector2.Zero);
            }

            // Close the door when entered
            if(_doorEntered != null && !_sequenceManager.SequenceActive)
            {
                _doorEntered.CloseDoor();
                _doorEntered = null;
            }
 
            // Scroll room and thing positions
            if(currentRoom.RoomWidth != _preferredBackBufferWidth)
            {
                ScrollRoom();
            }

            LimitRoom();

            if(currentRoom.checkBoundingBoxes(_player.CollisionBox))
            {
                _player.Position = _player.LastPosition;
            }

            // Decide player draw order
            _player.UpdateDrawOrder(currentRoom.getDrawOrderInRoom(_player.CollisionBox));
            
            foreach(Door door in _entityManager.GetEntitiesOfType<Door>())
            {
                if(door.currentlyUsed == true)
                {
                    door.currentlyUsed = false;
                    changeRoom(door.RoomId, door.InitPlayerPos, door.DoorId);
                    break;
                }
            }
        }

        public void LimitRoom()
        {
            int roomEnding = currentRoom.RoomWidth;
            if(_player.Position.X > roomEnding - (_player.Width/6) / 2)
                _player.Position.X = roomEnding - (_player.Width/6) / 2;
            else if(_player.Position.X < (_player.Width/6) / 2)
                _player.Position.X = (_player.Width/6) / 2;
            if(_player.Position.Y > _preferredBackBufferHeight - (_player.Height/1.75f))
                _player.Position.Y = _preferredBackBufferHeight - (_player.Height/1.75f);
            else if(_player.Position.Y < _preferredBackBufferHeight * .55f)
                _player.Position.Y = _preferredBackBufferHeight *.55f;
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
            CurrentRoomIndex = roomIndex;
        }

        public void ResetCurrentRoom()
        {
            // currentRoom = _rooms[CurrentRoomIndex];
            
            // Testing: Sequence
            _player.Position = new Vector2(10, 900);
            WalkCommand command = new WalkCommand(1000f, 1000f);
            List<Command> coms = new List<Command>()
            {
                command
            };
            Sequence seq = new Sequence(coms);
            _rooms[CurrentRoomIndex].EntrySequence = seq;

            // currentRoom = _rooms[CurrentRoomIndex];
            changeRoom(CurrentRoomIndex, Vector2.Zero);

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
