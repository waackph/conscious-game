using System;

namespace conscious
{
    /// <summary>Class <c>RootThoughtSelectedEvent</c> holds EventArgs for the event 
    /// that a root thought is selected.
    /// </summary>
    ///
    public class RootThoughtSelectedEvent : EventArgs
    {
        public int ThingId { get; set; }
    }
}