using System;
using Domain.Games;
using Domain.Players;
using System.Drawing;
using Domain.Maps;
using System.Collections.Generic;
using System.Linq;
using Domain.Weapons;
using GameEngine.Exceptions;

namespace GameEngine.Commands.PlayerCommands
{
    public class FireDoubleShotCommand : ICommand
    {
        private readonly Point _centerPoint;
        private readonly Direction _direction;

        public FireDoubleShotCommand(Point centerPoint, Direction direction)
        {
            this._centerPoint = centerPoint;
            this._direction = direction;
        }
        public void PerformCommand(GameMap gameMap, BattleshipPlayer player)
        {
            try
            {
                if (player.Ships.Any(x => x.ShipType == ShipType.Destroyer && !x.Destroyed))
                {
                    var doubleShot = new List<Point>();
                    var opponentMap = gameMap.GetOpponetMap(player.PlayerType);

                    if (_direction == Direction.North || _direction == Direction.South)
                    {
                        doubleShot = opponentMap.Cells
                            .Where(cell => (cell.X == _centerPoint.X && cell.Y + 1 == _centerPoint.Y) ||
                                           (cell.X == _centerPoint.X && cell.Y - 1 == _centerPoint.Y))
                            .Select(x => new Point(x.X, x.Y)).ToList();
                    }
                    else if (_direction == Direction.East || _direction == Direction.West)
                    {
                        doubleShot = opponentMap.Cells
                            .Where(cell => (cell.X + 1 == _centerPoint.X && cell.Y == _centerPoint.Y) ||
                                           (cell.X - 1 == _centerPoint.X && cell.Y == _centerPoint.Y))
                            .Select(x => new Point(x.X, x.Y)).ToList();
                    }

                    gameMap.Shoot(player.PlayerType, doubleShot, WeaponType.DoubleShot);
                }
                else
                {
                    throw new DestroyedShipException(
                        $"{player.Name}'s Battleship has been destroyed and cannot use this shot");
                }
            }
            catch (Exception exception)
            {
                throw new InvalidCommandException(exception.Message, exception);
            }

        }
    }
}
