using System;
using Newtonsoft.Json;

namespace conscious
{
    public class ThoughtLink
    {
        public int Id { get; }
        [JsonProperty]
        private MoodState[] _validMoods;

        public string Option { get; }
        public ThoughtNode NextNode { get; }
        public bool IsLocked { get; set; }
        public bool IsVisited { get; set; }

        public ThoughtLink(int id, 
                           ThoughtNode nextNode, 
                           string option, 
                           bool isLocked, 
                           MoodState[] validMoods)
        {
            Id = id;
            NextNode = nextNode;
            Option = option;
            IsLocked = isLocked;
            IsVisited = false;
            if(validMoods != null && validMoods.Length > 3)
            {
                _validMoods = new MoodState[0];
                throw new OutOfMemoryException("The Mood Array is too long");
            }
            else
            {
                _validMoods = validMoods;
            }
        }

        public bool MoodValid(MoodState mood)
        {
            return Array.Exists(_validMoods, element => element == MoodState.None) || Array.Exists(_validMoods, element => element == mood);
        }
    }
}