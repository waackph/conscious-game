using System;

namespace conscious
{
    public class ThoughtEventTriggered : EventArgs
    {
        public int ThoughtEventId { get; set; }
    }
}