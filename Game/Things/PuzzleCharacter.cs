using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace conscious
{
    /// <summary>Class <c>PuzzleCharacter</c> holds data and logic of a specific type of Character in the Room.
    /// A specific Item must be given to it, to unlock a specific dialog option.
    /// </summary>
    ///
    public class PuzzleCharacter : Character
    {
        private int _itemDependency;
        private bool _dialogUnlocked;
        
        public PuzzleCharacter(int id, 
                               string name, 
                               string pronoun, 
                               string catchPhrase, 
                               bool giveAble,
                               int itemDependency, 
                               bool dialogUnlocked,
                               List<Node> treeStructure,
                               UiDialogManager dialogManager,
                               ThoughtNode thought,
                               MoodStateManager moodStateManager, 
                               Texture2D texture, Vector2 position, int drawOrder, bool collidable = true, int collBoxHeight = 20)
                            : base(id, name, pronoun, catchPhrase, giveAble, treeStructure, dialogManager, thought, moodStateManager, texture, position, drawOrder, collidable, collBoxHeight)
        {
            _itemDependency = itemDependency;
            _dialogUnlocked = dialogUnlocked;
        }

        public override void TalkTo()
        {
            if(_dialogUnlocked == true)
            {
                // _dialogManager.DoDisplayText("I'm happy now!", this);
                _dialogManager.StartDialog(_treeStructure, this);
            }
            else
            {
                _dialogManager.DoDisplayText(_catchPhrase, this);
            }
        }

        public override bool Give(Item item)
        {
            if(item.Id != _itemDependency)
            {
                _dialogManager.DoDisplayText(_pronoun + " is not interessted in that.", this);
                return false;
            }
            else
            {
                _dialogUnlocked = true;
                _dialogManager.DoDisplayText("Thank you!", this);
                return true;
            }
        }        
        public override DataHolderEntity GetDataHolderEntity()
        {
            DataHolderPuzzleCharacter dataHolderEntity = new DataHolderPuzzleCharacter();
            dataHolderEntity = (DataHolderPuzzleCharacter)base.GetDataHolderEntity(dataHolderEntity);
            // PuzzleCharacter
            dataHolderEntity.ItemDependency = _itemDependency;
            dataHolderEntity.DialogUnlocked = _dialogUnlocked;
            return dataHolderEntity;
        }
        public DataHolderEntity GetDataHolderEntity(DataHolderPuzzleCharacter dataHolderEntity)
        {
            dataHolderEntity = (DataHolderPuzzleCharacter)base.GetDataHolderEntity(dataHolderEntity);
            // PuzzleCharacter
            dataHolderEntity.ItemDependency = _itemDependency;
            dataHolderEntity.DialogUnlocked = _dialogUnlocked;
            return dataHolderEntity;
        }
    }
}