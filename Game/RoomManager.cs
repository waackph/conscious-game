using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

using System.Collections.Generic;
using System;

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

        public event EventHandler<bool> TerminateGameEvent;

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

        protected virtual void OnTerminateGameEvent(bool e)
        {
            TerminateGameEvent?.Invoke(this, e);
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
                                                      new int[] {0},
                                                      new bool[] {true},
                                                      1,
                                                      MoodState.Depressed);
            Door door = new Door(1, "Door", false, true, false, false, true, "It's a door", 
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
                                                      new int[]{0},
                                                      new bool[] {true},
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
                                                      new int[]{0},
                                                      new bool[] {true},
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

            ////////////////////////////////////////////////
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
                                                  null,
                                                  0,
                                                  55,
                                                  null, 
                                                  "Ok. Let's go! [use]", 
                                                  true,
                                                  new MoodState[] {MoodState.None},  // TODO: Use regular when bath door use action also implemented (showering)
                                                  true));
            innerThought2.AddLink(new FinalThoughtLink(MoodState.None,
                                                  Verb.None,
                                                  null,
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
            innerThought12.AddLink(new FinalThoughtLink(MoodState.None,
                                                  Verb.None,
                                                  null,
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

            // Add final link with animation
            // Add sequence going into bathroom, wait x seconds, go back to current room.
            WalkCommand walkToDoor = new WalkCommand(door.Position.X, door.Position.Y);
            DoorActionCommand useDoor = new DoorActionCommand(door);
            VanishCommand playerAppearance = new VanishCommand();
            WaitCommand wait = new WaitCommand(10000);
            SoundEffect sound = _content.Load<SoundEffect>("Audio/ShowerSound");
            wait.Sound = sound;
            WalkCommand walkToRoom = new WalkCommand(2500, 1000);
            List<Command> coms = new List<Command>()
            {
                walkToDoor,
                useDoor,
                playerAppearance,
                wait,
                useDoor,
                playerAppearance,
                walkToRoom
            };
            Sequence seq = new Sequence(coms);

            innerThought12.AddLink(new FinalThoughtLink(MoodState.Regular,
                                                        Verb.None,  // We use the sequence here, so no verb needed
                                                        null, 
                                                        seq,
                                                        55,
                                                        94,
                                                        null, 
                                                        "Puh.. I need to take a shower [use]", 
                                                        true,
                                                        new MoodState[] {MoodState.None},
                                                        true));

            room.addThing(door);
            
            itemPosition = new Vector2(1488, 600);
            // Start Initilizing dialog tree
            List<Node> dialogTree = new List<Node>();
            List<Edge> edges = new List<Edge>();
            edges.Add(new Edge(2, "Like puke. But its nice that you called.", MoodState.None));
            edges.Add(new Edge(3, "Won't complain. You?", MoodState.None));
            Node root = new Node(1, "Hey Lola, how are you?", edges);
            dialogTree.Add(root);
            edges = new List<Edge>();
            edges.Add(new Edge(4, "Sorry.", MoodState.None));
            Node node2 = new Node(2, "Well, you need to get out of your room. Im trying to reach you for days now.", edges);
            dialogTree.Add(node2);
            edges = new List<Edge>();
            edges.Add(new Edge(4, "Do I have to?", MoodState.None));
            Node node3 = new Node(3, "Just meet me in the park. It will be nice. I promise!", edges);
            dialogTree.Add(node3);
            edges = new List<Edge>();
            edges.Add(new Edge(0, "Yeah, seems to help. I'll be there as soon as I can. Bye.", MoodState.None));
            Node node4 = new Node(4, "It it helps.. yes! Meet you there.", edges);
            dialogTree.Add(node4);
            // End Initilizing dialog tree

            ThoughtNode thought4 = CreateSimpleThought(33, 
                                                       "Oh no. Mara called... 10 times.",
                                                       new string[] {"Social Contact, yikes!", "I need to call her now. She surely is angry with me by now [talk]"}, 
                                                       new Verb[] {Verb.None, Verb.TalkTo},
                                                       new int[] {0, 94},
                                                       new bool[] {false, true},
                                                       5,
                                                       MoodState.None);
            Thing character = new Character(5, "Phone", "She", "Riiiing", 
                                            false, dialogTree, _dialogManager, 
                                            thought4,
                                            _content.Load<Texture2D>("NPCs/phone_draft"), itemPosition);
            room.addThing(character);

            ThoughtNode innerThought10 = new ThoughtNode(90, "Right. Just like in the last days. (Sigh) Ok. Here we go", 0, false, 0);
            ThoughtNode innerThought9 = new ThoughtNode(89, "No. Exhausting is the right word. Maybe I try some time later again.", 0, false, 0);
            ThoughtNode innerThought8 = new ThoughtNode(88, "But why? There is only struggle awaiting me.", 0, false, 0);
            innerThought8.AddLink(new FinalThoughtLink(MoodState.None,
                                                  Verb.None,
                                                  null,
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
                                                  null,
                                                  0,
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

        private ThoughtNode CreateSimpleThought(int minId, string thoughtText, string[] action, Verb[] verbAction, int[] unlockIDs, bool[] isSuccessList, int containingThingId, MoodState state)
        {
            ThoughtNode thought2 = new ThoughtNode(minId, "First node", 0, false, 0);
            for(int i = 0; i < action.Length; i++)
            {
                thought2.AddLink(new FinalThoughtLink(state,
                                                    verbAction[i],
                                                    null,
                                                    null,
                                                    unlockIDs[i],
                                                    minId+i+1,
                                                    null, 
                                                    action[i], 
                                                    false, 
                                                    new MoodState[] {MoodState.None},
                                                    isSuccessList[i]));
            }
            thought2.AddLink(new FinalThoughtLink(MoodState.None,
                                                  Verb.None,
                                                  null,
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
                // if(_rooms[CurrentRoomIndex].EntrySequence == null && CurrentRoomIndex == 2)
                // {
                //     _player.Position = new Vector2(10, 786);
                //     WalkCommand command = new WalkCommand(1000f, 786f);
                //     List<Command> coms = new List<Command>()
                //     {
                //         command
                //     };
                //     Sequence seq = new Sequence(coms);
                //     _rooms[CurrentRoomIndex].EntrySequence = seq;
                // }

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
                    // changeRoom(door.RoomId, door.InitPlayerPos, door.DoorId);
                    OnTerminateGameEvent(true);
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
