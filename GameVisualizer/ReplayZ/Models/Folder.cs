using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReplayZ.Models
{
    public class Folder
    {
        public string FolderName { get; set; }
        public int TotalFoldersInFolder { get; set; }
        public int TotalFilesInFolder { get; set; }
        public GameState GameState { get; set; }

        public long? Numeric1 {
            get
            {
                if (Numbers(FolderName).Any())
                    return Numbers(FolderName)[0];
                else
                    return null;
            }
        }
        public long? Numeric2 {
            get
            {
                if (Numbers(FolderName).Any() && Numbers(FolderName).Count > 1)
                    return Numbers(FolderName)[1];
                else
                    return null;
            }
        }
        public long? Numeric3
        {
            get
            {
                if (Numbers(FolderName).Any() && Numbers(FolderName).Count > 2)
                    return Numbers(FolderName)[2];
                else
                    return null;
            }
        }
        public long? Numeric4
        {
            get
            {
                if (Numbers(FolderName).Any() && Numbers(FolderName).Count > 3)
                    return Numbers(FolderName)[3];
                else
                    return null;
            }
        }
        public static List<long> Numbers(string str)
        {
            var nums = new List<long>();
            var start = -1;
            for (int i = 0; i < str.Length; i++)
            {
                if (start < 0 && Char.IsDigit(str[i]))
                {
                    start = i;
                }
                else if (start >= 0 && !Char.IsDigit(str[i]))
                {
                    nums.Add(long.Parse(str.Substring(start, i - start)));
                    start = -1;
                }
            }
            if (start >= 0)
                nums.Add(long.Parse(str.Substring(start, str.Length - start)));
            return nums;
        }
    }
}
