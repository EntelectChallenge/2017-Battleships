using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReferenceBot.Domain.Command;

namespace ReferenceBot.Domain.State
{
    public class BattleshipPlayer
    {
        [JsonProperty]
        public int FailedFirstRoundCommands { get; set; }
        [JsonProperty]
        public string Name { get; }
        
        [JsonProperty]
        public List<Ship> Ships { get; }
        
        [JsonProperty]
        public int Points { get; private set; }

        [JsonIgnore]
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
        public int ShipsRemaining { get; set; }

        [JsonProperty]
        public char Key { get; set; }

    }
}
