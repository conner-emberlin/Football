namespace Football.Api.Models.Players
{
    public class PlayerInjuryModel
    {
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public string Position { get; set; } = "";
        public int InjuryId { get; set; }
        public int Season { get; set; }
        public int InjuryStartWeek { get; set; }
        public int InjuryEndWeek { get; set; }
        public string Description { get; set; } = "";
        public double GamesPlayedInjured { get; set; }
    }
}
