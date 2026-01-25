using System;

namespace conscious
{
    /// <summary>Class <c>ThoughtFinishedEvent</c> holds EventArgs for the event 
    /// that a thought has been finished.
    /// </summary>
    ///
    public class ThoughtFinishedEvent : EventArgs
    {
        public int RootThoughtId { get; set; }
    }
}