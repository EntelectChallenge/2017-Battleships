using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReferenceBot.Domain.Command;
using ReferenceBot.Domain.State;

namespace ReferenceBot.Strategy
{
    public class BasicShootStrategy
    {
        public Command ExecuteStrategy(GameState gameState)
        {
            return RandomShotCommand(gameState);
        }
        
        private Command RandomShotCommand(GameState gameState)
        {
            var cells = gameState.OpponentMap.Cells;
            var lastShot = cells.OrderBy(cell => cell.Y).ThenBy(cell => cell.X).LastOrDefault(cell => cell.Damaged || cell.Missed);
            if (lastShot == null)
            {
                //We have not made any shots yet, shot in the first coordinate
                return new Command(Code.FireShot, 0, 0);
            }

            
            var x = lastShot.X;
            var y = lastShot.Y;

            if (x + 2 < gameState.PlayerMap.MapWidth)
            {
                x += 2;
            }
            else
            {
                x = 0;
                y++;
            }

            if (y >= gameState.PlayerMap.MapWidth)
            {
                return AlternateRandomShot(gameState);
            }

            return new Command(Code.FireShot, x, y);
        }

        private Command AlternateRandomShot(GameState gameState)
        {
            var cells = gameState.OpponentMap.Cells;
            var availableCell = cells.FirstOrDefault(cell => !cell.Damaged && !cell.Missed);

            if (availableCell == null)
            {
                var random = new Random();
                return new Command(Code.FireShot, random.Next(0, gameState.PlayerMap.MapWidth), random.Next(gameState.PlayerMap.MapHeight));
            }
            return new Command(Code.FireShot, availableCell.X, availableCell.Y);
        }
    }
}
