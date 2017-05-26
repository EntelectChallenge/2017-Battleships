using System;
using System.IO;
using BotRunner.Properties;
using BotRunner.Util;

namespace TestHarness.TestHarnesses.Bot.Runners
{
    public class GolangRunner : BotRunner {
        private readonly EnvironmentSettings _environmentSettings;

        public GolangRunner(BotHarness parentHarness, EnvironmentSettings environmentSettings) : base(parentHarness)
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
          return new ProcessHandler(botDir, processName,  processArgs, ParentHarness.Logger);
      }

        protected override void RunCalibrationTest()
        {
	    /* Leaving this unimplemented until Calibration test bot methodology has been formalised */
        }
    }
}

