using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Properties;

namespace Domain.Abilities
{
    public class Shield
    {
        public int ChargeTime { get; set; }
        public int RoundLastUsed { get; set; }
        public int CurrentCharges { get; set; }
        public bool Active { get; set; }
        public int CurrentRadius { get; set; }
        public int MaxRadius { get; set; }
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