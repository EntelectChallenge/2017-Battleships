using System.Drawing;
using Domain.Maps;
using NUnit.Framework;

namespace Tests.Domain.Maps
{
    [TestFixture]
    public class DirectionFixture
    {
        private static readonly object[] oppositeDirections =
        {
            new object[] { Direction.North, Direction.South },
            new object[] { Direction.NorthEast, Direction.SouthWest },
            new object[] { Direction.East, Direction.West },
            new object[] { Direction.SouthEast, Direction.NorthWest },
            new object[] { Direction.South, Direction.North },
            new object[] { Direction.SouthWest, Direction.NorthEast },
            new object[] { Direction.West, Direction.East },
            new object[] { Direction.NorthWest, Direction.SouthEast }
        };

        private static readonly object[] directionSizes =
        {
            new object[] { Direction.North, new Size(0, 1) },
            new object[] { Direction.NorthEast, new Size(1, 1) },
            new object[] { Direction.East, new Size(1, 0) },
            new object[] { Direction.SouthEast, new Size(1, -1) },
            new object[] { Direction.South, new Size(0, -1) },
            new object[] { Direction.SouthWest, new Size(-1, -1) },
            new object[] { Direction.West, new Size(-1, 0) },
            new object[] { Direction.NorthWest, new Size(-1, 1) }
        };

        [Test]
        [TestCaseSource(nameof(directionSizes))]
        public void GivenDirection_CastingToSize_CastsToCorrectSize(Direction direction, Size expected)
        {
            var size = (Size)direction;

            Assert.AreEqual(expected, size);
        }

        [Test]
        [TestCaseSource(nameof(oppositeDirections))]
        public void GivenDirection_FetchingOpposite_FetchesCorrectOpposite(Direction original, Direction expected)
        {
            var opposite = original.Opposite;

            Assert.AreEqual(expected, opposite);
        }
    }
}
