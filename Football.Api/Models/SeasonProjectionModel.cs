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
    }
}
