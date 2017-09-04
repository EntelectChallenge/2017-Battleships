using System.Drawing;

namespace Domain.JSON
{
    public class Player1Move
    {
        public bool SuccessfulMove { get; set; }
        public string CommandIssued { get; set; }
        public Point CenterPoint { get; set; }
        public int ShieldRadius { get; set; }
        public string PlaceShipString { get; set; }
        public int Energy { get; set; }
        public int RemainingShieldTurns { get; set; }
    }
}