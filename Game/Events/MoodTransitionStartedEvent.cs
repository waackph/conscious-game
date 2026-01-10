using System;

namespace conscious
{
    /// <summary>Class <c>MoodTransitionStartedEvent</c> holds EventArgs for the event 
    /// that the mood transition has started.
    /// </summary>
    ///
    public class MoodTransitionStartedEvent : EventArgs
    {
        public MoodState CurrentMoodState { get; set; }
    }
}