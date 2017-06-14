using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BotRunner.Exceptions;
using BotRunner.Properties;
using BotRunner.Util;
using Domain.Maps;
using GameEngine.Commands;
using GameEngine.Commands.PlayerCommands;
using Newtonsoft.Json;

namespace TestHarness.TestHarnesses.Bot
{
    public abstract class BotRunner
    {
        protected readonly BotHarness ParentHarness;
        protected TimeSpan MaxRunTime;
        private bool _errorLogged = false;

        protected BotRunner(BotHarness parentHarness)
        {
            ParentHarness = parentHarness;

            MaxRunTime = TimeSpan.FromSeconds(Settings.Default.MaxBotRuntimeSeconds);
        }

        protected string AddAdditionalRunArgs(string currentArgs)
        {
            var runArgs = ParentHarness.BotMeta.RunArgs;

            return String.IsNullOrEmpty(runArgs) ? currentArgs : $"{currentArgs} {runArgs}";
        }

        public void CalibrateBot()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            try
            {
                RunCalibrationTest();
            }
            catch(Exception e)
            {
                ParentHarness.Logger.LogException("Failed to run calibration test " + e);
            }
            stopWatch.Stop();

            MaxRunTime = MaxRunTime.Add(stopWatch.Elapsed);

            ParentHarness.Logger.LogInfo(
                $"Bot calibration complete and can run an additional {stopWatch.ElapsedMilliseconds}ms due to environment startup");
        }

        public void RunBot()
        {
            using (var handler = CreateProcessHandler())
            {
                var sw = new Stopwatch();
                try
                {
                    handler.LimitExecutionTime = ParentHarness.EnforceTimeLimit;
                    handler.ProcessToRun.OutputDataReceived +=
                        (sender, args) => ParentHarness.Logger.LogInfo("Output from bot: " + args.Data);
                    handler.ProcessToRun.ErrorDataReceived +=
                        (sender, args) => ParentHarness.Logger.LogException("Output from bot: " + args.Data);

                    if (ParentHarness.HaltOnError)
                    {
                        handler.ProcessToRun.ErrorDataReceived +=
                            (sender, args) => { _errorLogged = _errorLogged || !String.IsNullOrEmpty(args.Data); };
                    }

                    ParentHarness.Logger.LogDebug(
                        $"Executing bot with following commands {handler.ProcessToRun.StartInfo.FileName} {handler.ProcessToRun.StartInfo.Arguments}");
                    sw.Start();
                    handler.RunProcess();
                    sw.Stop();

                    ParentHarness.Logger.LogInfo("Your bots total execution time was " + sw.Elapsed);

                    HaltOnError();
                }
                catch (Exception ex)
                {
                    ParentHarness.Logger.LogException(
                        "Failure while executing bot " + handler.ProcessToRun.StartInfo.FileName + " " +
                        handler.ProcessToRun.StartInfo.Arguments, ex);

                    _errorLogged = true;
                    HaltOnError();
                }

                if (ParentHarness.EnforceTimeLimit && sw.Elapsed >= MaxRunTime)
                {
                    ParentHarness.Logger.LogInfo("Your bot exceeded the maximum execution time");
                    throw new TimeLimitExceededException("Time limit exceeded by " + (sw.Elapsed - MaxRunTime));
                }
            }
        }

        private void HaltOnError()
        {
            if (ParentHarness.HaltOnError && _errorLogged)
            {
                Console.WriteLine(ParentHarness.Logger.ReadAll());
                Console.WriteLine(
                    $"Bot {ParentHarness.BattleshipPlayer} encountered an error, press any key to continue");
                Console.ReadKey();
            }
        }

        public ICommand GetBotCommand()
        {
            try
            {
                var placeShipCommand = GetBotPlaceCommandFromFile();
                var otherCommand = GetBotCommandFromFile();

                if (placeShipCommand != null)
                {
                    return new PlaceShipCommand(placeShipCommand.Ships, placeShipCommand.Points,
                        placeShipCommand.Directions);
                }
                if (otherCommand == null) return new DoNothingCommand();
                switch (otherCommand.Code)
                {
                    case 1:
                        return new FireSingleShotCommand(otherCommand.Point);
                    case 2:
                        return new FireDoubleShotCommand(otherCommand.Point, Direction.North);
                    case 3:
                        return new FireDoubleShotCommand(otherCommand.Point, Direction.East);
                    case 4:
                        return new FireCornerrShotCommand(otherCommand.Point);
                    case 5:
                        return new FireCrossShotCommand(otherCommand.Point, true);
                    case 6:
                        return new FireCrossShotCommand(otherCommand.Point, false);
                    case 7:
                        return new FireSeekerMissleCommand(otherCommand.Point);
                    default:
                        return new DoNothingCommand();
                }
            }
            catch (Exception ex)
            {
                ParentHarness.Logger.LogException("Failed to read move file from bot", ex);
                return new DoNothingCommand();
            }
        }

        private GeneralCommand GetBotCommandFromFile()
        {
            var workDir = ParentHarness.CurrentWorkingDirectory;
            var commandLocation = Path.Combine(workDir, Settings.Default.CommandFileTxt);

            if (!File.Exists(commandLocation))
                return null;

            var basicCommand = File.ReadAllText(commandLocation).Split(',');
            if (basicCommand.Length > 3)
            {
                throw new ArgumentException("There needs to be 3 numbers seperated by commas in the general command file");
            }

            return new GeneralCommand(Convert.ToInt32(basicCommand[0]), Convert.ToInt32(basicCommand[1]),
                Convert.ToInt32(basicCommand[2]));
        }

        private StringToPlaceShipCommand GetBotPlaceCommandFromFile()
        {
            var workDir = ParentHarness.CurrentWorkingDirectory;
            var placeFile = Path.Combine(workDir, Settings.Default.ShipPlacementFile);

            if (!File.Exists(placeFile))
            {
                return null;
            }

            try
            {
                var placeShipCommands = File.ReadAllText(placeFile);
                var commands = placeShipCommands.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                if (commands.Length != 5)
                {
                    throw new ArgumentException("There needs to be 5 ships in the place ship file");
                }
                return new StringToPlaceShipCommand(commands);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected abstract ProcessHandler CreateProcessHandler();
        protected abstract void RunCalibrationTest();
    }
}