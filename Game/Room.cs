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

        public bool checkBoundingBoxes(Rectangle boundBox){
            foreach(Thing thing in _things){
                if(thing.Collidable && boundBox.Intersects(thing.BoundingBox))
                {
                    return true;
                }
            }
            return false;
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

        public void SetThings(List<Thing> things)
        {
            _things = things;
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
            return dataHolderRoom;
        }
    }
}