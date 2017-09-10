using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Properties;
using Newtonsoft.Json;

namespace Domain.Abilities
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

        public Shield(int mapSize)
        {
            this.ChargeTime = 7;
            this.RoundLastUsed = 0;
            this.CurrentCharges = 0;
            this.CurrentRadius = 0;
            if (mapSize == Settings.Default.SmallMapSize)
            {
                this.MaxRadius = Settings.Default.MaxShieldRadiusSmall;
            }
            else if (mapSize == Settings.Default.MediumMapSize)
            {
                this.MaxRadius = Settings.Default.MaxShieldRadiusMedium;
            }
            else
            {
                this.MaxRadius = Settings.Default.MaxShieldRadiusLarge;
            }
        }

        public void GrowRadius()
        {
            if (CurrentRadius < MaxRadius)
            {
                CurrentRadius++;
            }
        }
    }
}