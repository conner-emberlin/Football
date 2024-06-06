namespace Football.Players.Models
{
    public class Rookie
    {
        public int PlayerId { get; set; }
        public string TeamDrafted { get; set; } = "";
        public string Position { get; set; } = "";
        public int RookieSeason { get; set; }
        public int DraftPick { get; set; }
        public int DeclareAge { get; set; } 
        public int TeamId { get; set; }
    }
}
