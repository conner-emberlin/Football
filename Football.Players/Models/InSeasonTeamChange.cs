namespace Football.Players.Models
{
    public class InSeasonTeamChange
    {
        public int Season { get; set; }
        public int PlayerId { get; set; }
        public string PreviousTeam { get; set; } = "";
        public string NewTeam { get; set; } = "";
        public int WeekEffective { get; set; }

    }
}
