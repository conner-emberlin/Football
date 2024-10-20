namespace Football.Shared.Models.Players
{
    public class InSeasonTeamChangeModel
    {
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public string PreviousTeam { get; set; } = "";
        public string NewTeam { get; set; } = "";
        public int WeekEffective { get; set; }
    }
}
