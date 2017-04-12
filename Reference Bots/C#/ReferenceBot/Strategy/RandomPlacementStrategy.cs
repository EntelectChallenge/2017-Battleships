using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReferenceBot.Domain.Command;
using ReferenceBot.Domain.Command.Ship;
using ReferenceBot.Domain.State;

namespace ReferenceBot.Strategy
{
    public class RandomPlacementStrategy
    {
        public PlaceShipCommand GetShipPlacement(GameState gameState)
        {
            var shipSizes = new Dictionary<ShipType, int>()
            {
                { ShipType.Battleship, 4},
                { ShipType.Carrier, 5},
                { ShipType.Cruiser, 3},
                { ShipType.Destroyer, 2},
                { ShipType.Submarine, 3},
            };

            var placements = new List<ShipPlacement>();

            var random = new Random();
            while (shipSizes.Any())
            {
                var ship = shipSizes.Select(x => new {ShipType = x.Key, Size = x.Value}).FirstOrDefault();
                if(ship == null)
                    continue;

                var location = new Point(random.Next(0, gameState.PlayerMap.MapWidth-1), random.Next(0, gameState.PlayerMap.MapHeight-1));
                Direction direction;

                if (TryToPlace(gameState, ship.Size, location, out direction))
                {
                    placements.Add(new ShipPlacement()
                    {
                        Direction = direction,
                        Location = location,
                        ShipType = ship.ShipType
                    });
                    shipSizes.Remove(ship.ShipType);
                }

            }

            return new PlaceShipCommand()
            {
                Ships = placements.Select(x => x.ShipType).ToList(),
                Coordinates = placements.Select(x => x.Location).ToList(),
                Directions = placements.Select(x => x.Direction).ToList()
            };
        }

        private bool TryToPlace(GameState gameState, int size, Point location, out Direction direction)
        {
            var directions = new[] {Direction.North, Direction.East, Direction.South, Direction.West};
            var rnd = new Random();
            directions = directions.OrderBy(x => rnd.Next()).ToArray();

            foreach (var testDirection in directions)
            {
                if(!gameState.PlayerMap.HasCellsForDirection(location, testDirection, size))
                    continue;

                var cells = gameState.PlayerMap.GetAllCellsInDirection(location, testDirection, size);
                if (cells.Any(y => y.Occupied)) continue;

                cells.ForEach((x) => { x.Occupied = true; });
                direction = testDirection;
                return true;
            }

            direction = Direction.West;
            return false;
        }
    }

    internal class ShipPlacement
    {
        public Point Location { get; set; }

        public Direction Direction { get; set; }
        public ShipType ShipType { get; set; }
    }
}
