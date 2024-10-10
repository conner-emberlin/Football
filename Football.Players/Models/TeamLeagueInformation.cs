namespace Football.Players.Models
{
    public class TeamLeagueInformation
    {
        public int TeamId { get; set; }
        public string Conference { get; set; } = "";
        public string Division { get; set; } = "";
    }
}
