using System;
using System.Collections.Generic;
using System.Drawing;

using Domain.Games;
using Domain.Maps;
using Domain.Players;
using Domain.Ships;
using GameEngine.Exceptions;

namespace GameEngine.Commands.PlayerCommands
{
    public class PlaceShipCommand : ICommand
    {
        private readonly List<ShipType> _ships;
        private readonly List<Point> _points;
        private readonly List<Direction> _directions;
        private readonly int _maxNumOfShips = 5;
        public string OriginalString { get; }

        public PlaceShipCommand(List<ShipType> ships, List<Point> points, List<Direction> directions, string originalString)
        {
            _ships = ships;
            _points = points;
            _directions = directions;
            this.OriginalString = originalString;
        }

        public void PerformCommand(GameMap gameMap, BattleshipPlayer player)
        {
            if (_ships == null || _points == null || _directions == null)
            {
                throw new InvalidCommandException($"There was a problem during the placement of player's {player} ships (Placement null), the round will be played over");
            }
            if (_ships.Count != _maxNumOfShips || _points.Count != _maxNumOfShips || _directions.Count != _maxNumOfShips)
            {
                throw new InvalidCommandException($"There was a problem during the placement of player's {player} ships (Invalid placement count), the round will be played over");
            }

            gameMap.CleanMapBeforePlace(player.PlayerType);
            var successfulPlace = true;
            for (var index = 0; index < _maxNumOfShips; index++)
            {
                var ship = _ships[index];
                var point = _points[index];
                var direction = _directions[index];

                if (direction == null)
                {
                    throw new InvalidCommandException($"A direction for {ship} is required for placement");
                }
                try
                {
                    successfulPlace = gameMap.CanPlace(player.PlayerType, ship, point, direction);
                }
                catch (Exception e)
                {
                    throw new InvalidCommandException($"There was a problem during the placement of player's {player} ships, the round will be played over", e);
                }
            }

            if (!successfulPlace)
            {
                throw new InvalidCommandException($"There was a problem during the placement of player's {player} ships, the round will be played over");
            }

            for (var index = 0; index < _maxNumOfShips; index++)
            {
                try
                {
                    var ship = _ships[index];
                    var point = _points[index];
                    var direction = _directions[index];

                    if (direction == null)
                    {
                        throw new InvalidCommandException($"A direction for {ship} is required for placement");
                    }
                    gameMap.Place(player.PlayerType, ship, point, direction);
                }
                catch (InvalidOperationException ioe)
                {
                    throw new InvalidCommandException("There was an issue during the placement of the ships", ioe);
                }
            }
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}