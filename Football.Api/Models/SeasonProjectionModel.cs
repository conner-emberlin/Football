namespace Football.Api.Models
{
    public class SeasonProjectionModel
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public string Name { get; set; } = "";
        public string Position { get; set; } = "";
        public double ProjectedPoints { get; set; }
        public bool CanDelete { get; set; }
        public string Team { get; set; } = "";
        public double AvgerageGamesMissed { get; set; }
        public double AdjustedProjectedPoints { get; set; }
        public SeasonProjectionErrorModel? SeasonProjectionError { get; set; }
    }
}
