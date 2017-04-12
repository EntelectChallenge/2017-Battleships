using System;
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
    public class FireShotCommand : ICommand
    {
        private readonly Point _point;

        public FireShotCommand(Point point)
        {
            this._point = point;
        }
        public void PerformCommand(GameMap gameMap, BattleshipPlayer player)
        {
            try
            {
                var shotLanded = gameMap.Shoot(player.PlayerType, _point, WeaponType.SingleShot);
                player.ShotsFired++;
                if (shotLanded)
                {
                    player.ShotsHit++;
                    player.AddPoints(Settings.Default.PointsHit);
                    if (player.FirstShotLanded == int.MaxValue)
                    {
                        player.FirstShotLanded = gameMap.CurrentRound;
                    }
                }
                var destroyed = gameMap.WasShipDestroyed(player.PlayerType, _point);
                if (destroyed)
                {
                    player.AddPoints(Settings.Default.PointsShipSunk);
                }
            }
            catch (Exception exception)
            {
                throw new InvalidCommandException(exception.Message);
            }
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}