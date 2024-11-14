namespace Football.Players.Models
{
    public class TeamDepthChart
    {
        public int TeamId { get; set; }
        public string Team { get; set; } = "";
        public string TeamDescription { get; set; } = "";
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public string Position { get; set; } = "";
        public double Games { get; set; }
        public double TotalSnaps { get; set; }
        public double SnapsPerGame { get; set; }
    }
}
