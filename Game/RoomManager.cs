using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

using System.Collections.Generic;
using System;

namespace conscious
{
    /// <summary>Class <c>RoomManager</c> implements a level system.
    /// It manages the change of rooms and its initilization as well as change depending on mood state.
    /// </summary>
    ///
    public class RoomManager : IComponent
    {
        private Dictionary<int, Room> _rooms = new Dictionary<int, Room>();
        private EntityManager _entityManager;
        private UiDialogManager _dialogManager;
        private SequenceManager _sequenceManager;
        private MoodStateManager _moodStateManager;
        private AudioManager _audioManager;
        private SoCManager _socManager;
        private RoomGraph _roomGraph;
        private ContentManager _content;
        private int _preferredBackBufferWidth;
        private int _preferredBackBufferHeight;
        private Vector2 _centerPosition;
        private Player _player;
        private Cursor _cursor;
        private Texture2D _pixel;
        private Door _usedDoor;

        private bool doorSequenceActive = false;

        public Room currentRoom;
        public int CurrentRoomIndex;
        public SoundEffectInstance currentWalkingSound;
        public SoundEffectInstance currentAtmoSound;
        private SoundEffectInstance _defaultWalkingSound;

        public RoomManager(ContentManager content, 
                           Player player,
                           Cursor cursor,
                           Texture2D pixel,
                           EntityManager entityManager,
                           UiDialogManager dialogManager,
                           SequenceManager sequenceManager,
                           MoodStateManager moodStateManager,
                           AudioManager audioManager,
                           SoCManager socManager,
                           RoomGraph roomGraph,
                           int preferredBackBufferWidth, 
                           int preferredBackBufferHeight,
                           SoundEffectInstance defaultWalkingSound)
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
            _audioManager = audioManager;
            _roomGraph = roomGraph;

            _player = player;
            _cursor = cursor;

            _moodStateManager.MoodChangeEvent += changeRoomOnMood;

            _pixel = pixel;

            CurrentRoomIndex = 0;
            _usedDoor = null;

            _socManager = socManager;
            // _socManager.ActionEvent += executeThoughtInteraction;
            // _socManager.FinalEdgeSelected += doPlayerFinalThoughtActions;

            _defaultWalkingSound = defaultWalkingSound;
            _defaultWalkingSound.IsLooped = true;

            // LoadRooms();
        }

        private void changeRoomOnMood(object sender, MoodStateChangeEventArgs e)
        {
            updateLightMapOnMood(e.CurrentMoodState);
        }

        private void updateLightMapOnMood(MoodState moodState)
        {
            Texture2D currentLightMap;
            if(currentRoom.MoodLightMaps.ContainsKey(moodState))
            {
                currentLightMap = currentRoom.MoodLightMaps[moodState];
            }
            else
            {
                currentLightMap = currentRoom.MoodLightMaps[MoodState.None];
            }
            _entityManager.Lights = new List<Texture2D> { currentLightMap };
        }

        public void changeRoom(int roomId, Vector2 newPlayerPosition, int doorId = 0)
        {
            Room lastRoom = currentRoom;
            currentRoom = _rooms[roomId];

            _socManager.RemoveThoughts();

            float xPos = 0f;
            Matrix transform = Matrix.CreateTranslation(xPos, 0, 0);
            _entityManager.ViewTransform = transform;
            _cursor.InverseTransform = Matrix.Invert(transform);
            // foreach(Thing thing in currentRoom.GetThings())
            // {
            //     thing.XPosOffset = 0;
            // }

            // _audioManager.PlayMusic(currentRoom.MoodSoundFiles[_moodStateManager.moodState]);
            // _entityManager.LightMap = currentRoom.MoodLightMaps[_moodStateManager.moodState];
            // updateSongOnMood(_moodStateManager.moodState, Direction.None);
            updateLightMapOnMood(_moodStateManager.moodState);
            if (currentWalkingSound != null)
                currentWalkingSound.Pause();
            if (currentAtmoSound != null)
                currentAtmoSound.Pause();

            if (currentRoom.AtmoSound != null)
            {
                currentAtmoSound = currentRoom.AtmoSound;
                currentAtmoSound.IsLooped = true;
                currentAtmoSound.Play();
            }
            else
            {
                currentAtmoSound = null;
            }
            if (currentRoom.WalkingSound != null)
                currentWalkingSound = currentRoom.WalkingSound;
            else
                currentWalkingSound = _defaultWalkingSound;

            // Change the room visually by changing the entity manager's things
            if (lastRoom != null)
            {
                lastRoom.ClearRoomEntityManager();
                currentRoom.FillEntityManager();
            }

            _player.Scale = currentRoom.PlayerScale;

            // Create path graph of room here
            RecalculateRoomGraph(true);

            triggerThought(currentRoom);

            // Either start the entry sequence or in case of entering through a door 
            // start a sequence to walk to the new position
            if (currentRoom.EntrySequence != null && !currentRoom.EntrySequence.SequenceFinished)
            {
                _sequenceManager.StartSequence(currentRoom.EntrySequence, _player, MoodState.None);
            }
            else if (lastRoom != null && newPlayerPosition != Vector2.Zero
                     && doorId != 0 && !_sequenceManager.SequenceActive)
            {
                Door doorEntered = (Door)currentRoom.GetThingInRoom(doorId);
                doorEntered.OpenDoor(playSound: false);
                _player.Position = doorEntered.Position;
                WalkCommand walk = new WalkCommand(newPlayerPosition.X, newPlayerPosition.Y);
                WaitCommand wait = new WaitCommand(200);
                DoorActionCommand closeDoor = new DoorActionCommand(_entityManager, doorId);
                List<Command> coms = new List<Command>()
                {
                    walk,
                    wait,
                    closeDoor,
                };
                Sequence seq = new Sequence(coms, this);
                _sequenceManager.StartSequence(seq, _player, MoodState.None);
            }

            // Notify scripting API about room change
            EventBus.Publish(this, new RoomChangeEvent
            {
                RoomId = roomId,
            });
        }

        public void RecalculateRoomGraph(bool isInit)
        {
            _roomGraph.GenerateRoomGraph(isInit, currentRoom.GetBoundingBoxes(), 
                                         0, currentRoom.RoomWidth, 
                                         0, _preferredBackBufferHeight);
        }

        public void Update(GameTime gameTime)
        {

            if (currentRoom == null)
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
            // if (_usedDoor != null && !_sequenceManager.SequenceActive)
            // {
            //     _usedDoor.CloseDoor();
            //     _usedDoor = null;
            // }

            // Scroll room and thing positions
            if (currentRoom.RoomWidth != _preferredBackBufferWidth)
            {
                ScrollRoom();
            }

            if (!_sequenceManager.SequenceActive)
            {
                LimitRoom();
            }

            if (currentRoom.checkBoundingBoxes(_player.CollisionBox))
            {
                _player.Position = _player.LastPosition;
            }

            // Decide player draw order
            _player.UpdateDrawOrder(currentRoom.getDrawOrderInRoom(_player.CollisionBox));

            if (doorSequenceActive && !_sequenceManager.SequenceActive && _usedDoor != null)
            {
                doorSequenceActive = false;
                changeRoom(_usedDoor.RoomId, _usedDoor.InitPlayerPos, _usedDoor.DoorId);
                _usedDoor = null;
            }

            foreach (Door door in _entityManager.GetEntitiesOfType<Door>())
            {
                if (door.currentlyUsed && door.IsRoomChangeDoor)
                {
                    door.currentlyUsed = false;
                    startDoorSequence(door);
                    break;
                }
            }

            if (_player.IsMoving && currentWalkingSound.State != SoundState.Playing)
                currentWalkingSound.Play();
            else if (!_player.IsMoving && currentWalkingSound.State == SoundState.Playing)
                currentWalkingSound.Pause();
        }

        private void startDoorSequence(Door door)
        {
            // DoorActionCommand useDoor = new DoorActionCommand(_entityManager, door.DoorId);
            WaitCommand wait = new WaitCommand(200);
            DoorActionCommand openDoor = new DoorActionCommand(_entityManager, door.Id);
            WalkCommand walkThroughDoor = new WalkCommand(door.Position.X, door.Position.Y);
            List<Command> coms = new List<Command>()
            {
                wait,
                openDoor,
                walkThroughDoor,
            };
            Sequence seq = new Sequence(coms, this);
            _sequenceManager.StartSequence(seq, _player, MoodState.None);
            doorSequenceActive = true;
            _usedDoor = door;
        }

        public void LimitRoom()
        {
            Vector2 xLimits = currentRoom.xLimits;
            Vector2 yLimits = currentRoom.yLimits;
            // xLimits = new Vector2(0, roomEnding);
            // yLimits = new Vector2(_preferredBackBufferHeight*.45f, _preferredBackBufferHeight);
            if (_player.Position.X > xLimits.Y - (_player.Width / 6) / 2)
                _player.Position.X = xLimits.Y - (_player.Width / 6) / 2;
            else if (_player.Position.X < xLimits.X + (_player.Width / 6) / 2)
                _player.Position.X = xLimits.X + (_player.Width / 6) / 2;
            if (_player.Position.Y > yLimits.Y - (_player.Height / 1.75f))
                _player.Position.Y = yLimits.Y - (_player.Height / 1.75f);
            else if (_player.Position.Y < yLimits.X - (_player.Height / 1.75f))
                _player.Position.Y = yLimits.X - (_player.Height / 1.75f);
        }

        public void ScrollRoom()
        {
            float horizontalMiddleBegin = (float)_preferredBackBufferWidth / 2f;
            float horizontalMiddleEnd = (float)currentRoom.RoomWidth - horizontalMiddleBegin;
            float xPos = _player.Position.X;
            if (xPos < horizontalMiddleBegin) xPos = horizontalMiddleBegin;
            if (xPos > horizontalMiddleEnd) xPos = horizontalMiddleEnd;
            // The camera movement is inverted, so we need to use the negative xPos
            Matrix transform = Matrix.CreateTranslation(-xPos + horizontalMiddleBegin, 0, 0);
            _entityManager.ViewTransform = transform;
            _cursor.InverseTransform = Matrix.Invert(transform);
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
            // _player.Position = new Vector2(10, 900);
            // WalkCommand command = new WalkCommand(1000f, 1000f);
            // List<Command> coms = new List<Command>()
            // {
            //     command
            // };
            // Sequence seq = new Sequence(coms);
            // _rooms[CurrentRoomIndex].EntrySequence = seq;

            // currentRoom = _rooms[CurrentRoomIndex];
            changeRoom(CurrentRoomIndex, Vector2.Zero);

        }

        private void triggerThought(Room room)
        {
            if(room.Thought != null)
            {
                _socManager.AddThought(room.Thought);
            }
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


        public void LoadRooms()
        {
            Vector2 itemPosition;
            Texture2D bg;
            Room room;
            // Thing thing;
            Song song;
            Texture2D lightMap;
            Door door;
            
            ////////////////////////////////////////////////
            // room 2
            bg = _content.Load<Texture2D>("Backgrounds/480_270_Room_double_Concept_Draft");
            song = _content.Load<Song>("Audio/Red_Curtains");
            lightMap = _content.Load<Texture2D>("light/light_map_default");
            room = new Room(bg.Width, _entityManager, null, lightMap, null, song);
            Thing background = new Thing(11, null, _moodStateManager, "Background", bg, new Vector2(bg.Width/2, bg.Height/2), 1);
            room.addThing(background);

            // ThoughtNode thought = CreateSimpleThought(12, 
            //                                           "The door to the outside world. \nI am not ready for this.",
            //                                           new string[]{"Fuck it. I'll do it anyway [use]"}, 
            //                                           new Verb[]{Verb.Use},
            //                                           30,
            //                                           MoodState.None);
            // ----- Inner Dialog Thought -----
            
            ThoughtNode innerThought2 = new ThoughtNode(49, "I don't know if I'm ready.", 0, false, 0);
            innerThought2.AddLink(new FinalThoughtLink(MoodState.None,
                                                  Verb.Use,
                                                  null,
                                                  null,
                                                  0,
                                                  55,
                                                  null, 
                                                  "Ok. Let's go! [use]", 
                                                  false,
                                                  new MoodState[] {MoodState.Regular},
                                                  true));
            innerThought2.AddLink(new FinalThoughtLink(MoodState.None,
                                                  Verb.None,
                                                  null,
                                                  null,
                                                  0,
                                                  55,
                                                  null, 
                                                  "I can't. I'm just feeling to ugly [not use]", 
                                                  false,
                                                  new MoodState[] {MoodState.Depressed},
                                                  false));
            innerThought2.AddLink(new FinalThoughtLink(MoodState.None,
                                                  Verb.None,
                                                  null,
                                                  null,
                                                  0,
                                                  47,
                                                  null, 
                                                  "Nah. Let's go back to bed again!", 
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
                            _content.Load<Texture2D>("Objects/front_door"), true,
                            true, innerThought, _moodStateManager, 
                            _content.Load<Texture2D>("Objects/front_door_open"), itemPosition, 3);
            room.addThing(door);

            ThoughtNode innerThought12 = new ThoughtNode(49, "Cleaning is so annoying", 0, false, 0);
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
                            _content.Load<Texture2D>("Objects/bath_door"), true,
                            true, innerThought11, _moodStateManager, 
                            _content.Load<Texture2D>("Objects/bath_door_open"), itemPosition, 3);

            // Add final link with animation
            // Add sequence going into bathroom, wait x seconds, go back to current room.
            WalkCommand walkToDoor = new WalkCommand(door.Position.X, door.Position.Y);
            DoorActionCommand useDoor = new DoorActionCommand(_entityManager, 80);
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
            Sequence seq = new Sequence(coms, this);

            innerThought12.AddLink(new FinalThoughtLink(MoodState.Regular,
                                                        Verb.None,  // We use the sequence here, so no verb needed
                                                        null, 
                                                        seq,
                                                        0,
                                                        94,
                                                        null, 
                                                        "Puh.. But I need to take a shower [use]", 
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
                                                       "My phone means only trouble to me",
                                                       new string[] {"Just ignore it. Don't start thinking about it.", "I need to call her now. She surely is angry with me by now [talk]"}, 
                                                       new Verb[] {Verb.None, Verb.TalkTo},
                                                       new int[] {0, 94},
                                                       new bool[] {false, true},
                                                       new bool[] {false, true},
                                                       5,
                                                       MoodState.None);
            Thing character = new Character(5, "Phone", "She", "Riiiing", 
                                            false, dialogTree, _dialogManager, 
                                            thought4, _moodStateManager, 
                                            _content.Load<Texture2D>("NPCs/phone_draft"), itemPosition, 3);
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
                                                  35,
                                                  86,
                                                  innerThought10, 
                                                  "If I don't get up now I struggle with myself the rest of the day that I can't get up. Is that better?", 
                                                  false,
                                                  new MoodState[] {MoodState.None},
                                                  true));

            ThoughtNode innerThought7 = new ThoughtNode(85, "Its already 12 am. Fuck. I don't want to get up.", 0, false, 0);
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
            ThoughtNode innerThought6 = new ThoughtNode(83, "Oh no, I don't wanna know the time right now.", 0, true, 30);
            innerThought6.AddLink(new ThoughtLink(82,
                                            innerThought7,
                                            "First link",
                                            false,
                                            new MoodState[] {MoodState.None}));
            innerThought6.IsInnerDialog = true;
            itemPosition = new Vector2(1576, 578);
            Thing clock = new Item(81, "Alarm Clock", false, false, false, false, false, 
                                   "Its my alarm clock", innerThought6, _moodStateManager, 
                                   _content.Load<Texture2D>("Objects/alarm_clock_draft"), itemPosition, 3);
            room.addThing(clock);

            _rooms.Add(2, room);
        }

        private ThoughtNode CreateSimpleThought(int minId, string introText, string thoughtText, string[] action, Verb[] verbAction, int[] unlockIDs, bool[] isSuccessList, bool[] isLocked, int containingThingId, MoodState state)
        {
            ThoughtNode thought2 = new ThoughtNode(minId, introText, 0, false, 0);
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
                                                    isLocked[i],
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

    }
}
