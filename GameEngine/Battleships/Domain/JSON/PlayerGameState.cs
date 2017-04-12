using Domain.Maps;
using Newtonsoft.Json;

namespace Domain.JSON
{
    public class PlayerGameState
    {
        [JsonProperty]
        public PlayerMap PlayerMap { get; set; }

        [JsonProperty]
        public OpponentPlayerMap OpponentMap { get; set; }

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

        [JsonProperty]
        public PlayerMap Player1Map { get; set; }

        [JsonProperty]
        public PlayerMap Player2Map { get; set; }
    }
}