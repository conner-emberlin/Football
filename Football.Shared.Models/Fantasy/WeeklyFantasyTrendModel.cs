
namespace Football.Shared.Models.Fantasy
{
    public class WeeklyFantasyTrendModel
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public int PositionalRank { get; set; }
        public double FantasyPoints { get; set; }
    }
}
