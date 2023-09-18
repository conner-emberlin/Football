namespace Football.Data.Models
{
    public class WeeklyScraping
    {
        public string FantasyProsBaseURL { get; set; } = "";
        public string NFLTeamLogoURL { get; set; } = "";
        public string FantasyProsXPath { get; set; } = "";
        public int CurrentWeek { get; set; }
    }
}
