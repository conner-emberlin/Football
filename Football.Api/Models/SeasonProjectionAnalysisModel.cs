namespace Football.Api.Models
{
    public class SeasonProjectionAnalysisModel
    {
        public int Season { get; set; }
        public string Position { get; set; } = "";
        public double MSE { get; set; }
        public double RSquared { get; set; }
        public double AvgError { get; set; }
        public double AvgErrorPerGame { get; set; }
        public double ProjectionCount { get; set; }
    }
}
