using Domain.Games;
using Domain.Players;

namespace Domain.Ships
{
    public class Carrier : Ship
    {
        public Carrier(BattleshipPlayer owner, ShipType shipType) : base(owner, 5, shipType) {}
    }
}
