using Domain.Games;
using Domain.Players;

namespace Domain.Ships
{
    public class Cruiser : Ship
    {
        public Cruiser(BattleshipPlayer owner, ShipType shipType) : base(owner, 3, shipType) {}
    }
}
