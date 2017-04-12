using Domain.Games;
using Domain.Players;

namespace Domain.Ships
{
    public class Destroyer : Ship
    {
        public Destroyer(BattleshipPlayer owner, ShipType shipType) : base(owner, 2, shipType) {}
    }
}
