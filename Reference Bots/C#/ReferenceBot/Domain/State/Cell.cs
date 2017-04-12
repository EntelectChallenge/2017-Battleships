using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReferenceBot.Domain.Command;

namespace ReferenceBot.Domain.State
{
    public class Cell
    {
        [JsonProperty]
        public bool Occupied { get; set; }
        
        [JsonProperty]
        public bool Hit { get; set; }

        [JsonIgnore]
        public Point Point => new Point(X, Y);

        [JsonProperty]
        public int X { get; set; }

        [JsonProperty]
        public int Y { get; set; }
    }
}
