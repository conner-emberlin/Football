
namespace Football.Players.Models
{
    public class ConsensusWeeklyProjections
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public int Week { get; set; }
        public string Position { get; set; } = "";
        public double FantasyPoints { get; set; }
    }
}
