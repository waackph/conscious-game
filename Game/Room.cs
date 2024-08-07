using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace conscious
{
    /// <summary>Class <c>Room</c> holds data and logic for a room and the Things that are in a Room.
    /// It also includes light maps and music.
    /// </summary>
    ///
    public class Room
    {
        private EntityManager _entityManager;
        private List<Thing> _things = new List<Thing>();
        public int RoomWidth;
        public Vector2 xLimits;
        public Vector2 yLimits;
        public Sequence EntrySequence;
        public Texture2D LightMap;
        public Song SoundFile;
        public SoundEffectInstance AtmoSound;
        public SoundEffectInstance WalkingSound;
        public Dictionary<MoodState, Texture2D> MoodLightMaps;
        public Dictionary<MoodState, Song> MoodSoundFiles;
        public ThoughtNode Thought { get; protected set; }

        public Room(int roomWidth, EntityManager entityManager, Sequence sequence, Texture2D lightMap, ThoughtNode thought, 
                    Song soundFile = null, SoundEffectInstance atmoSound = null, SoundEffectInstance walkingSound = null,
                    int xLimStart = 0, int xLimEnd = 1920, int yLimStart = 0, int yLimEnd = 1080)
        {
            RoomWidth = roomWidth;
            _entityManager = entityManager;
            EntrySequence = sequence;

            xLimits = new Vector2(xLimStart, xLimEnd);
            yLimits = new Vector2(yLimStart, yLimEnd);
            
            LightMap = lightMap;
            MoodLightMaps = new Dictionary<MoodState, Texture2D>();
            MoodLightMaps[MoodState.None] = LightMap;

            SoundFile = soundFile;
            MoodSoundFiles = new Dictionary<MoodState, Song>();
            MoodSoundFiles[MoodState.None] = SoundFile;

            WalkingSound = walkingSound;
            AtmoSound = atmoSound;

            Thought = thought;
        }

        public IEnumerable<T> GetThingsOfType<T>() where T : Thing
        {
            return _things.OfType<T>();
        }

        public bool checkBoundingBoxes(Rectangle boundBox)
        {
            foreach(Thing thing in _things){
                if(thing.Collidable && boundBox.Intersects(thing.CollisionBox))
                {
                    return true;
                }
            }
            return false;
        }

        public int getDrawOrderInRoom(Rectangle collisionBox)
        {
            // project collision boxes onto X Axis
            // start with highest draw order and decrease if something is in front of player
            int currentDrawOrder = 12;
            Rectangle bboxProjected = collisionBox;
            bboxProjected.Y = 0;
            foreach(Thing thing in _things)
            {
                Rectangle thingBoxProjected = thing.CollisionBox;
                thingBoxProjected.Y = 0;
                if(GlobalData.IsNotBackgroundOrPlayer(thing) && thingBoxProjected.Intersects(bboxProjected) && thing.Collidable)
                {
                    if(thing.CollisionBox.Y >= collisionBox.Y)
                    {
                        if(currentDrawOrder >= thing.DrawOrder)
                        {
                            currentDrawOrder = thing.DrawOrder - 1;
                        }
                    }
                }
            }
            return currentDrawOrder;
        }

        public void addThing(Thing thing)
        {
            _things.Add(thing);
        }

        public void RemoveThing(Thing thing){
            if(_things.Contains(thing))
            {
                _things.Remove(thing);
                _entityManager.RemoveEntity(thing);
            }
        }

        public List<Thing> GetThings()
        {
            return _things;
        }

        public List<Rectangle> GetBoundingBoxes()
        {
            List<Rectangle> bbs = new List<Rectangle>();
            foreach(Thing thing in _things)
            {
                if(thing.Collidable)
                    bbs.Add(thing.CollisionBox);
            }
            return bbs;
        }

        public void SetThings(List<Thing> things)
        {
            _things = things;
        }

        public Thing GetThingInRoom(int thingId)
        {
            foreach(Thing thing in _things)
            {
                if(thing.Id == thingId)
                {
                    return thing;
                }
            }
            return null;
        }

        public void FillEntityManager()
        {
            foreach(Thing thing in _things)
            {
                _entityManager.AddEntity(thing);
            }
        }
        public void ClearRoomEntityManager()
        {
            foreach(Thing thing in _things)
            {
                _entityManager.RemoveEntity(thing);
            }
        }
        
        public DataHolderRoom GetDataHolderRoom()
        {
            DataHolderRoom dataHolderRoom = new DataHolderRoom();
            List<DataHolderEntity> dhThings = new List<DataHolderEntity>();
            foreach(Thing thing in _things)
            {
                dhThings.Add(thing.GetDataHolderEntity());
            }
            dataHolderRoom.RoomWidth = RoomWidth;
            dataHolderRoom.Things = dhThings;
            dataHolderRoom.EntrySequence = EntrySequence?.GetDataHolderSequence();
            dataHolderRoom.SoundFilePath = SoundFile?.Name;
            dataHolderRoom.LightMapPath = LightMap?.ToString();
            dataHolderRoom.AtmoSoundFilePath = AtmoSound?.ToString();
            dataHolderRoom.WalkingSoundFilePath = WalkingSound?.ToString();
            dataHolderRoom.Thought = Thought?.GetDataHolderThoughtNode();
            return dataHolderRoom;
        }
    }
}