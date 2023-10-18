namespace Football.Players.Models
{
    public class InSeasonInjury
    {
        public int Season { get; set; }
        public int PlayerId { get; set; }
        public int InjuryStartWeek { get; set; }
        public int InjuryEndWeek { get; set; }
        public string Description { get; set; } = "";
    }
}
