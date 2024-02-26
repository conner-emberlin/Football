using Football.Players.Models;

namespace Football.Fantasy.Analysis.Models
{
    public class SeasonProjectionAnalysis
    {
        public Player Player { get; set; } = new();
        public double TotalFantasy { get; set; }
        public int WeeksPlayed { get; set; }
        public double SeasonFantasyProjection { get; set; }        
    }
}
