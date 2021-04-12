using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace conscious
{
    public class Character : Thing
    {
        protected int Id { get; }
        protected List<Node> _treeStructure;

        protected UiDialogManager _dialogManager;
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
                         UiDialogManager dialogManager,
                         MoodState moodChange,
                         ThoughtNode thought,
                         Texture2D texture, Vector2 position)
                        : base(thought, name, texture, position)
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
            dataHolderEntity = (DataHolderCharacter)base.GetDataHolderEntity(dataHolderEntity);
            // Character
            dataHolderEntity.Id = Id;
            dataHolderEntity.TreeStructure = _treeStructure;
            dataHolderEntity.Pronoun = _pronoun;
            dataHolderEntity.CatchPhrase = _catchPhrase;
            dataHolderEntity.GiveAble = GiveAble;
            dataHolderEntity.MoodChange = MoodChange;
            return dataHolderEntity;
        }
        public DataHolderEntity GetDataHolderEntity(DataHolderCharacter dataHolderEntity)
        {
            dataHolderEntity = (DataHolderCharacter)base.GetDataHolderEntity(dataHolderEntity);
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