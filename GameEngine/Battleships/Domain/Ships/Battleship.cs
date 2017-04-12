using Domain.Games;
using Domain.Players;

namespace Domain.Ships
{
    public class Battleship : Ship
    {
        public Battleship(BattleshipPlayer owner, ShipType shipType) : base(owner, 4, shipType) {}
    }
}
