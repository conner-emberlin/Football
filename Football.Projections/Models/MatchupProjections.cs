
namespace Football.Projections.Models
{
    public class MatchupProjections
    {
        public string LeagueName { get; set; } = "";
        public string TeamName { get; set; } = "";
        public List<WeekProjection> TeamProjections { get; set; } = new();
    }
}
