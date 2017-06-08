using System.Drawing;
using Domain.Games;
using Domain.Players;

namespace GameEngine.Commands.PlayerCommands
{
    public class FireCornerShotCommand : ICommand
    {
        private readonly Point _centerPoint;

        public FireCornerShotCommand(Point centerPoint)
        {
            _centerPoint = centerPoint;
        }

        public void PerformCommand(GameMap gameMap, BattleshipPlayer player)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}