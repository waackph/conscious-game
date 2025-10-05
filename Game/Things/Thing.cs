using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace conscious
{
    /// <summary>Class <c>Thing</c> holds data and logic for an entity visible in a Room.
    /// </summary>
    ///
    public class Thing : Entity
    {
        private Dictionary<MoodState, Texture2D> _moodTextures;
        protected MoodStateManager _moodStateManager;

        public ThoughtNode Thought { get; protected set; }
        public ThoughtNode EventThought { get; protected set; }
        public int Id { get; protected set; }
        public bool IsInInventory { get; set; }
        public Texture2D LightMask { get; protected set; }
        

        public Thing(int id, ThoughtNode thought, MoodStateManager moodStateManager,
                     string name, Texture2D texture, Vector2 position, int drawOrder, 
                     bool collidable = false, int collBoxHeight = 20,
                     ThoughtNode eventThought = null, Texture2D lightMask = null,
                     Texture2D depressedTexture = null, Texture2D manicTexture = null,
                     bool isActive = true) 
                     : base(name, texture, position, drawOrder, collidable, collBoxHeight, isActive)
        {
            _moodStateManager = moodStateManager;
            Thought = thought;
            if(Thought != null && name != "")
            {
                Thought.Thought = "[" + name + "] " + Thought.Thought;
            }
            EventThought = eventThought;
            Id = id;
            IsInInventory = false;
            LightMask = lightMask;

            // Standard case for mood dependent textures
            _moodTextures = new Dictionary<MoodState, Texture2D>()
            {
                { MoodState.None, texture },
                { MoodState.Depressed, depressedTexture },
                { MoodState.Manic, manicTexture },
            };
            _moodStateManager.MoodChangeEvent += changeTextureOnMood;
            Texture2D moodTexture = getMoodTexture(_moodStateManager.moodState);
            UpdateTexture(moodTexture);
        }

        public void AddMoodTexture(MoodState moodState, Texture2D moodTexture)
        {
            _moodTextures[moodState] = moodTexture;
        }

        private void changeTextureOnMood(object sender, MoodStateChangeEventArgs e)
        {
            Texture2D moodTexture = getMoodTexture(e.CurrentMoodState);
            UpdateTexture(moodTexture);
        }

        private Texture2D getMoodTexture(MoodState moodState)
        {
            if(_moodTextures.ContainsKey(moodState) && _moodTextures[moodState] != null)
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
            dataHolderEntity.Thought = Thought?.GetDataHolderThoughtNode();
            dataHolderEntity.EventThought = EventThought?.GetDataHolderThoughtNode();
            dataHolderEntity.IsInInventory = IsInInventory;
            dataHolderEntity.LightMaskFilePath = LightMask?.ToString();
            if(_moodTextures.ContainsKey(MoodState.Depressed))
                dataHolderEntity.DepressedTexture = _moodTextures[MoodState.Depressed]?.ToString();
            if(_moodTextures.ContainsKey(MoodState.Manic))
                dataHolderEntity.ManicTexture = _moodTextures[MoodState.Manic]?.ToString();
            return dataHolderEntity;
        }

        public DataHolderEntity GetDataHolderEntity(DataHolderThing dataHolderEntity)
        {
            dataHolderEntity = (DataHolderThing)base.GetDataHolderEntity(dataHolderEntity);
            dataHolderEntity.Id = Id;
            dataHolderEntity.Thought = Thought?.GetDataHolderThoughtNode();
            dataHolderEntity.EventThought = EventThought?.GetDataHolderThoughtNode();
            dataHolderEntity.IsInInventory = IsInInventory;
            dataHolderEntity.LightMaskFilePath = LightMask?.ToString();
            if(_moodTextures.ContainsKey(MoodState.Depressed))
                dataHolderEntity.DepressedTexture = _moodTextures[MoodState.Depressed]?.ToString();
            if(_moodTextures.ContainsKey(MoodState.Manic))
                dataHolderEntity.ManicTexture = _moodTextures[MoodState.Manic]?.ToString();
            return dataHolderEntity;
        }
    }
}