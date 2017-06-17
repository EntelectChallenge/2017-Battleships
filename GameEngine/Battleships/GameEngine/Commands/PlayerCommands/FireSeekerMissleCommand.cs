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
    public class FireSeekerMissleCommand : ICommand
    {
        private readonly Point _centerPoint;

        public FireSeekerMissleCommand(Point centerPoint)
        {
            _centerPoint = centerPoint;
        }

        public void PerformCommand(GameMap gameMap, BattleshipPlayer player)
        {
            try
            {
                if (player.Ships.Any(x => x.ShipType == ShipType.Submarine && !x.Destroyed))
                {
                    var opponentsMap = gameMap.GetOpponetMap(player.PlayerType);

                    var occupiedCells = opponentsMap.Cells.Where(cell => cell.Occupied).ToList();

                    var cellInRage = occupiedCells.Select(x => new
                    {
                        distance = Math.Sqrt(Math.Pow((x.X - _centerPoint.X), 2) + Math.Pow(x.Y - _centerPoint.Y, 2)),
                        cell = x
                    }).OrderBy(x => x.distance).FirstOrDefault(x => x.distance <= 2);

                    var cellToHit = cellInRage == null
                        ? new Point(_centerPoint.X, _centerPoint.Y)
                        : new Point(cellInRage.cell.X, cellInRage.cell.Y);

                    gameMap.Shoot(player.PlayerType, new List<Point> {cellToHit}, WeaponType.SeekerMissle);
                }
                else
                {
                    throw new DestroyedShipException(
                        $"{player.Name}'s Submarine has been destroyed and cannot use this shot");
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
