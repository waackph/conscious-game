using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace conscious
{
    public class Thing : Entity
    {
        private Dictionary<MoodState, Texture2D> _moodTextures;
        protected MoodStateManager _moodStateManager;

        public ThoughtNode Thought { get; protected set; }
        public int Id { get; protected set; }
        public bool IsInInventory { get; set; }
        

        public Thing(int id, ThoughtNode thought, MoodStateManager moodStateManager,
                     string name, Texture2D texture, Vector2 position) : base(name, texture, position)
        {
            _moodStateManager = moodStateManager;
            Thought = thought;
            if(Thought != null && name != "")
            {
                Thought.Thought = "[" + name + "] " + Thought.Thought;
            }
            Id = id;
            IsInInventory = false;

            // Standard case for mood dependent textures
            _moodTextures = new Dictionary<MoodState, Texture2D>()
            {
                { MoodState.None, texture }
            };
            _moodStateManager.MoodChangeEvent += changeTextureOnMood;
            Texture2D moodTexture = getMoodTexture(_moodStateManager.moodState);
            UpdateTexture(moodTexture);
        }

        public void AddMoodTexture(MoodState moodState, Texture2D moodTexture)
        {
            _moodTextures[moodState] = moodTexture;
        }

        private void changeTextureOnMood(object sender, MoodState e)
        {
            Texture2D moodTexture = getMoodTexture(e);
            UpdateTexture(moodTexture);
        }

        private Texture2D getMoodTexture(MoodState moodState)
        {
            if(_moodTextures.ContainsKey(moodState))
            {
                return _moodTextures[moodState];
            }
            else
            {
                return _moodTextures[MoodState.None];
            }
        }

        public override DataHolderEntity GetDataHolderEntity()
        {
            DataHolderThing dataHolderEntity = new DataHolderThing();
            dataHolderEntity = (DataHolderThing)base.GetDataHolderEntity(dataHolderEntity);
            dataHolderEntity.Thought = Thought;
            dataHolderEntity.IsInInventory = IsInInventory;
            return dataHolderEntity;
        }
        
        public DataHolderEntity GetDataHolderEntity(DataHolderThing dataHolderEntity)
        {
            dataHolderEntity = (DataHolderThing)base.GetDataHolderEntity(dataHolderEntity);
            dataHolderEntity.Id = Id;
            dataHolderEntity.Thought = Thought;
            dataHolderEntity.IsInInventory = IsInInventory;
            return dataHolderEntity;
        }
    }
}