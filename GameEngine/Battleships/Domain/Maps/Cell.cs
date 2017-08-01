using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Domain.Games;
using Domain.Ships;
using Domain.Weapons;
using Newtonsoft.Json;

namespace Domain.Maps
{
    public class Cell : IWeaponTarget
    {
        [JsonIgnore] private readonly IDictionary<Direction, Cell> neighbours;

        [JsonProperty]
        public bool Occupied => OccupiedBy != null;

        [JsonIgnore]
        public Ship OccupiedBy { get; set; }

        [JsonProperty]
        public bool Hit { get; set; }

        [JsonIgnore]
        public Point Point { get; set; }

        [JsonProperty]
        public int X => Point.X;

        [JsonProperty]
        public int Y => Point.Y;

        [JsonIgnore]
        public bool Damaged => Occupied && Hit;

        [JsonIgnore]
        public bool Missed => !Occupied && Hit;

        [JsonProperty]
        public bool Shielded { get; set; }

        [JsonProperty]
        public bool ShieldHit { get; set; }

        private Cell()
        {
            this.neighbours = new Dictionary<Direction, Cell>();
            this.Hit = false;
            this.Shielded = false;
            this.ShieldHit = false;
        }

        internal Cell(Point cellCoordinate, IDictionary<Point, Cell> otherCells)
            : this()
        {
            foreach (var direction in Direction.All)
            {
                var neighbourCoordinate = cellCoordinate + direction;

                if (!otherCells.ContainsKey(neighbourCoordinate))
                {
                    continue;
                }

                var otherCell = otherCells[neighbourCoordinate];

                this.neighbours.Add(direction, otherCell);
                otherCell.neighbours.Add(direction.Opposite, this);
            }
            Point = cellCoordinate;
        }


        internal Cell Neighbour(Direction direction)
        {
            if (!this.neighbours.ContainsKey(direction))
            {
                throw new InvalidOperationException($"This cell does not have a neighbour in the {direction} direction");
            }

            return this.neighbours[direction];
        }


        internal bool HasNeighbour(Direction direction)
        {
            return this.neighbours.ContainsKey(direction);
        }

        internal void Place(Direction direction, Ship ship)
        {
            if (this.OccupiedBy != null)
            {
                throw new InvalidOperationException(
                    "Can't place an occupant on this cell as there is already one present.");
            }

            this.OccupiedBy = ship;
        }


        public bool CanPlace()
        {
            return OccupiedBy == null;
        }

        public bool LandShot()
        {
            if (!Shielded)
            {
                var firstShot = !Hit;
                Hit = true;
                var shotLanded = Occupied && firstShot;
                return shotLanded;
            }
            ShieldHit = true;
            return false;
        }

        public void ApplyShield()
        {
            this.Shielded = true;
        }

        public void RemoveShield()
        {
            this.Shielded = false;
        }

        IWeaponTarget IWeaponTarget.Neighbour(Direction direction)
        {
            return this.Neighbour(direction);
        }
    }
}