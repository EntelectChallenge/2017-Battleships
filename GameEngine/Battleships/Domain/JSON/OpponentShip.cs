using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Games;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Domain.JSON
{
    public class OpponentShip
    {
        public bool Destroyed { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ShipType ShipType { get; set; }
    }
}
