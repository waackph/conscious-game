using System;

namespace conscious
{
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
