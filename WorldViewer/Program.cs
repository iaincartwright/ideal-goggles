using System;

namespace WorldViewer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] a_args)
        {
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
}

