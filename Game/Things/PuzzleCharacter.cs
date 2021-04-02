using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace conscious
{
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
                               DialogManager dialogManager,
                               MoodState moodChange,
                               UIThought thought,
                               Texture2D texture, Vector2 position)
                            : base(id, name, pronoun, catchPhrase, giveAble, treeStructure, dialogManager, moodChange, thought, texture, position)
        {
            _itemDependency = itemDependency;
            _dialogUnlocked = dialogUnlocked;
        }

        public override void TalkTo()
        {
            if(_dialogUnlocked == true)
            {
                _dialogManager.DoDisplayText("I'm happy now!");
            }
            else
            {
                _dialogManager.DoDisplayText(_catchPhrase);
            }
        }

        public override bool Give(Item item)
        {
            if(item.Id != _itemDependency)
            {
                _dialogManager.DoDisplayText(_pronoun + " is not interessted in that.");
                return false;
            }
            else
            {
                _dialogUnlocked = true;
                _dialogManager.DoDisplayText("Thank you!");
                return true;
            }
        }        
        public override DataHolderEntity GetDataHolderEntity()
        {
            DataHolderPuzzleCharacter dataHolderEntity = new DataHolderPuzzleCharacter();
            // Entity
            dataHolderEntity.Name = Name;
            dataHolderEntity.PositionX  = Position.X;
            dataHolderEntity.PositionY = Position.Y;
            dataHolderEntity.Rotation = Rotation;
            dataHolderEntity.texturePath = EntityTexture.ToString();
            //Thing
            dataHolderEntity.Thought = _thought;
            // Character
            dataHolderEntity.Id = Id;
            dataHolderEntity.TreeStructure = _treeStructure;
            dataHolderEntity.Pronoun = _pronoun;
            dataHolderEntity.CatchPhrase = _catchPhrase;
            dataHolderEntity.GiveAble = GiveAble;
            dataHolderEntity.MoodChange = MoodChange;
            // PuzzleCharacter
            dataHolderEntity.ItemDependency = _itemDependency;
            dataHolderEntity.DialogUnlocked = _dialogUnlocked;
            return dataHolderEntity;
        }
    }
}