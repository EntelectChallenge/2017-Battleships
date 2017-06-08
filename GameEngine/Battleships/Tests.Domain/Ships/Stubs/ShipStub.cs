using System.Collections.Generic;
using System.Drawing;
using Domain.Games;
using Domain.Maps;
using Domain.Players;
using Domain.Ships;
using Domain.Weapons;

namespace Tests.Domain.Ships.Stubs
{
    public class ShipStub : Ship
    {
        public static Ship FullStub(BattleshipPlayer owner, Weapon weapon)
        {
           return new ShipStub(owner, 2, weapon);
        }

        public ShipStub(BattleshipPlayer owner, int segmentCount, Weapon weapon) : base(owner, segmentCount, ShipType.Stub, weapon)
        {
        }
    }
}
