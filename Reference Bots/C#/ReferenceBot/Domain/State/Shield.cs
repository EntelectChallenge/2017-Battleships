using System.Drawing;
using Newtonsoft.Json;

namespace ReferenceBot.Domain.State
{
    public class Shield
    {
        [JsonProperty]
        public int ChargeTime { get; set; }

        [JsonProperty]
        public int RoundLastUsed { get; set; }

        [JsonProperty]
        public int CurrentCharges { get; set; }

        [JsonProperty]
        public bool Active { get; set; }

        [JsonProperty]
        public int CurrentRadius { get; set; }

        [JsonProperty]
        public int MaxRadius { get; set; }

        [JsonProperty]
        public Point CenterPoint { get; set; }
    }
}