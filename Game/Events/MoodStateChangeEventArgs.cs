using System;

namespace conscious
{
    public class MoodStateChangeEventArgs : EventArgs
    {
        public MoodState CurrentMoodState { get; set; }
        public Direction ChangeDirection { get; set; }
    }
}