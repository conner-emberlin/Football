using Football.Players.Models;

namespace Football.Fantasy.Models
{
    public class BoomBust
    {
        public int Season { get; set; }
        public Player Player { get; set; } = new();
        public double BoomPercentage { get; set; }
        public double BustPercentage { get; set; }
    }

    public class BoomBustByWeek
    {
        public int Season { get; set; }
        public Player Player { get; set; }
        public int Week { get; set; }
        public bool Boom { get; set; }
        public bool Bust { get; set; }
        public double FantasyPoints { get; set; }
    }
}
