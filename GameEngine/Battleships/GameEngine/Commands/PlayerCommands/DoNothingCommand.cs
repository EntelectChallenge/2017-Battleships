using System.Drawing;
using Domain.Games;
using Domain.Maps;
using Domain.Players;
using Domain.Ships;

namespace GameEngine.Commands.PlayerCommands
{
    public class DoNothingCommand : ICommand
    {
        public void PerformCommand(GameMap gameMap, BattleshipPlayer player)
        {

        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}