using System;

namespace Domain.Meta
{
    public class BotaMeta
    {
        public string Author { get; set; }
        public string Email { get; set; }
        public string NickName { get; set; }
        public BotTypes BotType { get; set; }
        public string ProjectLocation { get; set; }
        public string RunFile { get; set; }
        public string RunArgs { get; set; }

        public enum BotTypes
        {
            CSharp,
            Java,
            CPlusPlus
        }
    }
}