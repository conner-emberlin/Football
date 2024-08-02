namespace Football.Api.Models.Projection
{
    public class MatchupProjectionsModel
    {
        public string LeagueName { get; set; } = "";
        public string TeamName { get; set; } = "";
        public List<WeekProjectionModel> TeamProjections { get; set; } = [];
    }
}
