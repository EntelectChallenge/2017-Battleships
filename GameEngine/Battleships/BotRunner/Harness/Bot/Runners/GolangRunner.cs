using System;
using System.IO;
using BotRunner.Properties;
using BotRunner.Util;

namespace TestHarness.TestHarnesses.Bot.Runners
{
    public class GolangRunner : BotRunner
    {
        public GolangRunner(BotHarness parentHarness) : base(parentHarness)
        {
        }

        protected override ProcessHandler CreateProcessHandler()
        {
            var processArgs = GetProcessArguments(ParentHarness.BotMeta.RunFile, ParentHarness.BattleshipPlayer.Key, ParentHarness.CurrentWorkingDirectory);
	    return new ProcessHandler(ParentHarness.BotDir, "go",  processArgs, ParentHarness.Logger, true);
        }

        protected override void RunCalibrationTest()
        {
            var calibrationJar = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                @"Calibrations" + Path.DirectorySeparatorChar + "BotCalibrationGolang.jar");
            var processArgs = GetProcessArguments(calibrationJar, ParentHarness.BattleshipPlayer.Key, ParentHarness.CurrentWorkingDirectory);

            // using (var handler = new ProcessHandler(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.PathToGolang, processArgs, ParentHarness.Logger))
            // {
            //     handler.RunProcess();
            // }
        }


        private static string GetProcessArguments(string exeFilePath, char playerKey, string workingDirectory)
        {
            return String.Format("run {0} {1} \"{2}\"", exeFilePath, playerKey, workingDirectory);
        }
    }
}
