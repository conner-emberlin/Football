namespace Football.Api.Models.Operations
{
    public class WeeklyTuningsModel
    {
        public double RecentWeekWeight { get; set; }
        public double ProjectionWeight { get; set; }
        public double TamperedMin { get; set; }
        public double TamperedMax { get; set; }
        public double MinWeekWeighted { get; set; }
        public int RecentWeeks { get; set; }
    }
}
