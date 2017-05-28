using System;
using System.Collections.Generic;
using System.IO;
using BotRunner.Util;
using Domain.Bot;
using Domain.Meta;

namespace TestHarness.TestHarnesses.Bot.Runners
{
    public class RustRunner : BotRunner
    {
        private readonly EnvironmentSettings _environmentSettings;

        public RustRunner(BotHarness parentHarness, EnvironmentSettings environmentSettings) : base(parentHarness)
        {
            _environmentSettings = environmentSettings;
        }

        protected override ProcessHandler CreateProcessHandler()
        {
            var botDir = ParentHarness.BotDir;
            var botFile = ParentHarness.BotMeta.RunFile; 
            var processName = Path.Combine(botDir, botFile);

            var processArgs = String.Format("{0} \"{1}\"", ParentHarness.BattleshipPlayer.Key,
                ParentHarness.CurrentWorkingDirectory);

            return new ProcessHandler(botDir, processName, processArgs, ParentHarness.Logger);
        }

        protected override void RunCalibrationTest()
        {
            var calibrationExe = _environmentSettings.CalibrationPathToRust;
            var processArgs =
                String.Format("{0} \"{1}\"", ParentHarness.BattleshipPlayer.Key, ParentHarness.CurrentWorkingDirectory);

			using (var handler = new ProcessHandler(AppDomain.CurrentDomain.BaseDirectory, calibrationExe, processArgs, ParentHarness.Logger))
            {
                handler.RunProcess();
            }
        }
    }
}
