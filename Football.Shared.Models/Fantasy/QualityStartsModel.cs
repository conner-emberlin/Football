
namespace Football.Shared.Models.Fantasy
{
    public class QualityStartsModel
    {
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public string Team { get; set; } = "";
        public string Position { get; set; } = "";
        public int Season { get; set; }
        public int Week { get; set; }
        public int GamesPlayed { get; set; }
        public int PoorStarts { get; set; }
        public double PoorStartPercentage { get; set; }
        public int GoodStarts { get; set; }
        public double GoodStartPercentage { get; set; }
        public int GreatStarts { get; set; }
        public double GreatStartPercentage { get; set; }
        public int QualityCount { get; set; }
        public double QualityPercentage { get; set; }
    }
}
