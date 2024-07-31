using Football.Players.Models;

namespace Football.Api.Models.Projection
{
    public class SeasonProjectionErrorModel
    {
        public int PlayerId { get; set; }
        public double TotalFantasy { get; set; }
        public int WeeksPlayed { get; set; }
        public double SeasonFantasyProjection { get; set; }
    }
}
