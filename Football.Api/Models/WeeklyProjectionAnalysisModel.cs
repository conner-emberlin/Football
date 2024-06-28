namespace Football.Api.Models
{
    public class WeeklyProjectionAnalysisModel
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public string Position { get; set; } = "";
        public double MSE { get; set; }
        public double RSquared { get; set; }
        public double MAE { get; set; }
        public double MAPE { get; set; }
        public double AvgError { get; set; }
        public double AvgRankError { get; set; }
        public double AdjAvgError { get; set; }
    }
}
