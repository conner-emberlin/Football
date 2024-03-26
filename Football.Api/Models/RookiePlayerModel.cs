namespace Football.Api.Models
{
    public class RookiePlayerModel
    {
        public string Name { get; set; } = "";
        public string Position { get; set; } = "";
        public int Active { get; set; }
        public string TeamDrafted { get; set; } = "";
        public int RookieSeason { get; set; }
        public int DraftPosition { get; set; }
        public int DeclareAge { get; set; }
    }
}
