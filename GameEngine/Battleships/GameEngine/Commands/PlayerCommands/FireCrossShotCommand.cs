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
        public readonly Point CenterPoint;
        public readonly bool Diagonal;

        public FireCrossShotCommand(Point centerPoint, bool diagonal)
        {
            this.CenterPoint = centerPoint;
            this.Diagonal = diagonal;
        }

        public void PerformCommand(GameMap gameMap, BattleshipPlayer player)
        {
            try
            {
                var opponentsMap = gameMap.GetOpponetMap(player.PlayerType);
                var crossShot = new List<Point>();

                if (Diagonal)
                {
                    if (player.Ships.Any(x => x.ShipType == ShipType.Battleship && !x.Destroyed))
                    {
                        crossShot = opponentsMap.Cells
                            .Where(cell => (cell.X == CenterPoint.X && cell.Y == CenterPoint.Y)
                                           || (cell.X + 1 == CenterPoint.X && cell.Y - 1 == CenterPoint.Y)
                                           || (cell.X - 1 == CenterPoint.X && cell.Y - 1 == CenterPoint.Y)
                                           || (cell.X + 1 == CenterPoint.X && cell.Y + 1 == CenterPoint.Y)
                                           || (cell.X - 1 == CenterPoint.X && cell.Y + 1 == CenterPoint.Y))
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
                            .Where(cell => (cell.X == CenterPoint.X && cell.Y == CenterPoint.Y)
                                           || (cell.X + 1 == CenterPoint.X && cell.Y == CenterPoint.Y)
                                           || (cell.X == CenterPoint.X && cell.Y - 1 == CenterPoint.Y)
                                           || (cell.X == CenterPoint.X && cell.Y + 1 == CenterPoint.Y)
                                           || (cell.X - 1 == CenterPoint.X && cell.Y == CenterPoint.Y))
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
