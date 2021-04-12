using System;

namespace conscious
{
    public class ThoughtLink
    {
        private int _id;
        private MoodState[] _validMoods;

        public string Option { get; }
        public ThoughtNode NextNode { get; }
        public bool IsLocked { get; set; }

        public ThoughtLink(int id, 
                           ThoughtNode nextNode, 
                           string option, 
                           bool isLocked, 
                           MoodState[] validMoods)
        {
            _id = id;
            NextNode = nextNode;
            Option = option;
            IsLocked = isLocked;
            if(validMoods.Length > 3)
            {
                _validMoods = new MoodState[0];
                throw new OutOfMemoryException("The Mood Array is too long");
            }
            else
            {
                _validMoods = validMoods;
            }
        }
    }
}