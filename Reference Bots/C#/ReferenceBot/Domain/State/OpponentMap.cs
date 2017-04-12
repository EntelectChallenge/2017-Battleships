using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ReferenceBot.Domain.State
{
    public class OpponentMap
    {
        [JsonProperty]
        public bool Alive { get; set; }
        [JsonProperty]
        public int Points { get; set; }
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public List<OpponentShip> Ships { get; set; }
        [JsonProperty]
        public List<OpponentCell> Cells { get; set; }
    }
}
