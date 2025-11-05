using System;

namespace conscious
{
    public class ThoughtEventFinished : EventArgs
    {
        public int ThoughtEventId { get; set; }
    }
}