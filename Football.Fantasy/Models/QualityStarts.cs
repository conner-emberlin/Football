
namespace Football.Fantasy.Models
{
    public class QualityStarts
    {
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public string Position { get; set; } = "";
        public int Season { get; set; }
        public int Week { get; set; }
        public int GamesPlayed { get; set; }
        public int PoorStarts { get; set; }
        public int GoodStarts { get; set; }
        public int GreatStarts { get; set; }
        
    }
}
