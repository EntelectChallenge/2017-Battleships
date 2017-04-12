using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Domain.Games;
using GameEngine.Commands;
using GameEngine.Common;
using GameEngine.Loggers;
using GameEngine.MapGenerators;
using GameEngine.Properties;

namespace GameEngine.Engine
{
    public class BattleshipEngine : ILogger
    {
        public delegate void GameStartedHandler(GameMap gameMap);

        public delegate void RoundCompleteHandler(GameMap gameMap, int round);

        public delegate void RoundStartingHandler(GameMap gameMap, int round);

        public delegate void GameCompleteHandler(GameMap gameMap, List<Player> leaderBoard);

        public delegate void FirstRoundHandler(GameMap gameMap);

        public event GameStartedHandler GameStarted;
        public event RoundCompleteHandler RoundComplete;
        public event RoundStartingHandler RoundStarting;
        public event GameCompleteHandler GameComplete;
        public event FirstRoundHandler FirstRoundFailed;

        private ILogger _logger = new NullLogger();
        private GameMap _gameMap;
        private List<Player> _players;
        private GameRoundProcessor _roundProcessor;
        private readonly int _playerKillPoints = Settings.Default.PointsPlayerKilled;
        private readonly ManualResetEvent _resetEvent = new ManualResetEvent(false);
        private bool _gameComplete = false;

        public void PrepareGame(List<Player> players, MapSizes mapSize)
        {
            _gameMap = new GameMapGenerator(players).GenerateGameMap(mapSize);
            _gameComplete = false;

            _players = players;

            foreach (var player in _players)
            {
                player.CommandListener += player_CommandListener;
            }

            Logger.LogInfo("\tOK!");
        }

        /// <summary>
        /// Starts the game.  Should be called after prepare game.  
        /// This will notify all registered players that the game has started, and they can start sending commands to the engine.
        /// </summary>
        public void StartNewGame()
        {
            if (_gameMap == null)
                throw new Exception("Game has not yet been prepared");

            StartNewRound();

            PublishGameStarted();

            _resetEvent.Reset();
            foreach (var player in _players)
            {
                var thread = new Thread(() => player.StartGame(_gameMap));
                thread.Start();
                thread.Join();
            }
            while (!_gameComplete)
            {
                ProcessRound();
            }
        }

        /// <summary>
        /// Notify all listeners that the current game round has started
        /// </summary>
        private void PublishGameStarted()
        {
            GameStarted?.Invoke(_gameMap);
        }

        /// <summary>
        /// Returns the current game map for this round
        /// </summary>
        /// <returns></returns>
        public GameMap GetGameState()
        {
            return _gameMap;
        }

        public Player Winner => LeaderBoard.FirstOrDefault();

        public List<Player> LeaderBoard
        {
            get
            {
                return
                    _players.OrderBy(x => x.BattleshipPlayer.Killed)
                        .ThenByDescending(x => x.BattleshipPlayer.Points)
                        .ThenBy(x => x.BattleshipPlayer.FirstShotLanded)
                        .ThenBy(x => x.BattleshipPlayer.FailedFirstPhaseCommands)
                        .ToList();
            }
        }

        public ReadOnlyCollection<Player> Players => new ReadOnlyCollection<Player>(_players);

        /// <summary>
        /// Processes this current round, then notifies all players of round completion.
        /// </summary>
        private void ProcessRound()
        {
            var successfulRound = _roundProcessor.ProcessRound();

            foreach (var player in _players)
            {
                player.RoundComplete(_gameMap, _gameMap.CurrentRound);
            }

            PublishRoundComplete();
            if (!successfulRound && _gameMap.Phase == 1)
            {
                _gameMap.SuccessfulFirstRound = false;
                _roundProcessor.ResetBackToStart();
                PublishFirstRoundFailed();
            }
            else
            {
                _gameMap.SuccessfulFirstRound = true;
                _gameMap.Phase = 2;
            }
            _gameMap.CurrentRound++;
            StartNewRound();

            if (_gameMap.RegisteredPlayers.Count(x => !x.Killed) <= 1)
            {
                PublishGameComplete();
                return;
            }

            if (_gameMap.CurrentRound > ((_gameMap.MapSize * _gameMap.MapSize) + 5))
            {
                PublishGameComplete();
                return;
            }

            foreach (var player in _players)
            {
                var thread = new Thread(() => player.NewRoundStarted(_gameMap));
                thread.Start();
                thread.Join();
            }
            _resetEvent.Reset();
        }

        private void PublishFirstRoundFailed()
        {
            FirstRoundFailed?.Invoke(_gameMap);
        }

        /// <summary>
        /// Notify all listeners that the current game round has been completed
        /// </summary>
        private void PublishRoundComplete()
        {
            RoundComplete?.Invoke(_gameMap, _gameMap.CurrentRound);
        }

        private void PublishGameComplete()
        {
            LogInfo("Game has ended, the winnig player is " +
                    (Winner == null ? "no one, game ended in a draw" : Winner.Name));
            LogInfo("Leader Board");
            for (int i = 0; i < LeaderBoard.Count; i++)
            {
                LogInfo(i + ": " + LeaderBoard[i]);
            }

            foreach (var player in _players)
            {
                player.GameEnded(_gameMap);
            }

            _gameComplete = true;
            GameComplete?.Invoke(_gameMap, LeaderBoard);
        }

        private void StartNewRound()
        {
            foreach (
                var player in
                _players.Where(
                    x => _roundProcessor != null && _roundProcessor.KillPlayerEntities.Contains(x.BattleshipPlayer)))
            {
                player.PlayerKilled(_gameMap);
            }

            _roundProcessor = new GameRoundProcessor(_gameMap.CurrentRound, _gameMap, Logger, _playerKillPoints);
            PublishRoundStarting();
        }


        /// <summary>
        /// Notify all listeners that the current game round is starting
        /// </summary>
        private void PublishRoundStarting()
        {
            RoundStarting?.Invoke(_gameMap, _gameMap.CurrentRound);
        }

        /// <summary>
        /// Player commands will passed on to the round processor.  Each player can only send one command per turn.
        /// Once the command for all of the players have been recieved, the round will be processed.
        /// </summary>
        /// <param name="player">The issuing player</param>
        /// <param name="command">The command to performa</param>
        void player_CommandListener(Player player, ICommand command)
        {
            _roundProcessor.AddPlayerCommand(player, command);

            if (_players.Count(x => !x.BattleshipPlayer.Killed) == 0)
            {
                _resetEvent.Set();
                return;
            }

            if (_roundProcessor.CountCommandsRecieved() == _players.Count(x => !x.BattleshipPlayer.Killed))
                //All players sent their commands, process the round
                _resetEvent.Set();
        }


        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public void LogDebug(string message)
        {
            Logger.LogDebug(message);

            foreach (var player in _players.Where(player => player.Logger != null))
            {
                player.Logger.LogDebug(message);
            }
        }

        public void LogInfo(string message)
        {
            Logger.LogInfo(message);

            foreach (var player in _players.Where(player => player.Logger != null))
            {
                player.Logger.LogInfo(message);
            }
        }

        public void LogException(string message)
        {
            Logger.LogException(message);

            foreach (var player in _players.Where(player => player.Logger != null))
            {
                player.Logger.LogException(message);
            }
        }

        public void LogException(Exception ex)
        {
            Logger.LogException(ex);

            foreach (var player in _players.Where(player => player.Logger != null))
            {
                player.Logger.LogException(ex);
            }
        }

        public void LogException(string message, Exception ex)
        {
            Logger.LogException(message, ex);

            foreach (var player in _players.Where(player => player.Logger != null))
            {
                player.Logger.LogException(message, ex);
            }
        }

        public string ReadAll()
        {
            return Logger.ReadAll();
        }
    }
}