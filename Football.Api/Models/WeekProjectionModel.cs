namespace Football.Api.Models
{
    public class WeekProjectionModel
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public int Week { get; set; }
        public string Name { get; set; } = "";
        public string Position { get; set; } = "";
        public double ProjectedPoints { get; set; }
        public bool CanDelete { get; set; }
        public string Team { get; set; } = "";

    }
}
