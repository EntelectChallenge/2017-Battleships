using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Domain.Games;
using Domain.Players;
using Domain.Properties;
using Domain.Ships;
using Domain.Weapons;
using Newtonsoft.Json;

namespace Domain.Maps
{
    public class PlayerMap
    {
        [JsonIgnore] private readonly IDictionary<Point, Cell> cells;

        [JsonProperty]
        public IEnumerable<Cell> Cells => this.cells.Values;

        [JsonProperty]
        public BattleshipPlayer Owner { get; }

        [JsonProperty]
        public int MapWidth { get; }

        [JsonProperty]
        public int MapHeight { get; }

        public bool IsReady()
        {
            return Owner.AllShippsPlaced();
        }

        private PlayerMap()
        {
            this.cells = new Dictionary<Point, Cell>();
        }

        public PlayerMap(int width, int height, BattleshipPlayer owner)
            : this()
        {
            if (width <= 0)
            {
                throw new ArgumentException("Argument cannot be zero or negative", nameof(width));
            }

            if (height <= 0)
            {
                throw new ArgumentException("Argument cannot be zero or negative", nameof(width));
            }

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var coordinate = new Point(x, y);
                    var cell = new Cell(coordinate, this.cells);

                    this.cells.Add(coordinate, cell);
                }
            }

            this.MapHeight = height;
            this.MapWidth = width;
            this.Owner = owner;
        }

        public bool CanPlace(Ship ship, Point point, Direction direction)
        {
            if (ship.Owner != this.Owner)
            {
                throw new InvalidOperationException(
                    $"Can't place a ship belonging to {ship.Owner.Name} on the map beloning to {this.Owner.Name}.");
            }

            if (ship.Placed)
            {
                throw new InvalidOperationException($"The {ship.GetType().Name} has already been placed");
            }

            if (direction == Direction.NorthEast ||
                direction == Direction.SouthEast ||
                direction == Direction.SouthWest ||
                direction == Direction.NorthWest)
            {
                throw new InvalidOperationException($"Can't place a ship going in the {direction} direction.");
            }

            return ship.CanPlace(point, direction, this);
        }

        public void Place(Ship ship, Point point, Direction direction)
        {
            if (ship.Owner != this.Owner)
            {
                throw new InvalidOperationException(
                    $"Can't place a ship belonging to {ship.Owner.Name} on the map beloning to {this.Owner.Name}.");
            }

            if (ship.Placed)
            {
                throw new InvalidOperationException($"The {ship.GetType().Name} has already been placed");
            }

            if (direction == Direction.NorthEast ||
                direction == Direction.SouthEast ||
                direction == Direction.SouthWest ||
                direction == Direction.NorthWest)
            {
                throw new InvalidOperationException($"Can't place a ship going in the {direction} direction.");
            }

            ship.Place(point, direction, this);
        }

        internal void Shoot(Weapon weapon, List<Point> areaOfEffect, int currentRound)
        {
            if (this.Owner == weapon.Owner)
            {
                throw new InvalidOperationException($"Can't shoot at your own map.");
            }

            foreach (var coordinate in areaOfEffect)
            {
                if (!this.cells.ContainsKey(coordinate))
                {
                    throw new ArgumentException("Coordinates are out of bounds.", nameof(coordinate));
                }
            }

            var targets = this.Cells.Where(x => areaOfEffect.Any(y => y.X == x.X && y.Y == x.Y)).ToList();

            weapon.Shoot(targets, currentRound);
        }

        public Ship GetShipAtPoint(Point point)
        {
            var cell = this.cells[point];
            var ship = cell.OccupiedBy;
            return ship;
        }

        public void ClearMap()
        {
            foreach (var keyValueCell in cells)
            {
                keyValueCell.Value.OccupiedBy = null;
                keyValueCell.Value.Hit = false;
            }
        }

        public Cell GetCellAtPoint(Point point)
        {
            return cells[point];
        }

        public void RemoveShield()
        {
            foreach (var keyValueCell in cells)
            {
                keyValueCell.Value.Shielded = false;
                keyValueCell.Value.ShieldHit = false;
            }
        }

        public void PlaceShield(Point centerPoint, int currentRound)
        {
            var shieldSize = Owner.Shield.CurrentRadius;

            var startX = Math.Max(centerPoint.X - shieldSize, 0);
            var endX = Math.Min(centerPoint.X + shieldSize, MapWidth - 1);

            var startY = Math.Max(centerPoint.Y - shieldSize, 0);
            var endY = Math.Min(centerPoint.Y + shieldSize, MapHeight - 1);

            for (var x = startX; x <= endX; x++)
            {
                for (var y = startY; y <= endY; y++)
                {
                    var point = new Point(x, y);
                    cells[point].ApplyShield();
                }
            }

            Owner.Shield.RoundLastUsed = currentRound;
            Owner.Shield.Active = true;
            Owner.Shield.CenterPoint = centerPoint;
        }
    }
}