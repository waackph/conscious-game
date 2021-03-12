using conscious.Sequences;

using System.Collections.Generic;
using System.Linq;
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public void addThing(string name, Texture2D thingTexture, Vector2 thingPosition){
            Thing thing = new Thing(name, thingTexture, thingPosition);
            _things.Add(thing);
        }

        public void addItem(int id, string name, bool pickUpAble, bool useAble, bool combineAble, bool giveAble, bool useWith, string examineText,
                            Texture2D itemTexture, Vector2 itemPosition){
            Item item = new Item(id, name, pickUpAble, useAble, combineAble, giveAble, useWith, examineText, itemTexture, itemPosition);
            _things.Add(item);
        }

        public void addCombinable(int id, string name, bool pickUpAble, bool useAble, bool combineAble, bool giveAble, bool useWith, string examineText,
                            Texture2D itemTexture, Vector2 itemPosition, int itemDependency, Item cItem){
            CombineItem item = new CombineItem(id, name, pickUpAble, useAble, combineAble, giveAble, useWith, examineText, cItem, itemDependency, itemTexture, itemPosition);
            _things.Add(item);
        }

        public void addDoor(int id, string name, bool pickUpAble, bool useAble, bool combineAble, bool giveAble, bool useWith, string examineText,
                            Texture2D itemTexture, Vector2 itemPosition, int itemDependency, int roomId, bool unlocked){
            Door door = new Door(id, name, pickUpAble, useAble, combineAble, giveAble, useWith, examineText, itemDependency, roomId, unlocked, itemTexture, itemPosition);
            _things.Add(door);
            // Doors.Add(door);
        }

        public void addKey(int id, string name, bool pickUpAble, bool useAble, bool combineAble, bool giveAble, bool useWith, string examineText,
                            Texture2D itemTexture, Vector2 itemPosition, int itemDependency){
            Key key = new Key(id, name, pickUpAble, useAble, combineAble, giveAble, useWith, examineText, itemDependency, itemTexture, itemPosition);
            _things.Add(key);
        }

        public void addCharacter(int id, string name, string pronoun, string catchPhrase, bool giveAble,
                                 Texture2D characterTexture, Vector2 characterPosition, List<Node> treeStructure, DialogManager dialogManager){
            Character npc = new Character(id, name, pronoun, catchPhrase, giveAble, 
                                          treeStructure, dialogManager, characterTexture, characterPosition);
            _things.Add(npc);
        }

        public void addPuzzleCharacter(int id, string name, string pronoun, string catchPhrase, bool giveAble,
                                 Texture2D characterTexture, Vector2 characterPosition, int itemDependency, bool dialogUnlocked,
                                 List<Node> treeStructure, DialogManager dialogManager){
            PuzzleCharacter npc = new PuzzleCharacter(id, name, pronoun, catchPhrase, giveAble, itemDependency, dialogUnlocked, 
                                                      treeStructure, dialogManager, characterTexture, characterPosition);
            _things.Add(npc);
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