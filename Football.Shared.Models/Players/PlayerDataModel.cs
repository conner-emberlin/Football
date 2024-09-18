using Football.Shared.Models.Fantasy;

namespace Football.Shared.Models.Players
{
    public class PlayerDataModel
    {
        public int PlayerId { get; set; }
        public string Position { get; set; } = "";
        public string Name { get; set; } = "";
        public List<SeasonDataModel> SeasonData { get; set; } = [];
        public List<WeeklyDataModel> WeeklyData { get; set; } = [];
        public bool WeeklyDataFromPastSeason { get; set; }
        public List<SeasonFantasyModel> SeasonFantasy { get; set; } = [];
        public int OverallRank { get; set; }
        public int PositionRank { get; set; }
        public double RunningFantasyTotal { get; set; }
        public List<WeeklyFantasyModel> WeeklyFantasy { get; set; } = [];
        public string Team { get; set; } = "";
        public List<ScheduleModel> Schedule { get; set; } = [];
        public List<WeeklyFantasyTrendModel> WeeklyFantasyTrends { get; set; } = [];
    }
}
