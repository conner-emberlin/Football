using Football.Models;

namespace Football.Models
{
    public class Rookie
    {
        public int PlayerId { get; set; }
        public string TeamDrafted { get; set; }
        public string Position { get; set; }
        public int RookieSeason { get; set; }
        public int DraftPosition { get; set; }
        public int DeclareAge { get; set; }
    }
}
