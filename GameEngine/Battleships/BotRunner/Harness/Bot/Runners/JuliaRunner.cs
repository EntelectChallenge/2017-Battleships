using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BotRunner.Properties;
using BotRunner.Util;

namespace TestHarness.TestHarnesses.Bot.Runners
{
    public class JuliaRunner : BotRunner
    {
        private readonly EnvironmentSettings _environmentSettings;

        public JuliaRunner(BotHarness parentHarness, EnvironmentSettings environmentSettings) : base(parentHarness)
        {
            _environmentSettings = environmentSettings;
        }

        protected override ProcessHandler CreateProcessHandler()
        {
			var processArgs = String.Format("--precompiled=yes --compilecache=no {0} {1} \"{2}\"", ParentHarness.BotMeta.RunFile, ParentHarness.BattleshipPlayer.Key,
                ParentHarness.CurrentWorkingDirectory);

            processArgs = AddAdditionalRunArgs(processArgs);

            return new ProcessHandler(ParentHarness.BotDir, _environmentSettings.PathToJulia, processArgs, ParentHarness.Logger);
        }

        protected override void RunCalibrationTest()
        {
            var calibrationExe = _environmentSettings.CalibrationPathToJulia;
            var processArgs = String.Format("--precompiled=yes --compilecache=no {0} {1} \"{2}\"", calibrationExe, ParentHarness.BattleshipPlayer.Key,
                ParentHarness.CurrentWorkingDirectory);

            using (var handler = new ProcessHandler(AppDomain.CurrentDomain.BaseDirectory, _environmentSettings.PathToJulia, processArgs, ParentHarness.Logger))
            {
                handler.RunProcess();
            }
        }
    }
}
