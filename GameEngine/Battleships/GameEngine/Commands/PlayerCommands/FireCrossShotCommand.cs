using System;
using System.Collections.Generic;
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
        private readonly bool _diagonal;

        public FireCrossShotCommand(Point centerPoint, bool diagonal)
        {
            this._centerPoint = centerPoint;
            this._diagonal = diagonal;
        }

        public void PerformCommand(GameMap gameMap, BattleshipPlayer player)
        {
            try
            {
                var opponentsMap = gameMap.GetOpponetMap(player.PlayerType);
                var crossShot = new List<Point>();

                if (_diagonal)
                {
                    if (player.Ships.Any(x => x.ShipType == ShipType.Battleship && !x.Destroyed))
                    {
                        crossShot = opponentsMap.Cells
                            .Where(cell => (cell.X == _centerPoint.X && cell.Y == _centerPoint.Y)
                                           || (cell.X + 1 == _centerPoint.X && cell.Y - 1 == _centerPoint.Y)
                                           || (cell.X - 1 == _centerPoint.X && cell.Y - 1 == _centerPoint.Y)
                                           || (cell.X + 1 == _centerPoint.X && cell.Y + 1 == _centerPoint.Y)
                                           || (cell.X - 1 == _centerPoint.X && cell.Y + 1 == _centerPoint.Y))
                            .Select(x => new Point(x.X, x.Y)).ToList();

                        gameMap.Shoot(player.PlayerType, crossShot, WeaponType.DiagonalCrossShot);
                    }
                    else
                    {
                        throw new DestroyedShipException(
                            $"{player.Name}'s Battleship has been destroyed and cannot use this shot");
                    }
                }
                else
                {
                    if (player.Ships.Any(x => x.ShipType == ShipType.Cruiser && !x.Destroyed))
                    {
                        crossShot = opponentsMap.Cells
                            .Where(cell => (cell.X == _centerPoint.X && cell.Y == _centerPoint.Y)
                                           || (cell.X + 1 == _centerPoint.X && cell.Y == _centerPoint.Y)
                                           || (cell.X == _centerPoint.X && cell.Y - 1 == _centerPoint.Y)
                                           || (cell.X == _centerPoint.X && cell.Y + 1 == _centerPoint.Y)
                                           || (cell.X - 1 == _centerPoint.X && cell.Y == _centerPoint.Y))
                            .Select(x => new Point(x.X, x.Y)).ToList();

                        gameMap.Shoot(player.PlayerType, crossShot, WeaponType.CrossShot);
                    }
                    else
                    {
                        throw new DestroyedShipException(
                            $"{player.Name}'s Cruiser has been destroyed and cannot use this shot");
                    }
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
