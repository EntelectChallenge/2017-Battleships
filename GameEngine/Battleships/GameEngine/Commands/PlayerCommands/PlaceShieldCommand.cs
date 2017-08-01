using System;
using System.Drawing;
using Domain.Games;
using Domain.Players;
using GameEngine.Exceptions;

namespace GameEngine.Commands.PlayerCommands
{
    public class PlaceShieldCommand : ICommand
    {
        private readonly Point _point;

        public PlaceShieldCommand(Point point)
        {
            this._point = point;
        }
        public void PerformCommand(GameMap gameMap, BattleshipPlayer player)
        {
            try
            {
                gameMap.PlaceShield(player, _point, gameMap.CurrentRound);
            }
            catch (Exception exception)
            {
                throw new InvalidCommandException(exception.Message, exception);
            }
        }
    }
}