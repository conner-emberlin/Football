namespace Football.Api.Models.Players
{
    public class InSeasonInjuryModel
    {
        public int InjuryId { get; set; }
        public int Season { get; set; }
        public int PlayerId { get; set; }
        public int InjuryStartWeek { get; set; }
        public int InjuryEndWeek { get; set; }
        public string Description { get; set; } = "";
        public double GamesPlayedInjured { get; set; }
    }
}
