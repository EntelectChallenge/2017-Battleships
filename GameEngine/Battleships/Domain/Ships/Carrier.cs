using Domain.Games;
using Domain.Players;
using Domain.Weapons;

namespace Domain.Ships
{
    public class Carrier : Ship
    {
        public Carrier(BattleshipPlayer owner, ShipType shipType, Weapon weapon) : base(owner, 5, shipType, weapon) {}
    }
}
