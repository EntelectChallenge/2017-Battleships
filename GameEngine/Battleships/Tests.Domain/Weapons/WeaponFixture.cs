using System.Collections.Generic;
using System.Drawing;
using Domain.Games;
using Domain.Maps;
using Domain.Players;
using Domain.Ships;
using Domain.Weapons;
using NUnit.Framework;
using Tests.Domain.Weapons.Stubs;

namespace Tests.Domain.Weapons
{
    [TestFixture]
    public class WeaponFixture
    {
        private readonly int width;
        private readonly int height;

        private Dictionary<Point, Cell> cells;
        private BattleshipPlayer player;

        [SetUp]
        public void SetUp()
        {
            this.cells = new Dictionary<Point, Cell>();

            for(var x = 0; x < this.width; x++)
            {
                for(var y = 0; y < this.height; y++)
                {
                    var coordinate = new Point(x, y);
                    var cell = new Cell(coordinate, this.cells);

                    this.cells.Add(coordinate, cell);
                }
            }

            this.player = new BattleshipPlayer("TestPlayer", 'A', PlayerType.One);
        }


        public WeaponFixture()
        {
            this.width = 10;
            this.height = 10;
        }
    }
}
