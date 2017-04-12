using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Domain.Maps
{
    public class Direction
    {
        private readonly DirectionEnum direction;

        private static IReadOnlyList<DirectionEnum> EnumValues { get; }

        private static int DirectionEnumCount { get; }

        public static Direction North { get; }
        public static Direction NorthEast { get; }
        public static Direction East { get; }
        public static Direction SouthEast { get; }
        public static Direction South { get; }
        public static Direction SouthWest { get; }
        public static Direction West { get; }
        public static Direction NorthWest { get; }

        public static IReadOnlyList<Direction> All { get; }

        public Direction Opposite
        {
            get
            {
                var current = (int)direction;

                var opposite = current - DirectionEnumCount / 2;

                if(opposite < 0)
                {
                    return new Direction((DirectionEnum)(opposite + DirectionEnumCount));
                }

                return new Direction((DirectionEnum)opposite);
            }
        }

        static Direction()
        {
            EnumValues = Enum.GetValues(typeof(DirectionEnum)).Cast<DirectionEnum>().ToList();
            DirectionEnumCount = EnumValues.Count;

            All = EnumValues.Select(x => new Direction(x)).ToList();
            North = All.First(x => x.direction == DirectionEnum.North);
            NorthEast = All.First(x => x.direction == DirectionEnum.NorthEast);
            East = All.First(x => x.direction == DirectionEnum.East);
            SouthEast = All.First(x => x.direction == DirectionEnum.SouthEast);
            South = All.First(x => x.direction == DirectionEnum.South);
            SouthWest = All.First(x => x.direction == DirectionEnum.SouthWest);
            West = All.First(x => x.direction == DirectionEnum.West);
            NorthWest = All.First(x => x.direction == DirectionEnum.NorthWest);
        }

        private Direction(DirectionEnum direction)
        {
            this.direction = direction;
        }

        public override bool Equals(object obj)
        {
            return obj is Direction && this == (Direction)obj;
        }

        public override int GetHashCode()
        {
            return direction.GetHashCode();
        }

        public override string ToString()
        {
            return direction.ToString();
        }

        public static bool operator ==(Direction left, Direction right)
        {
            return left?.direction == right?.direction;
        }

        public static bool operator !=(Direction left, Direction right)
        {
            return !(left == right);
        }

        //This implicit cast enables the use of + and - operators in conjunction with a Point in order to calculate the neighbouring coordinate.
        public static implicit operator Size(Direction value)
        {
            switch(value.direction)
            {
                case DirectionEnum.North:
                    return new Size(0, 1);
                case DirectionEnum.NorthEast:
                    return new Size(1, 1);
                case DirectionEnum.East:
                    return new Size(1, 0);
                case DirectionEnum.SouthEast:
                    return new Size(1, -1);
                case DirectionEnum.South:
                    return new Size(0, -1);
                case DirectionEnum.SouthWest:
                    return new Size(-1, -1);
                case DirectionEnum.West:
                    return new Size(-1, 0);
                case DirectionEnum.NorthWest:
                    return new Size(-1, 1);
                default:
                    throw new ArgumentException("Value is out of bounds!", nameof(value));
            }
        }

        #region Nested type: DirectionEnum

        public enum DirectionEnum
        {
            North = 0,
            NorthEast = 1,
            East = 2,
            SouthEast = 3,
            South = 4,
            SouthWest = 5,
            West = 6,
            NorthWest = 7
        }

        #endregion
    }
}
