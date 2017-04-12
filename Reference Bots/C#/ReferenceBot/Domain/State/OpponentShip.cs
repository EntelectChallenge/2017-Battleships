using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReferenceBot.Domain.Command.Ship;

namespace ReferenceBot.Domain.State
{
    public class OpponentShip
    {
        [JsonProperty]
        public bool Destroyed { get; set; }
        [JsonProperty]
        public ShipType ShipType { get; set; }
    }
}
