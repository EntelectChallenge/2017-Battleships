using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BotRunner.Properties;
using BotRunner.Util;
using Domain.Bot;
using Domain.Meta;

namespace TestHarness.TestHarnesses.Bot.Runners
{
    public class PythonRunner : BotRunner
    {
        private readonly EnvironmentSettings _environmentSettings;

        public PythonRunner(BotHarness parentHarness, EnvironmentSettings environmentSettings)
            : base(parentHarness)
        {
            _environmentSettings = environmentSettings;
        }

        protected override ProcessHandler CreateProcessHandler()
        {
            var pythonExecutable = _environmentSettings.PathToPython3;
            if (ParentHarness.BotMeta.BotType == BotMeta.BotTypes.Python2)
            {
                pythonExecutable = _environmentSettings.PathToPython2;
            }

            var processArgs = GetProcessArguments(ParentHarness.BotMeta.RunFile, ParentHarness.BattleshipPlayer.Key, ParentHarness.CurrentWorkingDirectory);
            processArgs = AddAdditionalRunArgs(processArgs);

            return new ProcessHandler(ParentHarness.BotDir, pythonExecutable, processArgs, ParentHarness.Logger);
        }

        protected override void RunCalibrationTest()
        {
            var calibrationBot = _environmentSettings.CalibrationPathToPython3;
            var pythonExecutable = _environmentSettings.PathToPython3;
            if (ParentHarness.BotMeta.BotType == BotMeta.BotTypes.Python2)
            {
                calibrationBot = _environmentSettings.CalibrationPathToPython2;
                pythonExecutable = _environmentSettings.PathToPython2;
            }

            var processArgs = GetProcessArguments(calibrationBot, ParentHarness.BattleshipPlayer.Key, ParentHarness.CurrentWorkingDirectory);

            using (var handler = new ProcessHandler(AppDomain.CurrentDomain.BaseDirectory, pythonExecutable, processArgs, ParentHarness.Logger))
            {
                handler.RunProcess();
            }
        }


        private static string GetProcessArguments(string scriptFilePath, char playerKey, string workingDirectory)
        {
            return String.Format("\"{0}\" {1} \"{2}\"", scriptFilePath, playerKey, workingDirectory);
        }
    }
}
