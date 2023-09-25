using Football.Players.Models;

namespace Football.Fantasy.Models
{
    public class MatchupRanking
    {
        public TeamMap Team { get; set; } = new TeamMap();  
        public int GamesPlayed { get; set; }
        public string Position { get; set; } = "";
        public double PointsAllowed { get; set; }
    }
}
