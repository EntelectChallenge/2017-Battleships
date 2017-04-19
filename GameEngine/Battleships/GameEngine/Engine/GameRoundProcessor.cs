using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Games;
using Domain.Players;
using Domain.Ships;
using GameEngine.Commands;
using GameEngine.Common;
using GameEngine.Exceptions;
using GameEngine.Loggers;

namespace GameEngine.Engine
{
    public class GameRoundProcessor
    {
        private readonly Dictionary<Player, ICommand> _commandsToProcess = new Dictionary<Player, ICommand>();
        private readonly HashSet<BattleshipPlayer> _killPlayerEntities = new HashSet<BattleshipPlayer>();
        private readonly GameMap _gameMap;
        private readonly int _round;
        private readonly ILogger _logger;
        private bool _roundProcessed = false;
        private readonly int _playerKillPoints;

        public GameRoundProcessor(int round, GameMap gameMap, ILogger logger, int playerKillPoints)
        {
            this._round = round;
            _gameMap = gameMap;
            _logger = logger;
            _playerKillPoints = playerKillPoints;
        }

        /// <summary>
        /// The amount of commands recieved by this processor
        /// </summary>
        /// <returns></returns>
        public int CountCommandsRecieved()
        {
            return _commandsToProcess.Count();
        }


        public HashSet<BattleshipPlayer> KillPlayerEntities
        {
            get { return _killPlayerEntities; }
        }

        /// <summary>
        /// Adds a players command to the queue of commands, only one command per player per round is allowed
        /// </summary>
        /// <param name="player">The player who the command belongs to</param>
        /// <param name="command">The command of the player to process</param>
        public void AddPlayerCommand(Player player, ICommand command)
        {
            try
            {
                if (_commandsToProcess.ContainsKey(player))
                    throw new InvalidCommandException(
                        "Player already has a command registered for this round, wait for the next round before sending a new command");

                if (player.BattleshipPlayer.Killed)
                    throw new InvalidCommandException("Player has been killed and can longer send commands");

                _logger.LogInfo($"Added Command {command} for Player {player.BattleshipPlayer}");
                _commandsToProcess.Add(player, command);
            }
            catch (InvalidCommandException ice)
            {
                player.PlayerCommandFailed(command, ice.Message);
            }
        }

        /// <summary>
        /// Performs all of the processing logic for this round of play
        /// </summary>
        public bool ProcessRound()
        {
            if (_roundProcessed)
            {
                throw new InvalidOperationException("This round has already been processed!");
            }
            _logger.LogDebug("Beginning round processing");
            var proccessed = ProcessPlayerCommands();
            if (proccessed == false && _gameMap.Phase == 1)
            {
                return false;
            }
            KillOffPlayers();
            _logger.LogDebug("Round processing complete");
            _roundProcessed = true;
            return proccessed;
        }

        /// <summary>
        /// Process the player commands.  
        /// </summary>
        protected bool ProcessPlayerCommands()
        {
            _logger.LogDebug("Processing Player Commands");
            foreach (var command in _commandsToProcess)
            {
                if (command.Key.BattleshipPlayer.Killed)
                {
                    _logger.LogInfo(
                        $"Player {command.Key.BattleshipPlayer} has been killed, and the command {command.Value} will be ignored");
                    continue;
                }

                try
                {
                    command.Value.PerformCommand(_gameMap, command.Key.BattleshipPlayer);
                }
                catch (InvalidCommandException ex)
                {
                    command.Key.PlayerCommandFailed(command.Value, ex.Message);
                    _logger.LogException(
                        $"Failed to process command {command.Value} for player {command.Key.BattleshipPlayer}", ex);
                    if (_gameMap.Phase == 1)
                    {
                        command.Key.BattleshipPlayer.FailedFirstPhaseCommands++;
                        _gameMap.ReasonForFirstRoundFailure =
                            $"Player {command.Key.BattleshipPlayer} was unable to place all of his ships";
                        return false;
                    }
                }
            }
            _commandsToProcess.Clear();
            return true;
        }

        /// <summary>
        /// Checks to see if all the ships of a player has been destroyed
        /// </summary>
        protected void KillOffPlayers()
        {
            var playerOne = _gameMap.RegisteredPlayers[0];
            var playerTwo = _gameMap.RegisteredPlayers[1];
            _logger.LogDebug("Killing off players who have no ships left");
            foreach (var battleshipPlayer in _gameMap.RegisteredPlayers)
            {
                if (battleshipPlayer.ShipsRemaining != 0) continue;
                if (battleshipPlayer == playerOne)
                {
                    playerTwo.AddPoints(_playerKillPoints);
                }
                else
                {
                    playerOne.AddPoints(_playerKillPoints);
                }
                battleshipPlayer.Killed = true;
                KillPlayerEntities.Add(battleshipPlayer);
                _logger.LogDebug($"Killed player {battleshipPlayer}");
            }
        }

        public void ResetBackToStart()
        {
            _gameMap.CleanMapBeforePlace(PlayerType.One);
            _gameMap.CleanMapBeforePlace(PlayerType.Two);
        }
    }
}
