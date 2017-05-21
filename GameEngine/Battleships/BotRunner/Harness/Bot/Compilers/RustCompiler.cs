using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BotRunner.Properties;
using BotRunner.Util;
using Domain.Bot;
using Domain.Meta;
using GameEngine.Loggers;

namespace TestHarness.TestHarnesses.Bot.Compilers
{
    public class RustCompiler : ICompiler
    {
        private readonly BotMeta _botMeta;
        private readonly string _botDir;
        private readonly ILogger _compileLogger;
        private readonly EnvironmentSettings _environmentSettings;

        public RustCompiler(BotMeta botMeta, string botDir, ILogger compileLogger, EnvironmentSettings environmentSettings)
        {
            _botMeta = botMeta;
            _botDir = botDir;
            _compileLogger = compileLogger;
            _environmentSettings = environmentSettings;
        }

        public bool HasPackageManager()
        {
            return false;
        }

        public bool RunPackageManager()
        {
            return true;
        }

        public bool RunCompiler()
        {
            _compileLogger.LogInfo("Compiling bot " + _botMeta.NickName + " in location " + _botMeta.ProjectLocation + " using Rust");
            using (var handler = new ProcessHandler(Path.Combine(_botDir, _botMeta.ProjectLocation??""), _environmentSettings.PathToCargo, "build --release", _compileLogger))
            {
                handler.ProcessToRun.ErrorDataReceived += ProcessDataRecieved;
                handler.ProcessToRun.OutputDataReceived += ProcessDataRecieved;

                return handler.RunProcess() == 0;
            }
        }

        void ProcessDataRecieved(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            _compileLogger.LogInfo(e.Data);
        }
    }
}
