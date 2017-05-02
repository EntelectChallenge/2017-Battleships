using System;
using System.IO;
using BotRunner.Properties;
using BotRunner.Util;

namespace TestHarness.TestHarnesses.Bot.Runners
{
    public class GolangRunner : BotRunner {
      public GolangRunner(BotHarness parentHarness) : base(parentHarness)
      {
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
	    /* Leaving this uncommented until Calibration test bots have been formalised */
            // var calibrationGo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            //     @"Calibrations" + Path.DirectorySeparatorChar + "BotCalibrationGo");
            // var processArgs = GetProcessArguments(calibrationGo, ParentHarness.BattleshipPlayer.Key, ParentHarness.CurrentWorkingDirectory);

            // using (var handler = new ProcessHandler(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.PathToGolang, processArgs, ParentHarness.Logger))
            // {
            //     handler.RunProcess();
            // }
        }
    }
}

