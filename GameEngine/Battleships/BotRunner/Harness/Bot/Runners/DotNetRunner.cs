using System;
using System.Collections.Generic;
using System.IO;
using BotRunner.Util;
using Domain.Bot;
using Domain.Meta;

namespace TestHarness.TestHarnesses.Bot.Runners
{
    public class DotNetRunner : BotRunner
    {
        private readonly EnvironmentSettings _environmentSettings;

        public DotNetRunner(BotHarness parentHarness, EnvironmentSettings environmentSettings) : base(parentHarness)
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
            
            processArgs = AddAdditionalRunArgs(processArgs);

            return new ProcessHandler(botDir, ConvertProcessName(processName), ConvertProcessArgs(botFile, processArgs), ParentHarness.Logger, true);
        }

        protected override void RunCalibrationTest()
        {
            var calibrationExe = GetCalibrarionExe();
            var processArgs =
                String.Format("{0} \"{1}\"", ParentHarness.BattleshipPlayer.Key, ParentHarness.CurrentWorkingDirectory);

			using (var handler = new ProcessHandler(AppDomain.CurrentDomain.BaseDirectory, ConvertProcessName(calibrationExe), ConvertProcessArgs(calibrationExe, processArgs), ParentHarness.Logger, true))
            {
                handler.RunProcess();
            }
        }

        private string ConvertProcessName(string processName)
        {
            return Environment.OSVersion.Platform == PlatformID.Unix ? "mono" : processName;
        }

        private string ConvertProcessArgs(string processName, string args)
        {
            return Environment.OSVersion.Platform == PlatformID.Unix ? String.Format("\"{0}\" {1}", processName, args) : args;
        }

        private string GetCalibrarionExe()
        {
            switch (ParentHarness.BotMeta.BotType)
            {
                case BotMeta.BotTypes.CPlusPlus:
                    return _environmentSettings.CalibrationPathToCPlusPlus;
                case BotMeta.BotTypes.CSharp:
                default:
                    return _environmentSettings.CalibrationPathToCSharp;
            }
        }
    }
}
