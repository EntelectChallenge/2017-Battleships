using System;
using System.IO;
using BotRunner.Properties;
using Domain.Bot;
using Newtonsoft.Json;

namespace BotRunner.Util
{
    public class BotMetaReader
    {
        public static BotMeta ReadBotMeta(String botLocation)
        {
            var metaLocation = Path.Combine(botLocation, Settings.Default.BotMetaFileName);

            if (!File.Exists(metaLocation))
                throw new FileNotFoundException("No bot meta file found at location " + metaLocation);

            var fileContent = File.ReadAllText(metaLocation);

            var meta = JsonConvert.DeserializeObject<BotMeta>(fileContent);
            meta.ProjectLocation = meta.ProjectLocation.Replace(@"\\", Path.DirectorySeparatorChar.ToString()).Replace(@"\", Path.DirectorySeparatorChar.ToString());
            meta.RunFile = meta.RunFile.Replace(@"\\", Path.DirectorySeparatorChar.ToString()).Replace(@"\", Path.DirectorySeparatorChar.ToString());
            return meta;
        }
    }
}