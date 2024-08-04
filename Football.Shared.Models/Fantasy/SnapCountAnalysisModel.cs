
namespace Football.Shared.Models.Fantasy
{
    public class SnapCountAnalysisModel
    {
        public string Name { get; set; } = "";
        public string Position { get; set; } = "";
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public double TotalSnaps { get; set; }
        public double SnapsPerGame { get; set; }
        public double FantasyPointsPerSnap { get; set; }
        public double RushAttsPerSnap { get; set; }
        public double TargetsPerSnap { get; set; }
        public double Utilization { get; set; }
    }
}
