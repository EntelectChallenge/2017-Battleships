using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Domain.Games;
using Domain.Players;
using Domain.Weapons;
using GameEngine.Exceptions;

namespace GameEngine.Commands.PlayerCommands
{
    public class FireCornerrShotCommand : ICommand
    {
        private readonly Point _centerPoint;

        public FireCornerrShotCommand(Point centerPoint)
        {
            _centerPoint = centerPoint;
        }

        public void PerformCommand(GameMap gameMap, BattleshipPlayer player)
        {
            try
            {
                if (player.Ships.Any(x => x.ShipType == ShipType.Carrier && !x.Destroyed))
                {
                    var opponentsMap = gameMap.GetOpponetMap(player.PlayerType);
                    var cornerShot =
                        opponentsMap.Cells.Where(cell => (cell.X + 1 == _centerPoint.X && cell.Y - 1 == _centerPoint.Y)
                                                         || (cell.X - 1 == _centerPoint.X && cell.Y - 1 == _centerPoint.Y)
                                                         || (cell.X + 1 == _centerPoint.X && cell.Y + 1 == _centerPoint.Y)
                                                         || (cell.X - 1 == _centerPoint.X && cell.Y + 1 == _centerPoint.Y)).Select(x => new Point(x.X, x.Y)).ToList();
                    gameMap.Shoot(player.PlayerType, cornerShot, WeaponType.CornerShot);
                }
                else
                {
                    throw new DestroyedShipException(
                        $"{player.Name}'s Carrier has been destroyed and cannot use this shot");
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
