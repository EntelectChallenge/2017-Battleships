using System;
using System.Drawing;
using System.Linq;
using Domain.Games;
using Domain.Players;
using Domain.Ships;
using Domain.Weapons;
using GameEngine.Exceptions;

namespace GameEngine.Commands.PlayerCommands
{
    public class FireCrossShotCommand : ICommand
    {
        private readonly Point _centerPoint;

        public FireCrossShotCommand(Point centerPoint)
        {
            _centerPoint = centerPoint;
        }

        public void PerformCommand(GameMap gameMap, BattleshipPlayer player)
        {
            try
            {
                if (player.Ships.Any(x => x.ShipType == ShipType.Battleship && !x.Destroyed))
                {
                    var crossShot = gameMap.GetOpponetMap(player.PlayerType).Cells
                        .Where(cell =>
                            (cell.X + 1 == _centerPoint.X && cell.Y - 1 == _centerPoint.Y)
                            || (cell.X == _centerPoint.X && cell.Y == _centerPoint.Y)
                            || (cell.X - 1 == _centerPoint.X && cell.Y - 1 == _centerPoint.Y)
                            || (cell.X + 1 == _centerPoint.X && cell.Y - 1 == _centerPoint.Y)
                            || (cell.X - 1 == _centerPoint.X && cell.Y + 1 == _centerPoint.Y)).Select(x => new Point(x.X, x.Y)).ToList();

                    gameMap.Shoot(player.PlayerType, crossShot, WeaponType.CrossShot);

                }
                else
                {
                    throw new DestroyedShipException($"{player.Name}'s Battleship has been destroyed and cannot use this shot");
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