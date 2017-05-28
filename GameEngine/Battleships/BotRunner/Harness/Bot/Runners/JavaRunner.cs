using System;
using System.IO;
using BotRunner.Properties;
using BotRunner.Util;
using Domain.Bot;

namespace TestHarness.TestHarnesses.Bot.Runners
{
    public class JavaRunner : BotRunner
    {
        private readonly EnvironmentSettings _environmentSettings;

        public JavaRunner(BotHarness parentHarness, EnvironmentSettings environmentSettings) : base(parentHarness)
        {
            _environmentSettings = environmentSettings;
        }

        protected override ProcessHandler CreateProcessHandler()
        {
            var processArgs = GetProcessArguments(ParentHarness.BotMeta.RunFile, ParentHarness.BattleshipPlayer.Key, ParentHarness.CurrentWorkingDirectory);
            processArgs = AddAdditionalRunArgs(processArgs);

            return new ProcessHandler(ParentHarness.BotDir, _environmentSettings.PathToJava, processArgs, ParentHarness.Logger);
        }

        protected override void RunCalibrationTest()
        {
            var calibrationJar = _environmentSettings.CalibrationPathToJava;
            if (ParentHarness.BotMeta.BotType == BotMeta.BotTypes.Scala)
            {
                calibrationJar = _environmentSettings.CalibrationPathToScala;
            }
            var processArgs = GetProcessArguments(calibrationJar, ParentHarness.BattleshipPlayer.Key, ParentHarness.CurrentWorkingDirectory);

            using (var handler = new ProcessHandler(AppDomain.CurrentDomain.BaseDirectory, _environmentSettings.PathToJava, processArgs, ParentHarness.Logger))
            {
                handler.RunProcess();
            }
        }


        private static string GetProcessArguments(string jarFilePath, char playerKey, string workingDirectory)
        {
            return String.Format("-Xms512m -jar \"{0}\" {1} \"{2}\"", jarFilePath, playerKey, workingDirectory);
        }
    }
}
