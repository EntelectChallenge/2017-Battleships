using System;
using Domain.Ships;

namespace Domain.Games
{
    internal static class ShipTypeExtension
    {
        internal static Type EnumToType(this ShipType source)
        {
            switch (source)
            {
                case ShipType.Battleship:
                    return typeof(Battleship);
                case ShipType.Carrier:
                    return typeof(Carrier);
                case ShipType.Cruiser:
                    return typeof(Cruiser);
                case ShipType.Destroyer:
                    return typeof(Destroyer);
                case ShipType.Submarine:
                    return typeof(Submarine);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        internal static string ToFriendlyName(this ShipType shipType)
        {
            switch (shipType)
            {
                case ShipType.Battleship:
                    return "Battle Ship";
                case ShipType.Carrier:
                    return "Carrier";
                case ShipType.Cruiser:
                    return "Cruiser";
                case ShipType.Destroyer:
                    return "Destroyer";
                case ShipType.Submarine:
                    return "Submarine";
                case ShipType.Stub:
                    return "Stub";
                default:
                    return "There is no such ship type";
            }
        }
    }
}