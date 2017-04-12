using System.Collections.Generic;
using System.Drawing;
using Domain.Games;
using Domain.Maps;
using Domain.Players;
using Domain.Ships;

namespace Tests.Domain.Ships.Stubs
{
    public class ShipStub : Ship
    {
        public static Ship FullStub(BattleshipPlayer owner)
        {
           return new ShipStub(owner, 2);
        }

        public ShipStub(BattleshipPlayer owner, int segmentCount) : base(owner, segmentCount, ShipType.Stub)
        {
        }
    }
}
