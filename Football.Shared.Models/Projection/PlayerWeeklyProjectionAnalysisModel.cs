namespace Football.Shared.Models.Projection
{
    public class PlayerWeeklyProjectionAnalysisModel
    {
        public int Season { get; set; }
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public int ProjectionCount { get; set; }
        public double MSE { get; set; }
        public double RSquared { get; set; }
        public double MAE { get; set; }
        public double MAPE { get; set; }
        public double AvgError { get; set; }
    }
}
