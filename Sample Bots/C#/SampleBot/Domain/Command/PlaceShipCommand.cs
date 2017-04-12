using System;
using System.Collections.Generic;
using System.Drawing;

namespace SampleBot.Domain.Command
{
    public class PlaceShipCommand
    {
        public List<Ship.Ship> Ships { get; set; }
        public List<Point> Coordinates { get; set; }
        public List<Direction.Direction> Directions { get; set; }

        public override string ToString()
        {
            var output = "";
            for (var i = 0; i < Ships.Count; i++)
            {
                output += $"{Ships[i]} {Coordinates[i].X} {Coordinates[i].Y} {Directions[i]}";
                if (i + 1 != Ships.Count)
                {
                    output += Environment.NewLine;
                }
            }
            return output;
        }
    }
}