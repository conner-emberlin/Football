namespace Football.Projections.Models
{
    public class ConsensusProjections
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public string Position { get; set; } = "";
        public double FantasyPoints { get; set; }
    }
}
