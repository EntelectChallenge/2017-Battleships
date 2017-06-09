using System;
using System.Collections.Generic;
using System.Drawing;
using Domain.Games;
using Domain.Maps;
using Domain.Players;
using Domain.Ships;
using Domain.Weapons;
using GameEngine.Exceptions;
using GameEngine.Properties;

namespace GameEngine.Commands.PlayerCommands
{
    public class FireSingleShotCommand : ICommand
    {
        private readonly Point _point;

        public FireSingleShotCommand(Point point)
        {
            this._point = point;
        }

        public void PerformCommand(GameMap gameMap, BattleshipPlayer player)
        {
            try
            {
                gameMap.Shoot(player.PlayerType, new List<Point>() {_point}, WeaponType.SingleShot);
            }
            catch (Exception exception)
            {
                throw new InvalidCommandException(exception.Message, exception);
            }
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}