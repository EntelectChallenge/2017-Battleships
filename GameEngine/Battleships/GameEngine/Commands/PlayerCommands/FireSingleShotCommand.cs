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
                var alreadyDestroyed = gameMap.WasShipDestroyed(player.PlayerType, _point);
                gameMap.Shoot(player.PlayerType, new List<Point>() {_point}, WeaponType.SingleShot);
                player.ShotsFired++;

                var destroyed = gameMap.WasShipDestroyed(player.PlayerType, _point);
                if (!alreadyDestroyed && destroyed)
                {
                    player.AddPoints(Settings.Default.PointsShipSunk);
                }
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