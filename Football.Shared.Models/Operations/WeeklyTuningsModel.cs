namespace Football.Shared.Models.Operations
{
    public class WeeklyTuningsModel
    {
        public bool PreviousSeasonWeeklyTunings { get; set; }
        public double RecentWeekWeight { get; set; }
        public double ProjectionWeight { get; set; }
        public double TamperedMin { get; set; }
        public double TamperedMax { get; set; }
        public double MinWeekWeighted { get; set; }
        public int RecentWeeks { get; set; }
        public int ErrorAdjustmentWeek { get; set; }
        public int QBProjectionCount { get; set; }
        public int RBProjectionCount { get; set; }
        public int WRProjectionCount { get; set; }
        public int TEProjectionCount { get; set; }
        public int MinWeekMatchupAdjustment { get; set; }
    }
}
