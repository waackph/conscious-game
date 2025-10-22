using System;

namespace conscious
{
    /// <summary>Class <c>RoomChangeEvent</c> holds EventArgs for the event 
    /// that the room changes.
    /// </summary>
    ///
    public class RoomChangeEvent : EventArgs
    {
        public int RoomId { get; set; }
    }
}