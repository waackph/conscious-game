using System;

namespace conscious
{
    public class FinalEdgeEventArgs : EventArgs
    {
        public Verb verbAction { get; set; }
        public Sequence seq { get; set; }
    }
}