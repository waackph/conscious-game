using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

namespace conscious
{
    public class Room
    {
        private EntityManager _entityManager;
        private List<Thing> _things = new List<Thing>();
        public int RoomWidth;
        public Sequence EntrySequence;

        public Room(int roomWidth, EntityManager entityManager, Sequence sequence)
        {
            RoomWidth = roomWidth;
            _entityManager = entityManager;
            EntrySequence = sequence;
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
            int currentDrawOrder = 5;
            Rectangle bboxProjected = collisionBox;
            bboxProjected.Y = 0;
            foreach(Thing thing in _things)
            {
                Rectangle thingBoxProjected = thing.CollisionBox;
                thingBoxProjected.Y = 0;
                if(thing.Name != "Background" && thingBoxProjected.Intersects(bboxProjected))
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
                bbs.Add(thing.BoundingBox);
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
            dataHolderRoom.EntrySequence = EntrySequence;
            return dataHolderRoom;
        }
    }
}