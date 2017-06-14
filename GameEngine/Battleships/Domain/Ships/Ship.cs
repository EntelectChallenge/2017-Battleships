using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Domain.Games;
using Domain.Maps;
using Domain.Players;
using Domain.Weapons;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Domain.Ships
{
    public abstract class Ship
    {
        [JsonIgnore] protected readonly Cell[] _cells;

        [JsonProperty]
        public IEnumerable<Cell> Cells => this._cells;

        public bool Destroyed { get; set; }

        public bool Placed { get; set; }

        [JsonIgnore]
        public BattleshipPlayer Owner { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ShipType ShipType { get; }

        [JsonProperty]
        public List<Weapon> Weapons { get; set; }

        private Ship()
        {
            this._cells = new Cell[0];
            this.Placed = false;
        }

        protected Ship(BattleshipPlayer owner, int segmentCount, ShipType shipType, Weapon weapon) : this()
        {
            this.ShipType = shipType;
            this.Owner = owner;
            if (segmentCount <= 0)
            {
                throw new ArgumentException("Ship can't have zero or negative cells", nameof(segmentCount));
            }
            this.Weapons = new List<Weapon> { new SingleShotWeapon(Owner, 1, WeaponType.SingleShot), weapon };
            this._cells = new Cell[segmentCount];
        }

        public void Place(Point point, Direction direction, PlayerMap playerMap)
        {
            var startCell = playerMap.GetCellAtPoint(point);

            var cells = new List<Cell>();
            try
            {
                var length = 0;
                var cell = startCell;
                while (length < _cells.Length)
                {
                    cells.Add(cell);
                    cell.Place(direction, this);
                    _cells[length] = cell;

                    if (!cell.HasNeighbour(direction))
                        break;

                    cell = cell.Neighbour(direction);
                    length++;
                }

                if (length < _cells.Length - 1)
                {
                    throw new InvalidOperationException(
                        $"There are not enough cells left in the {direction} direction to finish placing the {ShipType}");
                }

                Placed = true;
            }
            catch
            {
                cells.ForEach(x => x.OccupiedBy = null);
                throw;
            }
        }

        public bool CanPlace(Point point, Direction direction, PlayerMap playerMap)
        {
            var startCell = playerMap.GetCellAtPoint(point);

            var length = 0;
            var cell = startCell;
            while (length < _cells.Length)
            {
                if (!cell.CanPlace())
                    return false;

                length++;
                if (!cell.HasNeighbour(direction))
                    break;

                cell = cell.Neighbour(direction);
            }

            return length == _cells.Length;
        }

        public override string ToString()
        {
            return ShipType.ToFriendlyName();
        }
    }
}