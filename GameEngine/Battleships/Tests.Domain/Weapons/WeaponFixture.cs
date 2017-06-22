using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Domain.Games;
using Domain.Maps;
using Domain.Players;
using Domain.Ships;
using Domain.Weapons;
using GameEngine.Commands.PlayerCommands;
using NUnit.Framework;
using Tests.Domain.Maps.Stubs;
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
        private BattleshipPlayer opponent;

        private PlayerMap opponentMap;
        private PlayerMap playerMap;

        private GameMap gameMap;

        [SetUp]
        public void SetUp()
        {
            this.cells = new Dictionary<Point, Cell>();

            for (var x = 0; x < this.width; x++)
            {
                for (var y = 0; y < this.height; y++)
                {
                    var coordinate = new Point(x, y);
                    var cell = new Cell(coordinate, this.cells);

                    this.cells.Add(coordinate, cell);
                }
            }

            this.gameMap = new GameMap("TestPlayer", "OpponentPlayer", width, height);
            gameMap.Phase = 2;

            this.player = this.gameMap.GetBattleshipPlayer(PlayerType.One);
            this.opponent = this.gameMap.GetBattleshipPlayer(PlayerType.Two);

            this.playerMap = this.gameMap.GetPlayerMap(PlayerType.One);
            this.opponentMap = this.gameMap.GetPlayerMap(PlayerType.Two);

            PlacePlayerShips();

            this.gameMap.Place(PlayerType.Two, ShipType.Battleship, new Point(1, 0), Direction.East);
            this.gameMap.Place(PlayerType.Two, ShipType.Carrier, new Point(0, 0), Direction.North);

            this.player.Energy = 50;
        }

        [Test]
        public void SingleShotHit()
        {
            var singleShotCommand = new FireSingleShotCommand(new Point(0, 0));
            singleShotCommand.PerformCommand(gameMap, player);

            var firstCell = opponentMap.Cells.First(cell => cell.X == 0 && cell.Y == 0);

            Assert.IsTrue(firstCell.Damaged);
            Assert.IsTrue(player.Energy == 49);
        }

        [Test]
        public void SingleShotMissed()
        {
            var singleShotCommand = new FireSingleShotCommand(new Point(5, 5));
            singleShotCommand.PerformCommand(gameMap, player);

            var firstCell = opponentMap.Cells.First(cell => cell.X == 5 && cell.Y == 5);

            Assert.IsTrue(firstCell.Missed);
            Assert.IsTrue(player.Energy == 49);
        }

        [Test]
        public void DoubleShotVertHit()
        {
            var doubleShotVertCommand = new FireDoubleShotCommand(new Point(0, 1), Direction.North);
            doubleShotVertCommand.PerformCommand(gameMap, player);

            var cellsFiredAt =
                opponentMap.Cells.Where(cell => (cell.X == 0 && cell.Y == 0) || (cell.X == 0 && cell.Y == 2));

            foreach (var cell in cellsFiredAt)
            {
                Assert.IsTrue(cell.Damaged);
            }
            Assert.IsTrue(player.Energy == 26);
        }

        [Test]
        public void DoubleShotVertMiss()
        {
            var doubleShotVertCommand = new FireDoubleShotCommand(new Point(2, 2), Direction.North);
            doubleShotVertCommand.PerformCommand(gameMap, player);

            var cellsFiredAt =
                opponentMap.Cells.Where(cell => (cell.X == 2 && cell.Y == 1) || (cell.X == 2 && cell.Y == 3));

            foreach (var cell in cellsFiredAt)
            {
                Assert.IsTrue(cell.Missed);
            }
            Assert.IsTrue(player.Energy == 26);
        }

        [Test]
        public void DoubleShotHoriHit()
        {
            var doubleShotVertCommand = new FireDoubleShotCommand(new Point(1, 0), Direction.East);
            doubleShotVertCommand.PerformCommand(gameMap, player);

            var cellsFiredAt =
                opponentMap.Cells.Where(cell => (cell.X == 0 && cell.Y == 0) || (cell.X == 2 && cell.Y == 0));

            foreach (var cell in cellsFiredAt)
            {
                Assert.IsTrue(cell.Damaged);
            }
            Assert.IsTrue(player.Energy == 26);
        }

        [Test]
        public void DoubleShotHoriMiss()
        {
            var doubleShotVertCommand = new FireDoubleShotCommand(new Point(2, 2), Direction.East);
            doubleShotVertCommand.PerformCommand(gameMap, player);

            var cellsFiredAt =
                opponentMap.Cells.Where(cell => (cell.X == 1 && cell.Y == 2) || (cell.X == 3 && cell.Y == 2));

            foreach (var cell in cellsFiredAt)
            {
                Assert.IsTrue(cell.Missed);
            }
            Assert.IsTrue(player.Energy == 26);
        }

        [Test]
        public void CornerShotHitAndMiss()
        {
            var fireCornerrShotCommand = new FireCornerrShotCommand(new Point(1, 1));
            fireCornerrShotCommand.PerformCommand(gameMap, player);

            var topLeft = opponentMap.Cells.First(cell => cell.X == 0 && cell.Y == 2);
            var topRight = opponentMap.Cells.First(cell => cell.X == 2 && cell.Y == 2);
            var botRight = opponentMap.Cells.First(cell => cell.X == 2 && cell.Y == 0);
            var botLeft = opponentMap.Cells.First(cell => cell.X == 0 && cell.Y == 0);

            Assert.IsTrue(topLeft.Damaged);
            Assert.IsTrue(botRight.Damaged);
            Assert.IsTrue(botLeft.Damaged);
            Assert.IsTrue(topRight.Missed);

            Assert.IsTrue(player.Energy == 20);
        }

        [Test]
        public void DiagonalCrossShotHitAndMiss()
        {
            var fireCrossShotCommand = new FireCrossShotCommand(new Point(1, 1), true);
            fireCrossShotCommand.PerformCommand(gameMap, player);

            var topLeft = opponentMap.Cells.First(cell => cell.X == 0 && cell.Y == 2);
            var topRight = opponentMap.Cells.First(cell => cell.X == 2 && cell.Y == 2);
            var botRight = opponentMap.Cells.First(cell => cell.X == 2 && cell.Y == 0);
            var botLeft = opponentMap.Cells.First(cell => cell.X == 0 && cell.Y == 0);
            var middle = opponentMap.Cells.First(cell => cell.X == 1 && cell.Y == 1);

            Assert.IsTrue(topLeft.Hit);
            Assert.IsTrue(botRight.Hit);
            Assert.IsTrue(botLeft.Hit);
            Assert.IsTrue(topRight.Missed);
            Assert.IsTrue(middle.Missed);

            Assert.IsTrue(player.Energy == 14);
        }

        [Test]
        public void VerticalCrossShotHitAndMiss()
        {
            var fireCrossShotCommand = new FireCrossShotCommand(new Point(1, 1), false);
            fireCrossShotCommand.PerformCommand(gameMap, player);

            var top = opponentMap.Cells.First(cell => cell.X == 1 && cell.Y == 2);
            var right = opponentMap.Cells.First(cell => cell.X == 2 && cell.Y == 1);
            var bottom = opponentMap.Cells.First(cell => cell.X == 1 && cell.Y == 0);
            var left = opponentMap.Cells.First(cell => cell.X == 0 && cell.Y == 1);
            var middle = opponentMap.Cells.First(cell => cell.X == 1 && cell.Y == 1);

            Assert.IsTrue(top.Missed);
            Assert.IsTrue(right.Missed);
            Assert.IsTrue(bottom.Hit);
            Assert.IsTrue(left.Hit);
            Assert.IsTrue(middle.Missed);

            Assert.IsTrue(player.Energy == 8);
        }

        [Test]
        public void SeekerMissleHit()
        {
            var fireSeekerMissleCommand = new FireSeekerMissleCommand(new Point(1, 1));
            fireSeekerMissleCommand.PerformCommand(gameMap, player);

            var cellHit = opponentMap.Cells.First(cell => cell.X == 0 && cell.Y == 1);
            var cellNotHit = opponentMap.Cells.First(cell => cell.X == 1 && cell.Y == 0);

            Assert.IsTrue(cellHit.Damaged);
            Assert.IsFalse(cellNotHit.Hit);

            Assert.IsTrue(player.Energy == 20);
        }

        [Test]
        public void SeekerMissleMiss()
        {
            var fireSeekerMissleCommand = new FireSeekerMissleCommand(new Point(3, 3));
            fireSeekerMissleCommand.PerformCommand(gameMap, player);

            var middleCell = opponentMap.Cells.First(cell => cell.X == 3 && cell.Y == 3);

            Assert.IsTrue(middleCell.Missed);

            Assert.IsTrue(player.Energy == 20);
        }

        public WeaponFixture()
        {
            this.width = 10;
            this.height = 10;
        }

        public void PlacePlayerShips()
        {
            gameMap.Place(PlayerType.One, ShipType.Carrier, new Point(0, 0), Direction.North);
            gameMap.Place(PlayerType.One, ShipType.Cruiser, new Point(1, 0), Direction.North);
            gameMap.Place(PlayerType.One, ShipType.Battleship, new Point(2, 0), Direction.North);
            gameMap.Place(PlayerType.One, ShipType.Submarine, new Point(3, 0), Direction.North);
            gameMap.Place(PlayerType.One, ShipType.Destroyer, new Point(4, 0), Direction.North);
        }
    }
}