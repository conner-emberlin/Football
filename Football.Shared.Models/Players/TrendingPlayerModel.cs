namespace Football.Shared.Models.Players
{
    public class TrendingPlayerModel
    {
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public string Team { get; set; } = "";
        public string Position { get; set; } = "";
        public int Adds { get; set; }

    }
}
