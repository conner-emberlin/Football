namespace Football.Shared.Models.Teams
{
    public class TeamLeagueInformationModel
    {
        public int TeamId { get; set; }
        public string Conference { get; set; } = "";
        public string Division { get; set; } = "";
    }
}
