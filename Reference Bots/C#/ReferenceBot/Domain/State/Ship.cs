using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ReferenceBot.Domain.Command.Ship;

namespace ReferenceBot.Domain.State
{
    public class Ship
    {
        [JsonProperty]
        public List<Cell> Cells { get; set; }
        [JsonProperty]
        public bool Destroyed { get; set; }
        [JsonProperty]
        public bool Placed { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ShipType ShipType { get; set; }
        [JsonProperty]
        public List<Weapon> Weapons { get; set; }
    }
}
