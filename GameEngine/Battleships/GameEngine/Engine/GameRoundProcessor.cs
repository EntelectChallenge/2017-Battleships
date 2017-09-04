using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Games;
using Domain.JSON;
using Domain.Maps;
using Domain.Players;
using Domain.Ships;
using GameEngine.Commands;
using GameEngine.Commands.PlayerCommands;
using GameEngine.Common;
using GameEngine.Exceptions;
using GameEngine.Loggers;
using GameEngine.Properties;

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
            DestroyShips();
            AddEnergyToPlayers();
            UpdateShields();
            KillOffPlayers();
            _logger.LogDebug("Round processing complete");
            _roundProcessed = true;
            return proccessed;
        }

        protected void UpdateShields()
        {
            if (_gameMap.Phase == 2)
            {
                foreach (var player in _gameMap.RegisteredPlayers)
                {
                    var shield = player.Shield;
                    var currentRound = _gameMap.CurrentRound;
                    var difference = currentRound - shield.RoundLastUsed;
                    if (!shield.Active)
                    {
                        if (difference != 0 && difference % shield.ChargeTime == 0)
                        {
                            shield.CurrentCharges++;
                            shield.GrowRadius();
                        }
                    }
                    else
                    {
                        if (difference != 0 && --shield.CurrentCharges == 0)
                        {
                            shield.Active = false;
                            shield.RoundLastUsed = currentRound;
                            shield.CurrentRadius = 0;
                            _gameMap.RemoveShield(player.PlayerType);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds 2/3/4 points of energy to each player, depending on the current size of the map
        /// </summary>
        protected void AddEnergyToPlayers()
        {
            if (_gameMap.Phase == 2)
            {
                var energyToAdd = Settings.Default.EnergySmallMap;
                if (_gameMap.MapSize == Settings.Default.MediumMapSize)
                {
                    energyToAdd = Settings.Default.EnergyMediumMap;
                }
                else if (_gameMap.MapSize == Settings.Default.LargeMapSize)
                {
                    energyToAdd = Settings.Default.EnergyLargeMap;
                }
                foreach (var player in _gameMap.RegisteredPlayers)
                {
                    player.Energy += energyToAdd;
                }
            }
        }

        /// <summary>
        /// Process the player commands.  
        /// </summary>
        protected bool ProcessPlayerCommands()
        {
            _logger.LogDebug("Processing Player Commands");
            foreach (var command in _commandsToProcess)
            {
                SetCommandTypeAndCenterPoint(command.Value, command.Key.BattleshipPlayer);


                if (command.Key.BattleshipPlayer.Killed)
                {
                    _logger.LogInfo(
                        $"Player {command.Key.BattleshipPlayer} has been killed, and the command {command.Value} will be ignored");
                    continue;
                }

                try
                {
                    if (_gameMap.Phase == 1 && !(command.Value is PlaceShipCommand))
                    {
                        throw new InvalidCommandException(
                            $"There was a problem during the placement of player's {command.Key.BattleshipPlayer} ships (DoNothing), the round will be played over");
                    }

                    command.Value.PerformCommand(_gameMap, command.Key.BattleshipPlayer);
                    command.Key.BattleshipPlayer.SuccessfulMove = true;
                }
                catch (InvalidCommandException ex)
                {
                    command.Key.BattleshipPlayer.SuccessfulMove = false;
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

        protected void DestroyShips()
        {
            foreach (var player in _gameMap.RegisteredPlayers)
            {
                foreach (var playerShip in player.Ships)
                {
                    playerShip.Destroyed = playerShip.Cells.All(x => x != null && x.Hit);
                }
            }
        }

        public void ResetBackToStart()
        {
            _gameMap.CleanMapBeforePlace(PlayerType.One);
            _gameMap.CleanMapBeforePlace(PlayerType.Two);
        }

        protected void SetCommandTypeAndCenterPoint(ICommand command, BattleshipPlayer player)
        {
            if (command is FireSingleShotCommand)
            {
                player.CommandIssued = CommandType.FireSingleShot.ToString();
                player.CommandCenterPoint = ((FireSingleShotCommand) command).Point;
            }
            else if (command is PlaceShipCommand)
            {
                player.CommandIssued = CommandType.PlaceShip.ToString();
                player.PlaceShipString = ((PlaceShipCommand) command).OriginalString;
            }
            else if (command is PlaceShieldCommand)
            {
                player.CommandIssued = CommandType.PlaceShield.ToString();
                player.CommandCenterPoint = ((PlaceShieldCommand) command).Point;
            }
            else if (command is FireSeekerMissileCommand)
            {
                player.CommandIssued = CommandType.FireSeekerMissile.ToString();
                player.CommandCenterPoint = ((FireSeekerMissileCommand) command).CenterPoint;
            }
            else if (command is FireCornerrShotCommand)
            {
                player.CommandIssued = CommandType.FireCornerShot.ToString();
                player.CommandCenterPoint = ((FireCornerrShotCommand) command).CenterPoint;
            }
            else if (command is FireCrossShotCommand)
            {
                if (((FireCrossShotCommand) command).Diagonal)
                {
                    player.CommandIssued = CommandType.FireCrossShotDiagonal.ToString();
                    player.CommandCenterPoint = ((FireCrossShotCommand) command).CenterPoint;
                }
                else
                {
                    player.CommandIssued = CommandType.FireCrossShotNormal.ToString();
                    player.CommandCenterPoint = ((FireCrossShotCommand) command).CenterPoint;
                }
            }
            else if (command is FireDoubleShotCommand)
            {
                if (((FireDoubleShotCommand) command).Direction == Direction.North ||
                    ((FireDoubleShotCommand) command).Direction == Direction.South)
                {
                    player.CommandIssued = CommandType.FireDoubleShotVertical.ToString();
                    player.CommandCenterPoint = ((FireDoubleShotCommand) command).CenterPoint;
                }
                else
                {
                    player.CommandIssued = CommandType.FireDoubleShotHorizontal.ToString();
                    player.CommandCenterPoint = ((FireDoubleShotCommand) command).CenterPoint;
                }
            }
            else
            {
                player.CommandIssued = CommandType.DoNothing.ToString();
            }
        }
    }
}