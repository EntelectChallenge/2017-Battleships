using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReferenceBot.Domain.Command;

namespace ReferenceBot.Domain.State
{
    public class PlayerMap
    {
        [JsonProperty]
        public BattleshipPlayer Owner { get; set; }
        [JsonProperty]
        public List<Cell> Cells { get; set; }
        [JsonProperty]
        public int MapWidth { get; set; }
        [JsonProperty]
        public int MapHeight { get; set; }

        public Cell GetCellAt(int x, int y)
        {
            return Cells.FirstOrDefault(p => p.X == x && p.Y == y);
        }

        public Cell GetAdjacentCell(Cell cell, Direction direction)
        {
            if (cell == null)
                return null;

            switch (direction)
            {
                case Direction.North:
                    return GetCellAt(cell.X, cell.Y + 1);
                case Direction.South:
                    return GetCellAt(cell.X, cell.Y - 1);
                case Direction.West:
                    return GetCellAt(cell.X - 1, cell.Y);
                case Direction.East:
                    return GetCellAt(cell.X + 1, cell.Y);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public List<Cell> GetAllCellsInDirection(Point startLocation, Direction direction, int length)
        {
            var startCell = GetCellAt(startLocation.X, startLocation.Y);
            var cells = new List<Cell>() {startCell};

            if (startCell == null)
                return cells;

            for (int i = 1; i < length; i++)
            {
                var nextCell = GetAdjacentCell(startCell, direction);
                if(nextCell == null)
                    throw new ArgumentException("Not enough cells for requested length");

                cells.Add(nextCell);
                startCell = nextCell;
            }
            
            return cells;
        }

        public bool HasCellsForDirection(Point startLocation, Direction direction, int length)
        {
            var startCell = GetCellAt(startLocation.X, startLocation.Y);

            if (startCell == null)
                return false;

            for (int i = 1; i < length; i++)
            {
                var nextCell = GetAdjacentCell(startCell, direction);
                if (nextCell == null)
                    return false;
                
                startCell = nextCell;
            }

            return true;
        }
    }
}
