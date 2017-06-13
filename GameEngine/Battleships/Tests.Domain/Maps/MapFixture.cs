using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Domain.Games;
using Domain.Maps;
using Domain.Players;
using Domain.Weapons;
using NUnit.Framework;
using Tests.Domain.Maps.Stubs;
using Tests.Domain.Ships.Stubs;

namespace Tests.Domain.Maps
{
    [TestFixture]
    public class MapFixture
    {
        private static Direction[] validShipPlacementDirection =
        {
            Direction.North,
            Direction.East,
            Direction.South,
            Direction.West
        };

        private static Direction[] inValidShipPlacementDirection =
        {
            Direction.NorthEast,
            Direction.SouthEast,
            Direction.SouthWest,
            Direction.NorthWest
        };

        private BattleshipPlayer player;

        [SetUp]
        public void SetUp()
        {
            this.player = new BattleshipPlayer("SomePlayer", 'A', PlayerType.One, 10);
        }

        [Test]
        [TestCaseSource(nameof(validShipPlacementDirection))]
        public void GivenMap_WhenPlacingShip_PlacesShip(Direction direction)
        {
            const int width = 5;
            const int height = 5;
            var map = new PlayerMap(width, height, this.player);
            var coordinate = new Point(width / 2, height / 2);
            var ship = ShipStub.FullStub(this.player, new WeaponStub(player, 0, WeaponType.SingleShot));

            map.Place(ship, coordinate, direction);

            var cellOccupants = map.Cells.Where(x => x.Occupied).Select(x => x.OccupiedBy).ToList();

            foreach(var segment in ship.Cells)
            {
                Assert.Contains(segment, map.Cells.ToList());
            }
        }

        [Test]
        public void GivenMap_WhenPlacingShipOfOtherPlayer_ThrowsException()
        {
            const int width = 5;
            const int height = 5;
            var map = new PlayerMap(width, height, this.player);
            var coordinate = new Point(width / 2, height / 2);
            var otherPlayer = new BattleshipPlayer("OtherPlayer", 'A', PlayerType.One, 10);
            var ship = ShipStub.FullStub(otherPlayer, new WeaponStub(player, 1, WeaponType.SingleShot));

            Assert.Throws<InvalidOperationException>(() => map.Place(ship, coordinate, Direction.East));
        }

        [Test]
        public void GivenMap_WhenShootingWeaponAtCoordinatesInMap_CallsShootOnWeapon()
        {
            const int width = 5;
            const int height = 5;
            var map = new PlayerMap(width, height, this.player);
            var coordinate = new Point(width / 2, height / 2);
            var otherPlayer = new BattleshipPlayer("OtherPlayer", 'A', PlayerType.One, 10);
            var weapon = new WeaponStub(otherPlayer, 0, WeaponType.SingleShot);

            map.Shoot(weapon, new List<Point> {coordinate}, 1);

            Assert.True(weapon.ShootCalled);
            Assert.NotNull(weapon.Targets);
        }

        [Test]
        public void GivenMap_WhenShootingWeaponAtOwnMap_ThrowsException()
        {
            const int width = 5;
            const int height = 5;
            var map = new PlayerMap(width, height, this.player);
            var coordinate = new Point(width / 2, height / 2);
            var weapon = new WeaponStub(this.player,1, WeaponType.SingleShot);

            Assert.Throws<InvalidOperationException>(() => map.Shoot(weapon, new List<Point> { coordinate }, 1));
        }

        [Test]
        public void GivenMap_WhenShootingWeaponAtCoordinatesOutsideOfMap_ThrowsException()
        {
            const int width = 5;
            const int height = 5;
            var map = new PlayerMap(width, height, this.player);
            var coordinate = new Point(-1, -1);
            var otherPlayer = new BattleshipPlayer("OtherPlayer", 'A', PlayerType.One, 10);
            var weapon = new WeaponStub(otherPlayer, 0, WeaponType.SingleShot);

            Assert.Throws<ArgumentException>(() => map.Shoot(weapon, new List<Point> { coordinate }, 1));
        }

        [Test]
        [TestCaseSource(nameof(validShipPlacementDirection))]
        public void GivenMap_WhenPlacingShipWithDirectionLeadingToOutOfBounds_DoesAddShipToCells(Direction direction)
        {
            const int width = 1;
            const int height = 1;
            var map = new PlayerMap(width, height, this.player);
            var coordinate = new Point(width / 2, height / 2);
            var ship = ShipStub.FullStub(this.player, new WeaponStub(player, 1, WeaponType.SingleShot));

            try
            {
                map.Place(ship, coordinate, direction);
            }
            catch(Exception)
            {
                //No-Op
            }

            var cellsWithOccupants = map.Cells.Where(x => x.Occupied);

            Assert.IsEmpty(cellsWithOccupants);
        }

        [Test]
        [TestCaseSource(nameof(validShipPlacementDirection))]
        public void GivenMap_WhenPlacingShipWithDirectionLeadingToOutOfBounds_ThrowsException(Direction direction)
        {
            const int width = 1;
            const int height = 1;
            var map = new PlayerMap(width, height, this.player);
            var coordinate = new Point(width / 2, height / 2);
            var ship = ShipStub.FullStub(this.player, new WeaponStub(player, 1, WeaponType.SingleShot));

            Assert.Throws<InvalidOperationException>(() => map.Place(ship, coordinate, direction));
        }

        [Test]
        [TestCaseSource(nameof(inValidShipPlacementDirection))]
        public void GivenMap_WhenPlacingShipWithInvalidDirection_ThrowsException(Direction direction)
        {
            const int width = 5;
            const int height = 5;
            var map = new PlayerMap(width, height, this.player);
            var coordinate = new Point(width / 2, height / 2);
            var ship = ShipStub.FullStub(this.player, new WeaponStub(player, 1, WeaponType.SingleShot));

            Assert.Throws<InvalidOperationException>(() => map.Place(ship, coordinate, direction));
        }

        [Test]
        public void GivenMapWithShipPlaced_WhenPlacingShipOverlappingOtherShip_DoesNotAddShipToCells()
        {
            const int width = 5;
            const int height = 5;
            var map = new PlayerMap(width, height, this.player);
            var coordinate = new Point(width / 2, height / 2);
            var ship = ShipStub.FullStub(this.player, new WeaponStub(player, 1, WeaponType.SingleShot));
            map.Place(ship, coordinate, Direction.East);

            var overLappingShip = ShipStub.FullStub(this.player, new WeaponStub(player, 1, WeaponType.SingleShot));

            try
            {
                map.Place(overLappingShip, coordinate, Direction.East);
            }
            catch(Exception)
            {
                //No-Op
            }

            var cellsWithOccupants = map.Cells.Where(x => x.Occupied);

            foreach(var cellsWithOccupant in cellsWithOccupants)
            {
                Assert.That(!overLappingShip.Cells.Contains(cellsWithOccupant));
            }
        }

        [Test]
        public void GivenMapWithShipPlaced_WhenPlacingShipOverlappingOtherShip_ThrowsException()
        {
            const int width = 5;
            const int height = 5;
            var map = new PlayerMap(width, height, this.player);
            var coordinate = new Point(width / 2, height / 2);
            var ship = ShipStub.FullStub(this.player, new WeaponStub(player, 1, WeaponType.SingleShot));
            map.Place(ship, coordinate, Direction.West);

            var overLappingShip = ShipStub.FullStub(this.player, new WeaponStub(player, 1, WeaponType.SingleShot));

            Assert.Throws<InvalidOperationException>(() => map.Place(overLappingShip, coordinate, Direction.East));
        }

        [Test]
        public void GivenPlayer_WhenConstructing_AddsPlayerAsOwner()
        {
            var owner = new BattleshipPlayer("TestPlayer", 'A', PlayerType.One, 10);
            const int width = 5;
            const int height = 5;

            var map = new PlayerMap(width, height, owner);

            Assert.AreEqual(owner, map.Owner);
        }

        [Test]
        [TestCase(-5)]
        [TestCase(0)]
        public void GivenPositiveWidthAndZeroOrNegativeHeight_WhenConstructing_ThrowsInvalidArgumentException(int height)
        {
            const int width = 5;

            Assert.Throws<ArgumentException>(() => new PlayerMap(width, height, this.player));
        }

        [Test]
        public void GivenWidthAndHeight_WhenConstructing_CreatesMapWithCorrectNumberOfCells()
        {
            const int width = 5;
            const int height = 5;

            var map = new PlayerMap(width, height, this.player);

            Assert.AreEqual(width * height, map.Cells.Count());
        }

        [Test]
        [TestCase(-5)]
        [TestCase(0)]
        public void GivenZeroOrNegativeWidthAndPositiveHeight_WhenConstructing_ThrowsInvalidArgumentException(int width)
        {
            const int height = 5;

            Assert.Throws<ArgumentException>(() => new PlayerMap(width, height, this.player));
        }
    }
}
