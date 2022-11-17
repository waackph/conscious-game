using System;

namespace conscious
{
    /// <summary>Class <c>MoodStateChangeEventArgs</c> holds EventArgs for the event 
    /// that the state of the protagonists mood changed.
    /// </summary>
    ///
    public class MoodStateChangeEventArgs : EventArgs
    {
        public MoodState CurrentMoodState { get; set; }
        public Direction ChangeDirection { get; set; }
    }
}