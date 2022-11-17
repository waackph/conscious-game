namespace conscious
{
    /// <summary>Enum <c>Direction</c> is a enumeration of direction of change of the mood state.
    /// Given the order of depressed, regular, manic, down means in direction of depressed
    /// up means in direction of manic.
    /// </summary>
    ///
    public enum Direction
    {
        None,
        Up,
        Down,
        DoubleUp,
        DoubleDown
    }
}