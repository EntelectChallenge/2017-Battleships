using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReplayZ.Models
{
    public class Player
    {
        [JsonProperty]
        public int FailedFirstPhaseCommands { get; set; }
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public int Points { get; set; }
        [JsonProperty]
        public int Energy { get; set; }
        [JsonProperty]
        public bool Killed { get; set; }
        [JsonProperty]
        public bool IsWinner { get; set; }
        [JsonProperty]
        public int ShotsFired { get; set; }
        [JsonProperty]
        public int ShotsHit { get; set; }
        [JsonProperty]
        public int FirstShotLanded { get; set; }
    }
}
