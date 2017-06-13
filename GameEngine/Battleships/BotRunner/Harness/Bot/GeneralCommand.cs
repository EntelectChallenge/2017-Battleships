using System.Drawing;
using System.Reflection;
using Domain.Games;
using Domain.Maps;

namespace TestHarness.TestHarnesses.Bot
{
    public class GeneralCommand
    {
        public GeneralCommand(int code, int x, int y, string direction)
        {
            this.Code = code;
            this.Point = new Point(x,y);
            if (direction != null)
            {
                this.Direction = StringToPlaceShipCommand.ConvertoToDirection(direction);
            }
        }
        public int Code { get; set; }
        public Point Point {get; set; }
        public Direction Direction { get; set; }
    }
}