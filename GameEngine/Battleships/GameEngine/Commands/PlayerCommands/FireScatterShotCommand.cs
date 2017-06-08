using System.Drawing;
using Domain.Games;
using Domain.Players;

namespace GameEngine.Commands.PlayerCommands
{
    public class FireScatterShotCommand : ICommand
    {
        private readonly Point _centerPoint;

        public FireScatterShotCommand(Point centerPoint)
        {
            _centerPoint = centerPoint;
        }

        public override string ToString()
        {
            return GetType().Name;
        }

        public void PerformCommand(GameMap gameMap, BattleshipPlayer player)
        {
            throw new System.NotImplementedException();
        }
    }
}