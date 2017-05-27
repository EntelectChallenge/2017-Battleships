using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Battleships.Properties;
using BotRunner.Harness.ConsoleHarness;
using BotRunner.Util;
using Domain.File;
using Domain.Games;
using Domain.Meta;
using GameEngine.Common;
using GameEngine.Engine;
using GameEngine.Loggers;
using GameEngine.MapGenerators;
using GameEngine.Renderers;
using Newtonsoft.Json;
using TestHarness.TestHarnesses.Bot;

namespace Battleships
{
    public class BattleshipsGame
    {
        public ILogger Logger = new InMemoryLogger();
        private string _runLocation;
        private BattleshipEngine _engine;

        public void StartNewGame(Options options)
        {
            FileHelper.WriteImmediate = !options.TournamentMode;
            var players = new List<Player>();
            var mapSize = options.MapSize.HasValue && options.MapSize.Value >= 1 && options.MapSize.Value <= 3
                ? options.MapSize.Value
                : 2;
            _runLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Replays");

            try
            {
                _engine = new BattleshipEngine { Logger = Logger };
                _engine.GameComplete += EngineOnGameComplete;
                _engine.RoundStarting += WriteStateFiles;
                _engine.RoundComplete += WriteEngineInfo;
                _engine.FirstRoundFailed += FirstRoundFailed;

                if (options.Pretty)
                {
                    _engine.RoundStarting += engine_RoundComplete;
                    _engine.GameStarted += EngineOnGameStarted;
                }
                // _engine.Game

                _runLocation = !String.IsNullOrEmpty(options.Log)
                    ? options.Log
                    : Path.Combine(_runLocation, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss-fff"));

                for (var i = 0; i < options.ConsolePlayers; i++)
                {
                    players.Add(new ConsoleHarness("Player " + (players.Count + 1)));
                }

                players.AddRange(
                    options.BotFolders.Select(
                            botFolder => LoadBot(botFolder, _runLocation, options.NoLimit, options.DebugMode))
                        .Where(player => player != null));

                if (options.ForceRebuild)
                {
                    foreach (String botFolder in options.BotFolders)
                    {
                        System.Console.Write("{0}\n", botFolder);
                        BuildBot(botFolder).Compile();
                    }
                    Logger.LogInfo("done.");
                }


                if (players.Count == 0)
                {
                    for (var i = 0; i < 2; i++)
                    {
                        players.Add(new ConsoleHarness("Player " + (players.Count + 1)));
                    }
                }


                foreach (var player in players)
                {
                    Logger.LogInfo("Registered player " + player.Name);
                }

                _engine.PrepareGame(players, (MapSizes)mapSize);

                //WriteStateFiles(_engine.GetGameState(), 0);
                _engine.StartNewGame();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            FileHelper.FlushCache();
        }

        private void FirstRoundFailed(GameMap gameMap)
        {
            foreach (var player in _engine.Players)
            {
                player.FirstRoundFailed(gameMap);
            }
        }

        private void EngineOnGameStarted(GameMap gameMap)
        {
            foreach (var player in gameMap.RegisteredPlayers)
            {
                ConsoleRender.RenderToConsolePretty(gameMap, player.PlayerType);
            }
        }

        void engine_RoundComplete(GameMap gameMap, int round)
        {
            foreach (var player in gameMap.RegisteredPlayers)
            {
                ConsoleRender.RenderToConsolePretty(gameMap, player.PlayerType);
            }
        }

        private void EngineOnGameComplete(GameMap gameMap, List<Player> leaderBoard)
        {
            Logger.LogInfo("Game has ended");
            Logger.LogInfo("Leader Board");
            for (int i = 0; i < leaderBoard.Count; i++)
            {
                Logger.LogInfo(i + ": " + leaderBoard[i]);
            }
        }

        private void WriteStateFiles(GameMap gameMap, int round)
        {
            var roundMapPath = RoundPath(gameMap, round);
            var mapPlayer1Location = Path.Combine(_runLocation, roundMapPath);
            var mapPlayer2Location = Path.Combine(_runLocation, roundMapPath);
            var stateLocation = Path.Combine(_runLocation, roundMapPath);
            var roundInfoLocation = Path.Combine(_runLocation, roundMapPath);

            if (!Directory.Exists(mapPlayer1Location))
                Directory.CreateDirectory(mapPlayer1Location);

            mapPlayer1Location = Path.Combine(mapPlayer1Location,
                gameMap.GetBattleshipPlayer(PlayerType.One).Key + " - map.txt");
            mapPlayer2Location = Path.Combine(mapPlayer2Location,
                gameMap.GetBattleshipPlayer(PlayerType.Two).Key + " - map.txt");
            stateLocation = Path.Combine(stateLocation, "state.json");
            roundInfoLocation = Path.Combine(roundInfoLocation, "roundInfo.json");

            if (File.Exists(mapPlayer1Location))
                File.Delete(mapPlayer1Location);
            if (File.Exists(mapPlayer2Location))
                File.Delete(mapPlayer2Location);
            if (File.Exists(stateLocation))
                File.Delete(stateLocation);
            if (File.Exists(roundInfoLocation))
                File.Delete(roundInfoLocation);

            var renderer = new GameMapRender(gameMap, true);
            var json = renderer.RenderJsonGameState(PlayerType.Both);
            var mapPlayerOne = renderer.RenderTextGameState(PlayerType.One);
            var mapPlayerTwo = renderer.RenderTextGameState(PlayerType.Two);
            var roundInfo = GenerateRoundInfo(gameMap);

            FileHelper.WriteAllText(mapPlayer1Location, mapPlayerOne.ToString());
            FileHelper.WriteAllText(mapPlayer2Location, mapPlayerTwo.ToString());
            FileHelper.WriteAllText(stateLocation, json.ToString());
            FileHelper.WriteAllText(roundInfoLocation, roundInfo);
        }

        private void WriteEngineInfo(GameMap gameMap, int round)
        {
            var engineLog = Path.Combine(_runLocation, RoundPath(gameMap, round));

            if (!Directory.Exists(engineLog))
                Directory.CreateDirectory(engineLog);

            engineLog = Path.Combine(engineLog, "engine.log");

            if (File.Exists(engineLog))
                File.Delete(engineLog);

            FileHelper.WriteAllText(engineLog, Logger.ReadAll());
        }

        private string GenerateRoundInfo(GameMap gameMap)
        {
            var roundInfo = new RoundInfo()
            {
                MapSize = gameMap.MapSize,
                Round = gameMap.CurrentRound,
                Winner = GetPlayerInfo(_engine.Winner),
                Players = _engine.Players.Select(GetPlayerInfo),
                LeaderBoard = _engine.LeaderBoard.Select(GetPlayerInfo)
            };

            return JsonConvert.SerializeObject(roundInfo);
        }

        private PlayerInfo GetPlayerInfo(Player player)
        {
            if (player == null)
                return null;

            var botPlayer = player as BotHarness;
            return new PlayerInfo()
            {
                Email = botPlayer == null ? player.Name : botPlayer.BotMeta.Email,
                NickName = botPlayer == null ? player.Name : botPlayer.BotMeta.NickName,
                Author = botPlayer == null ? player.Name : botPlayer.BotMeta.Author,
                BotType = botPlayer == null ? "Unkown" : botPlayer.BotMeta.BotType.ToString()
            };
        }

        private Player LoadBot(String botLocation, String logLocation, bool noLimit, bool haltOnError)
        {
            try
            {
                var botMeta = BotMetaReader.ReadBotMeta(botLocation);

                Logger.LogInfo("Loaded bot " + botLocation);

                return new BotHarness(botMeta, botLocation, logLocation, noLimit, haltOnError, EnvironmentSettings);
            }
            catch (Exception ex)
            {
                Logger.LogException("Failed to load bot " + botLocation, ex);
                throw new Exception("Failed to load bot " + botLocation + Environment.NewLine + ex);
            }
        }

        private BotCompiler BuildBot(String botLocation)
        {
            try
            {
                var botMeta = BotMetaReader.ReadBotMeta(botLocation);

                Logger.LogInfo("Loaded bot to build " + botLocation);

                return new BotCompiler(botMeta, botLocation, Logger, EnvironmentSettings);
            }
            catch (Exception ex)
            {
                Logger.LogException("Failed to build bot " + botLocation, ex);
                throw new Exception("Failed to build bot " + botLocation + Environment.NewLine + ex);
            }

        }

        private string RoundPath(GameMap gameMap, int round)
        {
            return "Phase " + gameMap.Phase + " - Round " + round;
        }

        public EnvironmentSettings EnvironmentSettings =>

            new EnvironmentSettings()
            {
                PathToCargo = Settings.Default.PathToCargo,
                PathToGolang = Settings.Default.PathToGolang,
                PathToJava = Settings.Default.PathToJava,
			    PathToJulia = Settings.Default.PathToJulia,
                PathToMSBuild = Settings.Default.PathToMSBuild,
                PathToMaven = Settings.Default.PathToMaven,
                PathToNode = Settings.Default.PathToNode,
                PathToNpm = Settings.Default.PathToNpm,
                PathToPython2 = Settings.Default.PathToPython2,
                PathToPython3 = Settings.Default.PathToPython3,
                PathToPythonPackageIndex = Settings.Default.PathToPythonPackageIndex,
                PathToXBuild = Settings.Default.PathToXBuild
            };
    }
}
