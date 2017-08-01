namespace Domain.JSON
{
    public class OpponentCell
    {
        public bool Damaged { get; set; }
        public bool Missed { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool ShieldHit { get; set; }
    }
}