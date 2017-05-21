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
using System.Collections.Specialized;
using System.Collections;

namespace TestHarness.TestHarnesses.Bot.Compilers
{
    public class GolangCompiler : ICompiler
    {
        private readonly BotMeta _botMeta;
        private readonly string _botDir;
        private readonly ILogger _compileLogger;
        private readonly EnvironmentSettings _environmentSettings;

        public GolangCompiler(BotMeta botMeta, string botDir, ILogger compileLogger, EnvironmentSettings environmentSettings)
        {
            _botMeta = botMeta;
            _botDir = botDir;
            _compileLogger = compileLogger;
            _environmentSettings = environmentSettings;
        }

        public bool HasPackageManager()
        {
	    return false;  // "go get" is built in and as close as we're going to get to a package manager.
	}

        public bool RunPackageManager()
        {
            if (!HasPackageManager()) return true;

            _compileLogger.LogInfo("Running Go get");
            using (var handler = new ProcessHandler(_botDir, _environmentSettings.PathToGolang, "get .", _compileLogger))
            {
                handler.ProcessToRun.ErrorDataReceived += ProcessDataRecieved;
                handler.ProcessToRun.OutputDataReceived += ProcessDataRecieved;
                    
                return handler.RunProcess() == 0;
            }
        }

        public bool RunCompiler()
        {
            _compileLogger.LogInfo("Compiling bot " + _botMeta.NickName + " in location " + _botMeta.ProjectLocation + " using Golang");
	    var location = _botDir;
	    var envVars = new StringDictionary();
	    var target = _botMeta.RunFile;
	    envVars.Add("GOPATH", location);
	    using (var handler = new ProcessHandler(_botDir, _environmentSettings.PathToGolang, " build -v -o " + target +" -a "+  _botMeta.ProjectLocation, _compileLogger, false, envVars))
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
