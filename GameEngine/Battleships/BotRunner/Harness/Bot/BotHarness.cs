using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using BotRunner.Exceptions;
using BotRunner.Properties;
using BotRunner.Util;
using Domain.Bot;
using Domain.File;
using Domain.Games;
using Domain.Meta;
using GameEngine.Commands;
using GameEngine.Commands.PlayerCommands;
using GameEngine.Common;
using GameEngine.Loggers;
using GameEngine.Renderers;
using TestHarness.TestHarnesses.Bot.Runners;

namespace TestHarness.TestHarnesses.Bot
{
    public class BotHarness : Player
    {
        public readonly BotMeta BotMeta;
        public readonly string BotDir;
        public readonly string WorkDir;
        public readonly bool EnforceTimeLimit;
        public readonly bool HaltOnError;
        private readonly InMemoryLogger _inMemoryLogger;
        private readonly BotRunner _botRunner;
        private int _currentRound = 0;
        private int _totalDoNothingCommands;

        public BotHarness(BotMeta meta, string botDir, string workDir, bool noTimeLimit, bool haltOnError, EnvironmentSettings environmentSettings)
            : base(meta.NickName ?? meta.Author ?? meta.Email)
        {
            BotMeta = meta;
            BotDir = botDir;
            WorkDir = workDir;
            EnforceTimeLimit = !noTimeLimit;
            HaltOnError = haltOnError;

            _inMemoryLogger = new InMemoryLogger();
            Logger = _inMemoryLogger;

            switch (meta.BotType)
            {
                case BotMeta.BotTypes.CSharp:
                case BotMeta.BotTypes.CPlusPlus:
                case BotMeta.BotTypes.FSharp:
                    _botRunner = new DotNetRunner(this, environmentSettings);
                    break;
                case BotMeta.BotTypes.Python2:
                case BotMeta.BotTypes.Python3:
                    _botRunner = new PythonRunner(this, environmentSettings);
                    break;
                case BotMeta.BotTypes.Java:
                case BotMeta.BotTypes.Scala:
                    _botRunner = new JavaRunner(this, environmentSettings);
                    break;
                case BotMeta.BotTypes.Golang:
                    _botRunner = new GolangRunner(this, environmentSettings);
                    break;
                case BotMeta.BotTypes.JavaScript:
                    _botRunner = new JavaScriptRunner(this, environmentSettings);
                    break;
                case BotMeta.BotTypes.Julia:
                    _botRunner = new JuliaRunner(this, environmentSettings);
                    break;
                case BotMeta.BotTypes.Rust:
                    _botRunner = new RustRunner(this, environmentSettings);
                    break;
                default:
                    throw new ArgumentException("Invalid bot type " + meta.BotType);
            }
        }

        public override void StartGame(GameMap gameState)
        {
            CurrentWorkingDirectory = Path.Combine(Path.Combine(WorkDir, RoundPath(gameState, _currentRound)),
                BattleshipPlayer.Key.ToString(CultureInfo.InvariantCulture));
            WriteRoundFiles(gameState);
            _botRunner.CalibrateBot(); //
            RemoveCommandFile(); //Remove the move file created by calibration bots
            NewRoundStarted(gameState);
        }

        public override void NewRoundStarted(GameMap gameState)
        {
            _currentRound = gameState.CurrentRound;
            CurrentWorkingDirectory = Path.Combine(Path.Combine(WorkDir, RoundPath(gameState, _currentRound)),
                BattleshipPlayer.Key.ToString(CultureInfo.InvariantCulture));
            WriteRoundFiles(gameState);
            if (!BattleshipPlayer.Killed)
            {
                RunBotAndGetNextMove();
            }
            ClearRoundFiles();
        }

        public override void GameEnded(GameMap gameMap)
        {
            Logger.LogInfo("Game has ended");
            WriteRoundFiles(gameMap);
        }

        public override void PlayerKilled(GameMap gameMap)
        {
            Logger.LogInfo("Player has been killed");
        }

        public override void PlayerCommandFailed(ICommand command, string reason)
        {
            Logger.LogInfo($"Player Command {command} failed because {reason}");
        }

        public override void FirstRoundFailed(GameMap gameMap)
        {
            Logger.LogInfo("The first round has failed due to a bot\'s ships not all being placed");
        }

        public override void Dispose()
        {
        }

        private void RunBotAndGetNextMove()
        {
            if (_totalDoNothingCommands >= 10)
            {
                Logger.LogInfo(
                    "Bot is sending to many do nothing commands, if this continues it the bot will be killed off");
            }
            if (_totalDoNothingCommands >= 20)
            {
                BotUnresponsive();
                Logger.LogInfo(
                    "Bot sent to many do nothing commands, something is most likely going wrong, please fix your bot. The player's ships will all be marked as destroyed and killed off.");
                BattleshipPlayer.Killoff();
            }

            if (BattleshipPlayer.FailedFirstPhaseCommands == 5)
            {
                Logger.LogInfo("Bot has failed to place ships in the last 5 rounds and will be killed off");
                BattleshipPlayer.Killoff();
            }

            ICommand command;
            try
            {
                _botRunner.RunBot();
                command = _botRunner.GetBotCommand();
            }
            catch (TimeLimitExceededException ex)
            {
                Logger.LogException("Bot time limit exceeded ", ex);
                command = new DoNothingCommand();
            }

            if (command.GetType() == typeof(DoNothingCommand))
            {
                _totalDoNothingCommands++;
            }
            else
            {
                _totalDoNothingCommands = 0;
            }

            WriteLogs();
            RemoveCommandFile(true);
            BotCommandPublished(command);
            PublishCommand(command);
        }

        private void ClearRoundFiles()
        {
            /*var dir = Path.Combine(CurrentWorkingDirectory, Settings.Default.StateFileName);

            if (File.Exists(dir))
                File.Delete(dir);

            dir = Path.Combine(CurrentWorkingDirectory, Settings.Default.MapFileName);

            if (File.Exists(dir))
                File.Delete(dir);*/
        }

        private void RemoveCommandFile(bool restoreContent = false)
        {
            var dir = Path.Combine(CurrentWorkingDirectory, Settings.Default.ShipPlacementFile);

            if (File.Exists(dir))
            {
                var content = File.ReadAllText(dir);
                File.Delete(dir);
                if (restoreContent)
                {
                    FileHelper.WriteAllText(dir, content);
                }
            }

            dir = Path.Combine(CurrentWorkingDirectory, Settings.Default.CommandFileTxt);

            if (File.Exists(dir))
            {
                var content = File.ReadAllText(dir);
                File.Delete(dir);
                if (restoreContent)
                {
                    FileHelper.WriteAllText(dir, content);
                }
            }
        }

        private void WriteRoundFiles(GameMap gameMap)
        {
            if (!Directory.Exists(CurrentWorkingDirectory))
                Directory.CreateDirectory(CurrentWorkingDirectory);

            WriteStateFile(gameMap);
            WriteMapFile(gameMap);
        }

        private void WriteStateFile(GameMap gameMap)
        {
            var dir = Path.Combine(CurrentWorkingDirectory, Settings.Default.StateFileName);
            var renderer = new GameMapRender(gameMap, true);
            var json = renderer.RenderJsonGameState(BattleshipPlayer.PlayerType);

            File.WriteAllText(dir, json.ToString(), new UTF8Encoding(false));
        }

        private void WriteMapFile(GameMap gameMap)
        {
            var dir = Path.Combine(CurrentWorkingDirectory, Settings.Default.MapFileName);
            var renderer = new GameMapRender(gameMap, true);
            var map = renderer.RenderTextGameState(BattleshipPlayer.PlayerType);

            File.WriteAllText(dir, map.ToString(), new UTF8Encoding(false));
        }

        private void WriteLogs()
        {
            var dir = Path.Combine(CurrentWorkingDirectory, Settings.Default.LogFileName);

            FileHelper.WriteAllText(dir, _inMemoryLogger.ReadAll());
        }

        public string CurrentWorkingDirectory { get; set; /*get
            {
                var roundPath = Path.Combine(WorkDir, RoundPath(_currentRound));
                return Path.Combine(roundPath, BattleshipPlayer.Key.ToString(CultureInfo.InvariantCulture));
            }

            set { }*/ }

        private string RoundPath(GameMap gameMap, int round)
        {
            return $"Phase {gameMap.Phase} - Round {round}";
        }


        #region Tournament Methods

        public virtual void BotMaxExecution(long executionTime)
        {
            
        }

        public virtual void BotExecutionTime(long executionTime)
        {
            
        }

        public virtual void BotExecutionTimeLimitExceeded()
        {
            
        }

        public virtual void BotCommandPublished(ICommand command)
        {
            
        }

        public virtual void BotEncounteredExecutionException()
        {

        }

        public virtual void BotUnresponsive()
        {

        }

        #endregion
    }
}
