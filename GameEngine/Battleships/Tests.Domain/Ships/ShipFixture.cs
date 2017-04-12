using System;
using System.Drawing;
using System.Linq;
using Domain.Games;
using Domain.Maps;
using Domain.Players;
using Domain.Ships;
using NUnit.Framework;
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
            this.player = new BattleshipPlayer("SomePlayer", 'A', PlayerType.One);
        }

    [Test]
        public void GivenShip_WhenConstructing_AddsCorrectAmountOfShipSegments()
        {
            const int segmentCount = 4;

            Ship ship = new ShipStub(player, segmentCount);

            Assert.AreEqual(segmentCount, ship.Cells.Count());
        }

        [Test]
        public void GivenShip_WhenConstructing_AssociatesThePlayerToShip()
        {
            const int segmentCount = 4;

            Ship ship = new ShipStub(player, segmentCount);

            Assert.AreEqual(player, ship.Owner);
        }

        [Test]
        public void GivenShip_WhenConstruting_SetsDestroyedToFalse()
        {
            const int segmentCount = 4;

            Ship ship = new ShipStub(player, segmentCount);
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
            Assert.Throws<ArgumentException>(() => new ShipStub(player, segmentCount));
        }

        [Test]
        public void GivenCarrier_WhenConstructing_CreatesFiveSegements()
        {
            var carrier = new Carrier(player, ShipType.Carrier);

            Assert.AreEqual(5, carrier.Cells.Count());
        }

        [Test]
        public void GivenBattleship_WhenConstructing_CreatesFourSegments()
        {
            var battleship = new Battleship(player, ShipType.Battleship);

            Assert.AreEqual(4, battleship.Cells.Count());
        }

        [Test]
        public void GivenCruiser_WhenConstructing_CreatesThreeSegments()
        {
            var cruiser = new Cruiser(player, ShipType.Cruiser);

            Assert.AreEqual(3, cruiser.Cells.Count());
        }

        [Test]
        public void GivenSubmarine_WhenConstructing_CreatesThreeSegments()
        {
            var submarine = new Submarine(player, ShipType.Submarine);

            Assert.AreEqual(3, submarine.Cells.Count());
        }

        [Test]
        public void GivenDestroyer_WhenConstructing_CreatesTwoSegments()
        {
            var destroyer = new Destroyer(player, ShipType.Destroyer);

            Assert.AreEqual(2, destroyer.Cells.Count());
        }

        [Test]
        public void GivenShipWithOneSegment_WhenDamagingSegment_MarksShipAsDestroyed()
        {
            var ship = new ShipStub(player, 1);
            const int width = 5;
            const int height = 5;
            var map = new PlayerMap(width, height, this.player);
            var coordinate = new Point(0, 0);
            ship.Place(coordinate, Direction.East, map);
            var segmentToDamage = ship.Cells.First();

            segmentToDamage.LandShot();

            Assert.True(ship.Destroyed);
        }

        [Test]
        public void GivenShipWithAllSegmentsUnDamaged_WhenDamagingSegment_DoesNotMarkShipAsDestroyed()
        {
            var ship = new ShipStub(player, 2);
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
        public void GivenShipWithAllButOneSegmentDamaged_WhenDamagingSegment_MarksShipAsDestroyed()
        {
            var ship = new ShipStub(player, 4);
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

            Assert.True(ship.Destroyed);
        }
    }
}
