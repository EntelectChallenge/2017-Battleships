using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ReferenceBot.Domain.State
{
    public class GameState
    {
        [JsonProperty]
        public PlayerMap PlayerMap { get; set; }
        [JsonProperty]
        public OpponentMap OpponentMap { get; set; }
        [JsonProperty]
        public string GameVersion { get; set; }
        [JsonProperty]
        public int GameLevel { get; set; }
        [JsonProperty]
        public int Round { get; set; }
        [JsonProperty]
        public int MapDimension { get; set; }
        [JsonProperty]
        public int Phase { get; set; }
    }
}
