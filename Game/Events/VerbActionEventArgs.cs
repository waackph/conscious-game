using System;

namespace conscious
{
    /// <summary>Class <c>VerbActionEventArgs</c> holds EventArgs for the event 
    /// that a verb is executed. It holds data for the ID of the thing that the protagonist interacts with
    /// as well as the verb that indicates the interaction.
    /// </summary>
    ///
    public class VerbActionEventArgs : EventArgs
    {
        public int ThingId { get; set; }
        public Verb verbAction { get; set; }
    }
}