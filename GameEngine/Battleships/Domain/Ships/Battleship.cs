using Domain.Games;
using Domain.Players;
using Domain.Weapons;

namespace Domain.Ships
{
    public class Battleship : Ship
    {
        public Battleship(BattleshipPlayer owner, ShipType shipType, Weapon weapon) : base(owner, 4, shipType, weapon) {}
    }
}
