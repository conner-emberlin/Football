namespace Football.Shared.Models.Fantasy
{
    public class MatchupRankingModel
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public int TeamId { get; set; }
        public int Ranking { get; set; }
        public int GamesPlayed { get; set; }
        public string Position { get; set; } = "";
        public double PointsAllowed { get; set; }
        public double AvgPointsAllowed { get; set; }
        public string TeamDescription { get; set; } = "";

    }
}
