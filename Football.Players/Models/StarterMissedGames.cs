namespace Football.Players.Models
{
    public class StarterMissedGames
    {
        public int PlayerId { get; set; }
        public int PreviousSeasonGames { get; set; }
        public int PreviousSeasonTeamId { get; set; }
        public double CurrentSeasonProjection { get; set; }
    }
}
