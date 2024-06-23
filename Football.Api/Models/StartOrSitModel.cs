using Football.Fantasy.Models;

namespace Football.Api.Models
{
    public class StartOrSitModel
    {
        public string Name { get; set; } = "";
        public string Position { get; set; } = "";
        public string Team { get; set; } = "";
        public int TeamId { get; set; }
        public string AtIndicator { get; set; } = "";
        public int OpponentTeamId { get; set; }
        public string OpponentTeam { get; set; } = "";
        public int MatchupRanking { get; set; }
        public double OverUnder { get; set; }
        public double Line { get; set; }
        public double ImpliedTeamTotal { get; set; }
        public string GameTime { get; set; } = "";
        public string Temperature { get; set; } = "";
        public string Wind { get; set; } = "";
        public string Condition { get; set; } = "";
        public string ConditionURL { get; set; } = "";
        public double ProjectedPoints { get; set; }
        public List<PlayerComparisonModel> PlayerComparisons { get; set; } = new();
        public bool Start { get; set; }
    }
}
