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
        public readonly Point CenterPoint;
        public readonly Direction Direction;

        public FireDoubleShotCommand(Point centerPoint, Direction direction)
        {
            this.CenterPoint = centerPoint;
            this.Direction = direction;
        }
        public void PerformCommand(GameMap gameMap, BattleshipPlayer player)
        {
            try
            {
                if (player.Ships.Any(x => x.ShipType == ShipType.Destroyer && !x.Destroyed))
                {
                    var doubleShot = new List<Point>();
                    var opponentMap = gameMap.GetOpponetMap(player.PlayerType);

                    if (Direction == Direction.North || Direction == Direction.South)
                    {
                        doubleShot = opponentMap.Cells
                            .Where(cell => (cell.X == CenterPoint.X && cell.Y + 1 == CenterPoint.Y) ||
                                           (cell.X == CenterPoint.X && cell.Y - 1 == CenterPoint.Y))
                            .Select(x => new Point(x.X, x.Y)).ToList();
                    }
                    else if (Direction == Direction.East || Direction == Direction.West)
                    {
                        doubleShot = opponentMap.Cells
                            .Where(cell => (cell.X + 1 == CenterPoint.X && cell.Y == CenterPoint.Y) ||
                                           (cell.X - 1 == CenterPoint.X && cell.Y == CenterPoint.Y))
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
