using System;
using Domain.Games;

namespace GameEngine.Renderers
{
    public class ConsoleRender : GameMapRender
    {
        public ConsoleRender(GameMap gameMap) : base(gameMap)
        {
        }
        public static void RenderToConsolePretty(GameMap gameMap, PlayerType playerType)
        {
            Console.Clear();
            var render = new ConsoleRender(gameMap);
            var inMap = false;
            foreach (var character in render.RenderTextGameState(playerType, false, true).ToString())
            {
                if (character == 'X' || character == '-')
                {
                    inMap = false;
                }
                if (character == '#')
                {
                    inMap = true;
                }
                if (character == '|')
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                if (character == '!')
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                if (character == '~' && inMap)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                if (char.IsLower(character) && inMap)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                }
                if (char.IsUpper(character) && inMap)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.Write(character);

                Console.ResetColor();
            }
        }
    }
}