using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Football.LeagueAnalysis.Models
{
    public class SleeperMatchup
    {
        [JsonPropertyName("starters")]
        public List<string> Starters { get; set; }

        [JsonPropertyName("roster_id")]
        public int RosterId { get; set; }

        [JsonPropertyName("players")]
        public List<string> Players { get; set; }

        [JsonPropertyName("matchup_id")]
        public int MatchupId { get; set; }

        [JsonPropertyName("points")]
        public double Points { get; set; }

        [JsonPropertyName("custom_points")]
        public int CustomPoints { get; set; }
    }
}
