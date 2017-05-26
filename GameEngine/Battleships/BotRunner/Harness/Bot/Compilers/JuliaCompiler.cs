using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BotRunner.Properties;
using BotRunner.Util;
using Domain.Bot;
using Domain.Meta;
using GameEngine.Loggers;

namespace TestHarness.TestHarnesses.Bot.Compilers
{
    public class JuliaCompiler : ICompiler
    {
        private readonly BotMeta _botMeta;
        private readonly string _botDir;
        private readonly ILogger _compileLogger;
        private bool _failed;
		private readonly EnvironmentSettings _environmentSettings;

		public JuliaCompiler(BotMeta botMeta, string botDir, ILogger compileLogger, EnvironmentSettings environmentSettings)
        {
            _botMeta = botMeta;
            _botDir = botDir;
            _compileLogger = compileLogger;
			_environmentSettings = environmentSettings;
        }

        public bool HasPackageManager()
        {
            return false; // package manager is built in
        }

        public bool RunPackageManager()
        {
            return true; // the compile script can install packages if it must.
        }

        public bool RunCompiler()
        {
			// runs the bot with a --compile flag so it can warm up or precompile if needed

			var compileLocation = Path.Combine(_botDir, _botMeta.ProjectLocation ?? "");            
            _compileLogger.LogInfo("Compiling bot " + _botMeta.NickName + " using Julia");

			using (var handler = 
				new ProcessHandler(compileLocation, _environmentSettings.PathToJulia, " --precompiled=yes --compilecache=yes " +  _botMeta.RunFile + " --compile", _compileLogger)
			) {
				handler.ProcessToRun.ErrorDataReceived += ProcessDataRecieved;
				handler.ProcessToRun.OutputDataReceived += ProcessDataRecieved;

				_compileLogger.LogInfo("Compilation exited with: " + handler.RunProcess());

			}
            return true;
        }

        void ProcessDataRecieved(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            _compileLogger.LogInfo(e.Data);
        }
    }
}
