using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Games;

namespace Domain.JSON
{
    public class OpponentShip
    {
        public bool Destroyed { get; set; }
        public ShipType ShipType { get; set; }
    }
}
