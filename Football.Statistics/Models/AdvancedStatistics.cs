using Football.Players.Models;

namespace Football.Statistics.Models
{
    public class AdvancedQBStatistics
    {
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public double PasserRating { get; set; }
        public double YardsPerPlay { get; set; }
        public double FiveThirtyEightRating { get; set; }
    }

    public class StrengthOfSchedule
    {
        public TeamMap TeamMap { get; set; } = new();
        public int CurrentWeek { get; set; }
        public double Strength { get; set; }
    }
}
