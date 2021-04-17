using System;

namespace conscious
{
    public class VerbActionEventArgs : EventArgs
    {
        public int ThingId { get; set; }
        public Verb verbAction { get; set; }
    }
}