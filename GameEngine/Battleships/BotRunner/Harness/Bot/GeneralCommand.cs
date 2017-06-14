using System.Drawing;
using System.Reflection;
using Domain.Games;
using Domain.Maps;

namespace TestHarness.TestHarnesses.Bot
{
    public class GeneralCommand
    {
        public GeneralCommand(int code, int x, int y)
        {
            this.Code = code;
            this.Point = new Point(x,y);
        }
        public int Code { get; set; }
        public Point Point {get; set; }
    }
}