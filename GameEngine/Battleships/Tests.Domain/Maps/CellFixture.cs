using System;
using System.Collections.Generic;
using System.Drawing;
using Domain.Maps;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Domain.Maps
{
    [TestFixture]
    public class CellFixture
    {
        [Test]
        [TestCaseSource(typeof(Direction), nameof(Direction.All))]
        public void GivenCellWithNoNeighbour_WhenConstructing_DoesNotAddAnyNeighbours(Direction direction)
        {
            var neighbours = new Dictionary<Point, Cell>();
            var point = new Point(0, 0);
            var cell = new Cell(point, neighbours);

            Assert.Throws<InvalidOperationException>(() => cell.Neighbour(direction));
        }

        [Test]
        [TestCaseSource(typeof(Direction), nameof(Direction.All))]
        public void GivenCellWithSingleNeighbour_WhenConstructing_AddsCellToNeighbour(Direction direction)
        {
            var neighbours = new Dictionary<Point, Cell>();
            var point = new Point(0, 0);
            var neighbourPoint = point + direction;
            var neighbourCell = new Cell(neighbourPoint, neighbours);
            neighbours.Add(neighbourPoint, neighbourCell);

            var cell = new Cell(point, neighbours);

            Assert.AreEqual(cell, neighbourCell.Neighbour(direction.Opposite));
        }

        [Test]
        [TestCaseSource(typeof(Direction), nameof(Direction.All))]
        public void GivenCellWithSingleNeighbour_WhenConstructing_AddsNeighbour(Direction direction)
        {
            var neighbours = new Dictionary<Point, Cell>();
            var point = new Point(0, 0);
            var neighbourPoint = point + direction;
            var neighbourCell = new Cell(neighbourPoint, neighbours);
            neighbours.Add(neighbourPoint, neighbourCell);

            var cell = new Cell(point, neighbours);

            Assert.AreEqual(neighbourCell, cell.Neighbour(direction));
        }
    }
}
