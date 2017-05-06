using System;
using CommandLine;
using CommandLine.Text;

namespace Battleships
{
    public class Options
    {
        [OptionArray('b', "bot",
    HelpText = "Relative path to the folder containing the player bot.  More than one of this command can be used to add bots",
    DefaultValue = new string[] { })]
        public String[] BotFolders { get; set; }

        [Option('c', "console",
            HelpText = "The amount of console players to add to the game",
        DefaultValue = 0)]
        public int ConsolePlayers { get; set; }

        [Option("clog", DefaultValue = false,
            HelpText = "Enables console logging.")]
        public bool ConsoleLogger { get; set; }

        [Option("pretty", DefaultValue = false,
            HelpText = "Draws the game map to console for every round")]
        public bool Pretty { get; set; }

        [Option('l', "log", DefaultValue = "",
            HelpText = "Relative path where you want the match replay log files to be output (instead of the default Replays/{mapSeed}).")]
        public string Log { get; set; }

        [Option('s', "size", DefaultValue = null,
            HelpText = "The size to use for map generation")]
        public int? MapSize { get; set; }

        [Option("nolimit", DefaultValue = false,
            HelpText = "Removes the time limit for bot execution")]
        public bool NoLimit { get; set; }

        [Option("debug", DefaultValue = false,
            HelpText = "Halts the Game Engine when a bot writes to the error stream")]
        public bool DebugMode { get; set; }

        [Option('f', "forceRebuild", DefaultValue = false,
            HelpText = "Rebuilds Bots based on their compiler types")]
        public bool ForceRebuild { get; set; }
	
        [Option("tournamentMode", DefaultValue = false,
            HelpText = "Will prevent writing sensitive information until the end of the match")]
        public bool TournamentMode { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this);
        }
    }
}
