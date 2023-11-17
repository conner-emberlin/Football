using Football.Fantasy.Models;
using Football.Players.Models;

namespace Football.Fantasy.Analysis.Models
{
    public class StartOrSit
    {
        public Player Player { get; set; } = new();
        public TeamMap? TeamMap { get; set; }
        public ScheduleDetails? ScheduleDetails { get; set; }
        public MatchLines? MatchLines { get; set; }
        public Weather? Weather { get; set; }
        public int MatchupRanking { get; set; }
        public double ProjectedPoints { get; set; }
        public List<PlayerComparison>? PlayerComparisons { get; set; }
        public bool Start { get; set; }
    }

    public class MatchLines
    {
        public double? OverUnder { get; set; }
        public double? Line { get; set; }
        public double? ImpliedTeamTotal { get; set; }
    }

    public class Weather
    {
        public string GameTime { get; set; } = "";
        public string Temperature { get; set; } = "";
        public string Condition { get; set; } = "";
        public string ConditionURL { get; set; } = "";
        public string Wind { get; set; } = "";
        public string RainChance { get; set; } = "";
        public string SnowChance { get; set; } = "";
    }

    public class PlayerComparison
    {
        public Player Player { get; set; } = new();
        public string? Team { get; set; } 
        public Schedule? Schedule { get; set; }
        public WeeklyFantasy? WeeklyFantasy { get; set; }
        public double ProjectedPoints { get; set; }
    }
}
