using System;
using Arena.Game;

namespace Arena.Game
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                using (var game = new Game1())
                    game.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("=== CRASH ===");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}