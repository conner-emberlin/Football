namespace Football.Shared.Models.Players
{
    public class ScheduleModel
    {
        public int Season { get; set; }
        public int TeamId { get; set; }
        public string Team { get; set; } = "";
        public int Week { get; set; }
        public int OpposingTeamId { get; set; }
        public string OpposingTeam { get; set; } = "";
    }
}
