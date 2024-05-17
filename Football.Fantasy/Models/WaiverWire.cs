using Football.Players.Models;

namespace Football.Fantasy.Models
{
    public class WaiverWireCandidate
    {
        public Player Player { get; set; } = new();
        public PlayerTeam? PlayerTeam { get; set; } = new();
        public int Week { get; set; }
        public double RosteredPercentage { get; set; }
    }
}
