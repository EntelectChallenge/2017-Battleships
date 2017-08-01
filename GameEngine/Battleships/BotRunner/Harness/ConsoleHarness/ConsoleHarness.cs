using System;
using System.Drawing;
using System.Linq;
using System.Text;

using Domain.Games;
using Domain.Maps;
using Domain.Ships;
using GameEngine.Commands;
using GameEngine.Commands.PlayerCommands;
using GameEngine.Common;
using GameEngine.Renderers;
using TestHarness.TestHarnesses.Bot;

namespace BotRunner.Harness.ConsoleHarness
{
    public class ConsoleHarness : Player
    {
        public ConsoleHarness(string name) : base(name)
        {
        }

        public override void StartGame(GameMap gameState)
        {
            NewRoundStarted(gameState);
        }

        public override void NewRoundStarted(GameMap gameState)
        {
            ConsoleRender.RenderToConsolePretty(gameState, BattleshipPlayer.PlayerType);
            if (gameState.Phase == 1)
            {
                try
                {
                    FirstRoundCommand();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    Console.WriteLine("Please try again");
                    Console.WriteLine();
                    FirstRoundCommand();
                }
            }
            else
            {
                try
                {
                    GeneralCommands();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    Console.WriteLine("Please try again");
                    Console.WriteLine();
                    GeneralCommands();
                }
            }
        }

        private void FirstRoundCommand()
        {
            int index = 0;
            Console.WriteLine("Please place your ships in the following format <Shipname> <x> <y> <direction>");
            Console.WriteLine("Ship names: Battleship, Cruiser, Carrier, Destroyer, Submarine");
            Console.WriteLine("Directions: north east south west");
            Console.WriteLine();
            Console.WriteLine("Ship placement for player " + Name);
            Console.WriteLine();
            var ships = new string[5];
            while (index < 5)
            {
                ships[index] = Console.ReadLine();
                index++;
                Console.WriteLine("Ship number " + index + " has been added to the queue");
            }
            Console.WriteLine("All Ships have been added to the queue to be placed for player " + Name);
            var command = new StringToPlaceShipCommand(ships);
            PublishCommand(new PlaceShipCommand(command.Ships, command.Points, command.Directions));
        }

        private void GeneralCommands()
        {
            Console.WriteLine();
            Console.WriteLine("Command from player " + Name + " pending");
            Console.WriteLine("To send through a command please pass through the following <code>,<x>,<y>");
            Console.WriteLine("Possible codes: " +
                              "\n0 - Do Nothing (please pass through coordinates still)" +
                              "\n1 - Fireshot, " +
                              "\n2 - Fire Double Shot Vertical" +
                              "\n3 - Fire Corner Shot Horizontal" +
                              "\n4 - Fire Corner Shot" +
                              "\n5 - Fire Horizontal Cross Shot" +
                              "\n6 - Fire Diagonal Cross Shot" +
                              "\n7 - Fire Seeker Missile" +
                              "\n8 - Place shield");
            Console.WriteLine();
            string line = null;
            while (string.IsNullOrEmpty(line))
            {
                line = Console.ReadLine();
                if (string.IsNullOrEmpty(line)) continue;
                if (line.Split(',').Length >= 4)
                {
                    line = null;
                }
            }
            var code = Convert.ToInt32(line.Split(',')[0]);
            switch (code)
            {
                case 1:
                    PublishCommand(new FireSingleShotCommand(new Point(Convert.ToInt32(line.Split(',')[1]), Convert.ToInt32(line.Split(',')[2]))));
                    break;
                case 2:
                    //Vertical Double Shot
                    PublishCommand(new FireDoubleShotCommand(new Point(Convert.ToInt32(line.Split(',')[1]), Convert.ToInt32(line.Split(',')[2])),Direction.North));
                    break;
                case 3:
                    //Horizontal Double Shot
                    PublishCommand(new FireDoubleShotCommand(new Point(Convert.ToInt32(line.Split(',')[1]), Convert.ToInt32(line.Split(',')[2])), Direction.East));
                    break;
                case 4:
                    PublishCommand(new FireCornerrShotCommand(new Point(Convert.ToInt32(line.Split(',')[1]), Convert.ToInt32(line.Split(',')[2]))));;
                    break;
                case 5:
                    //Diagonal cross shot
                    PublishCommand(new FireCrossShotCommand(new Point(Convert.ToInt32(line.Split(',')[1]), Convert.ToInt32(line.Split(',')[2])), true));
                    break;
                case 6:
                    //Horizontal and vertical cross shot
                    PublishCommand(new FireCrossShotCommand(new Point(Convert.ToInt32(line.Split(',')[1]), Convert.ToInt32(line.Split(',')[2])), false));
                    break;
                case 7:
                    PublishCommand(new FireSeekerMissileCommand(new Point(Convert.ToInt32(line.Split(',')[1]), Convert.ToInt32(line.Split(',')[2]))));
                    break;
                case 8:
                    PublishCommand(new PlaceShieldCommand(new Point(Convert.ToInt32(line.Split(',')[1]), Convert.ToInt32(line.Split(',')[2]))));
                    break;
                default:
                    PublishCommand(new DoNothingCommand());
                    break;
            }
        }

        public override void GameEnded(GameMap gameMap)
        {
        }

        public override void PlayerKilled(GameMap gameMap)
        {
            System.Console.WriteLine("Player " + Name + " has been killed");
        }

        public override void PlayerCommandFailed(ICommand command, string reason)
        {
            System.Console.WriteLine("Could not process player command: " + reason);
        }

        public override void Dispose()
        {
        }

        public override void FirstRoundFailed(GameMap gameMap)
        {
            Console.WriteLine(gameMap.ReasonForFirstRoundFailure);
            Console.WriteLine("The first round has failed, due to one of the player's ships not being placed.");
            Console.WriteLine("The round will now restart and both players will have to try again");
            Console.WriteLine("Press any key to continue");
            Console.Read();
        }
    }
}