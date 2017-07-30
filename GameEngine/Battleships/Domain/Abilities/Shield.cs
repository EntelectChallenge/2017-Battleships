using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abilities
{
    public class Shield
    {
        public int ChargeTime { get; set; }
        public int RoundLastUsed { get; set; }
        public int CurrentCharges { get; set; }
        public bool Active { get; set; }

        public Shield()
        {
            this.ChargeTime = 5;
            this.RoundLastUsed = 0;
            this.CurrentCharges = 0;
        }
    }
}