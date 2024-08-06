namespace Football.Fantasy.Models
{
    public class MatchupRanking
    {
        public int Season { get; set; }
        public int Week {  get; set; }
        public int TeamId { get; set; }
        public int GamesPlayed { get; set; }
        public string Position { get; set; } = "";
        public double PointsAllowed { get; set; }
        public double AvgPointsAllowed { get; set; }
    }
}
