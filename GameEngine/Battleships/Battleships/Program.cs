using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using GameEngine.Engine;
using GameEngine.Loggers;

namespace Battleships
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var options = new Options();
                if (!Parser.Default.ParseArguments(args, options))
                {
                    return;
                }
                Console.WriteLine("Starting new game");

                var game = new BattleshipsGame();
                if (options.ConsoleLogger)
                {
                    game.Logger = new CombinedLogger(new InMemoryLogger(), new ConsoleLogger());
                }
                game.StartNewGame(options);
                if (options.Pretty)
                {
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}