using System.Drawing;
using Domain.Games;
using Domain.Players;

namespace GameEngine.Commands.PlayerCommands
{
    public class FireSeekerMissleCommand : ICommand
    {

        private readonly Point _centerPoint;

        public FireSeekerMissleCommand(Point centerPoint)
        {
            _centerPoint = centerPoint;
        }

        public override string ToString()
        {
            return GetType().Name;
        }

        public void PerformCommand(GameMap gameMap, BattleshipPlayer player)
        {

        }
    }
}