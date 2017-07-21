using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReplayZ.Models
{
    public class PlayerMap
    {
        [JsonProperty]
        public Player Owner { get; set; }
        [JsonProperty]
        public int MapWidth { get; set; }
        [JsonProperty]
        public int MapHeight { get; set; }
    }
}
