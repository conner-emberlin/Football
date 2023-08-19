namespace Football.Models
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public string? Position { get; set; }
        public int Active { get; set; }
        public bool IsTightEnd { get; set; }
        public List<PassingStatisticWithSeason>? PassingStats { get; set; }
        public List<RushingStatisticWithSeason>? RushingStats { get; set; }
        public List<ReceivingStatisticWithSeason>? ReceivingStats { get; set; }
        public List<FantasyPoints>? FantasyPoints { get; set; }
    }
}
