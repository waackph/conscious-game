using System;

namespace conscious
{
    /// <summary>Class <c>FinalEdgeEventArgs</c> holds EventArgs for the event of the player 
    /// clicking a final option (FinalThoughtLink) in the thought-dialog.
    /// It may lead to a verb being executed (interaction with an object)
    /// a sequence being played and the state of the protagonists mood changed.
    /// </summary>
    ///
    public class FinalEdgeEventArgs : EventArgs
    {
        public Verb verbAction { get; set; }
        public Sequence seq { get; set; }
        public MoodState EdgeMood { get; set; }
        public ThoughtNode EventThought { get; set; }
    }
}