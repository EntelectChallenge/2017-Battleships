using System;
using System.Collections.Generic;
using System.Drawing;
using Domain.Games;
using Domain.Maps;
using Domain.Ships;

namespace TestHarness.TestHarnesses.Bot
{
    public class StringToPlaceShipCommand
    {
        public List<ShipType> Ships { get; set; }
        public List<Point> Points { get; set; }
        public List<Direction> Directions { get; set; }

        public StringToPlaceShipCommand(string[] commands)
        {
            ExtractShips(commands);
            ExtractPoints(commands);
            ExtractDirections(commands);
        }

        private void ExtractShips(string[] commands)
        {
            Ships = new List<ShipType>();
            foreach (var command in commands)
            {
                var ship = command.Split(' ')[0];

                Ships.Add(ConvertToShipType(ship));
            }
        }

        private void ExtractPoints(string[] commands)
        {
            Points = new List<Point>();
            foreach (var command in commands)
            {
                var x = Convert.ToInt32(command.Split(' ')[1]);
                var y = Convert.ToInt32(command.Split(' ')[2]);
                Points.Add(new Point(x, y));
            }
        }

        private void ExtractDirections(string[] commands)
        {
            Directions = new List<Direction>();
            foreach (var command in commands)
            {
                var direction = command.Split(' ')[3];
                Directions.Add(ConvertoToDirection(direction));
            }
        }

        private ShipType ConvertToShipType(string shipType)
        {
            var lowerCase = shipType.ToLower();
            switch (lowerCase)
            {
                case "battleship":
                    return ShipType.Battleship;
                case "cruiser":
                    return ShipType.Cruiser;
                case "carrier":
                    return ShipType.Carrier;
                case "destroyer":
                    return ShipType.Destroyer;
                case "submarine":
                    return ShipType.Submarine;
                default:
                    throw new ArgumentException("Invalid ship type: " + lowerCase);
            }
        }

        public static Direction ConvertoToDirection(string direction)
        {
            var lowerCase = direction.ToLower();
            lowerCase = lowerCase.Replace("\r","");
            switch (lowerCase)
            {
                case "north":
                    return Direction.North;
                case "east":
                    return Direction.East;
                case "south":
                    return Direction.South;
                case "west":
                    return Direction.West;
                default:
                    throw new ArgumentException("Invalid direction: " + lowerCase);
            }
        }
    }
}