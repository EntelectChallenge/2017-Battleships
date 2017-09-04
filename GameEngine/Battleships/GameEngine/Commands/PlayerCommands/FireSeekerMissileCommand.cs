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
    public class FireSeekerMissileCommand : ICommand
    {
        public readonly Point CenterPoint;

        public FireSeekerMissileCommand(Point centerPoint)
        {
            CenterPoint = centerPoint;
        }

        public void PerformCommand(GameMap gameMap, BattleshipPlayer player)
        {
            try
            {
                if (player.Ships.Any(x => x.ShipType == ShipType.Submarine && !x.Destroyed))
                {
                    var opponentsMap = gameMap.GetOpponetMap(player.PlayerType);

                    var occupiedCells = opponentsMap.Cells.Where(cell => cell.Occupied && !cell.Hit && !cell.Shielded).ToList();

                    var cellToHit = CenterPoint;

                    if (occupiedCells.Any())
                    {
                        var cellsInRange = occupiedCells.Select(x => new
                        {
                            distance = Math.Sqrt(
                                Math.Pow((x.X - CenterPoint.X), 2) + Math.Pow(x.Y - CenterPoint.Y, 2)),
                            cell = x
                        }).OrderBy(x => x.distance);

                        var cellInRange = cellsInRange.FirstOrDefault(x => x.distance < 1d);

                        if (cellInRange == null)
                        {
                            cellInRange = cellsInRange.FirstOrDefault(x => x.distance < 1.4d);
                        }
                        if (cellInRange == null)
                        {
                            cellInRange = cellsInRange.FirstOrDefault(x => x.distance > 1.4d && x.distance < 2d);
                        }
                        if (cellInRange == null)
                        {
                            cellInRange = cellsInRange.FirstOrDefault(x => x.distance <= 2d);
                        }

                        cellToHit = cellInRange == null
                            ? cellToHit
                            : new Point(cellInRange.cell.X, cellInRange.cell.Y);
                    }

                    gameMap.Shoot(player.PlayerType, new List<Point> {cellToHit}, WeaponType.SeekerMissile);
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
