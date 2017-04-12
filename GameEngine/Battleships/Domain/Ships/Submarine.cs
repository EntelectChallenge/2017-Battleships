using Domain.Games;
using Domain.Players;

namespace Domain.Ships
{
    public class Submarine : Ship
    {
        public Submarine(BattleshipPlayer player, ShipType shipType) : base(player, 3, shipType)
        {
        }
    }
}