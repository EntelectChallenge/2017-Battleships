using System.Collections.Generic;
using Domain.Maps;
using Domain.Ships;

namespace Domain.JSON
{
    public class OpponentPlayerMap
    {
        public List<OpponentShip> Ships { get; set; }
        public bool Alive { get; set; }
        public int Points { get; set; }
        public string Name { get; set; }
        public List<OpponentCell> Cells { get; set; }
    }
}