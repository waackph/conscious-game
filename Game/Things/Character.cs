using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace conscious
{
    public class Character : Thing
    {
        protected int Id { get; }
        protected List<Node> _treeStructure;

        protected DialogManager _dialogManager;
        protected string _pronoun;
        protected string _catchPhrase;

        public bool GiveAble { get; }
        public MoodState MoodChange { get; set; }

        public Character(int id, 
                         string name, 
                         string pronoun, 
                         string catchPhrase, 
                         bool giveAble, 
                         List<Node> treeStructure, 
                         DialogManager dialogManager,
                         MoodState moodChange,
                         Texture2D texture, Vector2 position)
                        : base(name, texture, position)
        {
            Id = id;
            _pronoun = pronoun;
            _catchPhrase = catchPhrase;

            GiveAble = giveAble;

            MoodChange = moodChange;

            _treeStructure = treeStructure;
            _dialogManager = dialogManager;

            Collidable = true;
        }

        public virtual void TalkTo()
        {
            if(_treeStructure.Count == 0)
                _dialogManager.DoDisplayText(_pronoun + " has nothing to talk about.");
            else
                _dialogManager.StartDialog(_treeStructure);
        }

        public virtual bool Give(Item item)
        {
            _dialogManager.DoDisplayText(_pronoun + " is not interessted in that.");
            return false;
        }
        public override DataHolderEntity GetDataHolderEntity()
        {
            DataHolderCharacter dataHolderEntity = new DataHolderCharacter();
            // Entity
            dataHolderEntity.Name = Name;
            dataHolderEntity.PositionX  = Position.X;
            dataHolderEntity.PositionY = Position.Y;
            dataHolderEntity.Rotation = Rotation;
            dataHolderEntity.texturePath = EntityTexture.ToString();
            // Character
            dataHolderEntity.Id = Id;
            dataHolderEntity.TreeStructure = _treeStructure;
            dataHolderEntity.Pronoun = _pronoun;
            dataHolderEntity.CatchPhrase = _catchPhrase;
            dataHolderEntity.GiveAble = GiveAble;
            dataHolderEntity.MoodChange = MoodChange;
            return dataHolderEntity;
        }
    }
}