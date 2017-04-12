using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Domain.Games;
using Domain.Maps;
using Domain.Players;
using GameEngine.Exceptions;
using GameEngine.Loggers;

namespace GameEngine.Commands
{
    public class CommandTransaction
    {
        private readonly ILogger _logger;

        private readonly Dictionary<BattleshipPlayer, Point> _playerShotLocation =
            new Dictionary<BattleshipPlayer, Point>();

        public CommandTransaction(ILogger logger)
        {
            this._logger = logger;
        }

        public void RequestShotFired(BattleshipPlayer player, Point locationOfShot)
        {
            if (_playerShotLocation.ContainsKey(player))
            {
                throw new InvalidCommandException("Command Transaction already contains a destination shot for player " +
                                                  player);
            }
            _playerShotLocation.Add(player, locationOfShot);
        }

        public void ProcessCommands(GameMap gameMap)
        {
            while (_playerShotLocation.Any())
            {
                var playerShotLocation = _playerShotLocation.First();
                _playerShotLocation.Remove(playerShotLocation.Key);

                _logger.LogInfo($"Trying to fire shot for player {playerShotLocation.Key.Name} to location {playerShotLocation.Value}");

                var player = playerShotLocation.Key;
                var playerLocationShot = playerShotLocation.Value;

                //var opponentsMap = gameMap.GetOpponentsMap(player);
                //opponentsMap.
            }
        }
    }
}