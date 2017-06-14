using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Domain.Games;
using Domain.Maps;
using Domain.Players;
using Domain.Ships;
using Domain.Weapons;
using NUnit.Framework;
using Tests.Domain.Maps.Stubs;
using Tests.Domain.Ships.Stubs;

namespace Tests.Domain.Ships
{
    [TestFixture]
    public class ShipFixture
    {
        private BattleshipPlayer player;

        [SetUp]
        public void SetUp()
        {
            this.player = new BattleshipPlayer("SomePlayer", 'A', PlayerType.One, 10);
        }

    [Test]
        public void GivenShip_WhenConstructing_AddsCorrectAmountOfShipSegments()
        {
            const int segmentCount = 4;

            Ship ship = new ShipStub(player, segmentCount, new WeaponStub(player, 1, WeaponType.SingleShot));

            Assert.AreEqual(segmentCount, ship.Cells.Count());
        }

        [Test]
        public void GivenShip_WhenConstructing_AssociatesThePlayerToShip()
        {
            const int segmentCount = 4;

            Ship ship = new ShipStub(player, segmentCount, new WeaponStub(player, 1, WeaponType.SingleShot));

            Assert.AreEqual(player, ship.Owner);
        }

        [Test]
        public void GivenShip_WhenConstruting_SetsDestroyedToFalse()
        {
            const int segmentCount = 4;

            Ship ship = new ShipStub(player, segmentCount, new WeaponStub(player, 1, WeaponType.SingleShot));
            const int width = 5;
            const int height = 5;
            var map = new PlayerMap(width, height, this.player);
            var coordinate = new Point(0, 0);
            ship.Place(coordinate, Direction.East, map);

            Assert.False(ship.Destroyed);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public void GivenShip_WhenConstructingWithNegativeOrZeroSegmentCount_ThrowsException(int segmentCount)
        {
            Assert.Throws<ArgumentException>(() => new ShipStub(player, segmentCount, new WeaponStub(player, 1, WeaponType.SingleShot)));
        }

        [Test]
        public void GivenCarrier_WhenConstructing_CreatesFiveSegements()
        {
            var carrier = new Carrier(player, ShipType.Carrier, new WeaponStub(player, 0, WeaponType.SingleShot));

            Assert.AreEqual(5, carrier.Cells.Count());
        }

        [Test]
        public void GivenBattleship_WhenConstructing_CreatesFourSegments()
        {
            var battleship = new Battleship(player, ShipType.Battleship, new WeaponStub(player, 0, WeaponType.SingleShot));

            Assert.AreEqual(4, battleship.Cells.Count());
        }

        [Test]
        public void GivenCruiser_WhenConstructing_CreatesThreeSegments()
        {
            var cruiser = new Cruiser(player, ShipType.Cruiser, new WeaponStub(player, 0, WeaponType.SingleShot));

            Assert.AreEqual(3, cruiser.Cells.Count());
        }

        [Test]
        public void GivenSubmarine_WhenConstructing_CreatesThreeSegments()
        {
            var submarine = new Submarine(player, ShipType.Submarine, new WeaponStub(player, 0, WeaponType.SingleShot));

            Assert.AreEqual(3, submarine.Cells.Count());
        }

        [Test]
        public void GivenDestroyer_WhenConstructing_CreatesTwoSegments()
        {
            var destroyer = new Destroyer(player, ShipType.Destroyer, new WeaponStub(player, 0, WeaponType.SingleShot));

            Assert.AreEqual(2, destroyer.Cells.Count());
        }

        [Test]
        public void GivenShipWithOneSegment_WhenDamagingSegment_MarksCellsAsDestroyed()
        {
            var ship = new ShipStub(player, 1, new WeaponStub(player, 1, WeaponType.SingleShot));
            const int width = 5;
            const int height = 5;
            var map = new PlayerMap(width, height, this.player);
            var coordinate = new Point(0, 0);
            ship.Place(coordinate, Direction.East, map);
            var segmentToDamage = ship.Cells.First();

            segmentToDamage.LandShot();

            Assert.True(ship.Cells.All(x => x != null && x.Hit));
        }

        [Test]
        public void GivenShipWithAllSegmentsUnDamaged_WhenDamagingSegment_DoesNotMarkShipAsDestroyed()
        {
            var ship = new ShipStub(player, 2, new WeaponStub(player, 1, WeaponType.SingleShot));
            const int width = 5;
            const int height = 5;
            var map = new PlayerMap(width, height, this.player);
            var coordinate = new Point(0, 0);
            ship.Place(coordinate, Direction.East, map);
            var segmentToDamage = ship.Cells.First();

            segmentToDamage.LandShot();

            Assert.False(ship.Destroyed);
        }

        [Test]
        public void GivenShipWithAllButOneSegmentDamaged_WhenDamagingSegment_MarksCellsAsDestroyed()
        {
            var ship = new ShipStub(player, 4, new WeaponStub(player, 1, WeaponType.SingleShot));
            const int width = 5;
            const int height = 5;
            var map = new PlayerMap(width, height, this.player);
            var coordinate = new Point(0, 0);
            ship.Place(coordinate, Direction.East, map);
            var segmentToDamage = ship.Cells.First();
            foreach(var segments in ship.Cells.Where(x => x != segmentToDamage))
            {
                segments.LandShot();
            }

            segmentToDamage.LandShot();

            Assert.True(ship.Cells.All(x => x != null && x.Hit));
        }

        [Test]
        public void GivenShipWithAllButOneSegmentDamaged_WhenDamagingSegment_PlayerCanStillShoot()
        {
            const int width = 5;
            const int height = 5;
            var map = new GameMap("SomePlayer", "SomeOtherPlayer", width, height);
            map.Place(PlayerType.One, ShipType.Cruiser, new Point(0, 0), Direction.East);
            map.Place(PlayerType.One, ShipType.Battleship, new Point(0, 1), Direction.East);
            map.Place(PlayerType.One, ShipType.Carrier, new Point(0, 2), Direction.East);
            map.Place(PlayerType.One, ShipType.Destroyer, new Point(0, 3), Direction.East);
            map.Place(PlayerType.One, ShipType.Submarine, new Point(0, 4), Direction.East);
            map.Place(PlayerType.Two, ShipType.Cruiser, new Point(0, 0), Direction.East);
            map.Place(PlayerType.Two, ShipType.Battleship, new Point(0, 1), Direction.East);
            map.Place(PlayerType.Two, ShipType.Carrier, new Point(0, 2), Direction.East);
            map.Place(PlayerType.Two, ShipType.Destroyer, new Point(0, 3), Direction.East);
            map.Place(PlayerType.Two, ShipType.Submarine, new Point(0, 4), Direction.East);

            foreach (var cell in map.GetBattleshipPlayer(PlayerType.One).Ships.SelectMany(x => x.Cells))
            {
                cell.LandShot();
            }

            Assert.True(map.GetBattleshipPlayer(PlayerType.One).Ships.SelectMany(x => x.Cells).All(x => x != null && x.Hit));
            
            Assert.DoesNotThrow(() => map.Shoot(PlayerType.One, new List<Point> { new Point(0,0)}, WeaponType.SingleShot));
        }
    }
}
