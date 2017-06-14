using Domain.Games;
using Domain.Players;
using Domain.Weapons;

namespace Domain.Ships
{
    public class Submarine : Ship
    {
        public Submarine(BattleshipPlayer player, ShipType shipType, Weapon weapon) : base(player, 3, shipType, weapon)
        {
        }
    }
}