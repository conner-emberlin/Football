using System.Text.Json.Serialization;

namespace Football.LeagueAnalysis.Models
{
    public class SleeperUser
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = "";

        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = "";

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } = "";

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; } = "";
    }
}
