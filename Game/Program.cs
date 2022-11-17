using System;

namespace conscious
{
    /// <summary>Class <c>Program</c> is the main / entry point to start the game.
    /// </summary>
    ///
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Vuerbaz())
                game.Run();
        }
    }
}
