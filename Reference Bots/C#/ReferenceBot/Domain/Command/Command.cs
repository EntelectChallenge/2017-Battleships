using System.Drawing;
using System.Reflection;

namespace ReferenceBot.Domain.Command
{
    public class Command
    {
        public Command(Code code, int x, int y, Direction direction = Direction.None)
        {
            CommandCode = code;
            Coordinate = new Point(x, y);
            if (direction != Direction.None)
            {
                Direction = direction;
            }
        }

        public Code CommandCode { get; set; }

        public Point Coordinate { get; set; }

        public Direction Direction { get; set; }
        public override string ToString()
        {
            return $"{(int)CommandCode},{Coordinate.X},{Coordinate.Y}";
        }
    }
}