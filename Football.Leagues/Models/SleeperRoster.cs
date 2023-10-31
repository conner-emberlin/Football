using System.Text.Json.Serialization;

namespace Football.Leagues.Models
{

    public class SleeperRoster
    {
        [JsonPropertyName("taxi")]
        public object Taxi { get; set; }

        [JsonPropertyName("starters")]
        public List<string> Starters { get; set; }

        [JsonPropertyName("settings")]
        public RosterSettings Settings { get; set; }

        [JsonPropertyName("roster_id")]
        public int RosterId { get; set; }

        [JsonPropertyName("reserve")]
        public List<string> Reserve { get; set; }

        [JsonPropertyName("players")]
        public List<string> Players { get; set; }

        [JsonPropertyName("player_map")]
        public object PlayerMap { get; set; }

        [JsonPropertyName("owner_id")]
        public string OwnerId { get; set; }

        [JsonPropertyName("metadata")]
        public RosterMetadata Metadata { get; set; }

        [JsonPropertyName("league_id")]
        public string LeagueId { get; set; }

        [JsonPropertyName("keepers")]
        public object Keepers { get; set; }

        [JsonPropertyName("co_owners")]
        public object CoOwners { get; set; }
    }
    public class RosterMetadata
    {
        [JsonPropertyName("streak")]
        public string Streak { get; set; }

        [JsonPropertyName("restrict_pn_scoring_starters_only")]
        public string RestrictPnScoringStartersOnly { get; set; }

        [JsonPropertyName("record")]
        public string Record { get; set; }

        [JsonPropertyName("allow_pn_scoring")]
        public string AllowPnScoring { get; set; }

        [JsonPropertyName("allow_pn_player_injury_status")]
        public string AllowPnPlayerInjuryStatus { get; set; }

        [JsonPropertyName("allow_pn_inactive_starters")]
        public string AllowPnInactiveStarters { get; set; }
    }

    public class RosterSettings
    {
        [JsonPropertyName("wins")]
        public int Wins { get; set; }

        [JsonPropertyName("waiver_position")]
        public int WaiverPosition { get; set; }

        [JsonPropertyName("waiver_budget_used")]
        public int WaiverBudgetUsed { get; set; }

        [JsonPropertyName("total_moves")]
        public int TotalMoves { get; set; }

        [JsonPropertyName("ties")]
        public int Ties { get; set; }

        [JsonPropertyName("ppts_decimal")]
        public int PptsDecimal { get; set; }

        [JsonPropertyName("ppts")]
        public int Ppts { get; set; }

        [JsonPropertyName("losses")]
        public int Losses { get; set; }

        [JsonPropertyName("fpts_decimal")]
        public int FptsDecimal { get; set; }

        [JsonPropertyName("fpts_against_decimal")]
        public int FptsAgainstDecimal { get; set; }

        [JsonPropertyName("fpts_against")]
        public int FptsAgainst { get; set; }

        [JsonPropertyName("fpts")]
        public int Fpts { get; set; }
    }


}
