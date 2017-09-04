using System;
using System.Drawing;
using Domain.Games;
using Domain.Players;
using GameEngine.Exceptions;

namespace GameEngine.Commands.PlayerCommands
{
    public class PlaceShieldCommand : ICommand
    {
        public readonly Point Point;

        public PlaceShieldCommand(Point point)
        {
            this.Point = point;
        }
        public void PerformCommand(GameMap gameMap, BattleshipPlayer player)
        {
            try
            {
                gameMap.PlaceShield(player, Point, gameMap.CurrentRound);
            }
            catch (Exception exception)
            {
                throw new InvalidCommandException(exception.Message, exception);
            }
        }
    }
}