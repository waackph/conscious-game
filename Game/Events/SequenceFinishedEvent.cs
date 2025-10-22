using System;

namespace conscious
{
    /// <summary>Class <c>SequenceFinishedEvent</c> holds EventArgs for the event 
    /// that an event finished.
    /// </summary>
    ///
    public class SequenceFinishedEvent : EventArgs
    {
        public int sequenceCommandThingId { get; set; }
    }
}