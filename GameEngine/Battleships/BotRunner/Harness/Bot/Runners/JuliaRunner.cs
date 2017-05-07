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
        public JuliaRunner(BotHarness parentHarness) : base(parentHarness)
        {
        }

        protected override ProcessHandler CreateProcessHandler()
        {
            var processArgs = String.Format("{0} {1} \"{2}\"", ParentHarness.BotMeta.RunFile, ParentHarness.BattleshipPlayer.Key,
                ParentHarness.CurrentWorkingDirectory);

            processArgs = AddAdditionalRunArgs(processArgs);

            return new ProcessHandler(ParentHarness.BotDir, Settings.Default.PathToJulia, processArgs, ParentHarness.Logger);
        }

        protected override void RunCalibrationTest()
        {
            /* Leaving this unimplemented until Calibration test bot methodology has been formalised */
        }
    }
}
