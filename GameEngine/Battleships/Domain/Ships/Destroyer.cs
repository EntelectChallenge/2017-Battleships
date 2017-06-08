using Domain.Games;
using Domain.Players;
using Domain.Weapons;

namespace Domain.Ships
{
    public class Destroyer : Ship
    {
        public Destroyer(BattleshipPlayer owner, ShipType shipType, Weapon weapon) : base(owner, 2, shipType, weapon) {}
    }
}
