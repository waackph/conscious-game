using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace conscious
{
    public class Character : Thing
    {
        protected List<Node> _treeStructure;

        protected UiDialogManager _dialogManager;
        protected string _pronoun;
        protected string _catchPhrase;

        public bool GiveAble { get; }

        public Character(int id, 
                         string name, 
                         string pronoun, 
                         string catchPhrase, 
                         bool giveAble, 
                         List<Node> treeStructure, 
                         UiDialogManager dialogManager,
                         ThoughtNode thought,
                         MoodStateManager moodStateManager, 
                         Texture2D texture, Vector2 position)
                        : base(id, thought, moodStateManager, name, texture, position)
        {
            _pronoun = pronoun;
            _catchPhrase = catchPhrase;

            GiveAble = giveAble;

            _treeStructure = treeStructure;
            _dialogManager = dialogManager;

            Collidable = true;
            DrawOrder = 4;
        }

        public virtual void TalkTo()
        {
            if(_treeStructure.Count == 0)
                _dialogManager.DoDisplayText(_pronoun + " has nothing to talk about.", this);
            else
                _dialogManager.StartDialog(_treeStructure, this);
        }

        public virtual bool Give(Item item)
        {
            _dialogManager.DoDisplayText(_pronoun + " is not interessted in that.", this);
            return false;
        }
        public override DataHolderEntity GetDataHolderEntity()
        {
            DataHolderCharacter dataHolderEntity = new DataHolderCharacter();
            dataHolderEntity = (DataHolderCharacter)base.GetDataHolderEntity(dataHolderEntity);
            // Character
            dataHolderEntity.TreeStructure = _treeStructure;
            dataHolderEntity.Pronoun = _pronoun;
            dataHolderEntity.CatchPhrase = _catchPhrase;
            dataHolderEntity.GiveAble = GiveAble;
            return dataHolderEntity;
        }
        public DataHolderEntity GetDataHolderEntity(DataHolderCharacter dataHolderEntity)
        {
            dataHolderEntity = (DataHolderCharacter)base.GetDataHolderEntity(dataHolderEntity);
            // Character
            dataHolderEntity.TreeStructure = _treeStructure;
            dataHolderEntity.Pronoun = _pronoun;
            dataHolderEntity.CatchPhrase = _catchPhrase;
            dataHolderEntity.GiveAble = GiveAble;
            return dataHolderEntity;
        }
    }
}