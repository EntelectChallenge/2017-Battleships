using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReplayZ.Models
{
    public class GameState
    {
        [JsonProperty]
        public string FolderName { get; set; }
        [JsonProperty]
        public PlayerMap Player1Map { get; set; }
        [JsonProperty]
        public PlayerMap Player2Map { get; set; }
    }
}
