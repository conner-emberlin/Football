using Football.Players.Models;

namespace Football.Fantasy.Models
{
    public class SnapCountAnalysis
    {
        public Player Player { get; set; } = new();
        public int Season { get; set; }
        public double TotalSnaps { get; set; }
        public double SnapsPerGame { get; set; }
        public double FantasyPointsPerSnap { get; set; }
        public double RushAttsPerSnap { get; set; }
        public double TargetsPerSnap { get; set; }
        public double Utilization { get; set; }
    }
}
