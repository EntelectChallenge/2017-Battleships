using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.File
{
    public class FileHelper
    {
        public static Boolean WriteImmediate { get; set; }
        private static readonly Dictionary<String, String> FileContentCache = new Dictionary<string, string>();

        public static void WriteAllText(String location, String content)
        {
            if (WriteImmediate)
            {
                WriteToFile(location, content);
            }
            else
            {
                FileContentCache.Add(location, content);
            }
        }

        private static void WriteToFile(string location, string content)
        {
            var dirLocation = Path.GetDirectoryName(location);
            if (!Directory.Exists(dirLocation))
                Directory.CreateDirectory(dirLocation);

            System.IO.File.WriteAllText(location, content, new UTF8Encoding(false));
        }

        public static void FlushCache()
        {
            foreach (var content in FileContentCache)
            {
                WriteToFile(content.Key, content.Value);
            }   
            FileContentCache.Clear();
        }

    }
}
