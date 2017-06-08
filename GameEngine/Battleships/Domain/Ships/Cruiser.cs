using Domain.Games;
using Domain.Players;
using Domain.Weapons;

namespace Domain.Ships
{
    public class Cruiser : Ship
    {
        public Cruiser(BattleshipPlayer owner, ShipType shipType, Weapon weapon) : base(owner, 3, shipType, weapon) {}
    }
}
