namespace Football.Models
{
    public class ProjectionModel
    {
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public string Team { get; set; }
        public string Position { get; set; }
        public double ProjectedPoints { get; set; }
    }
}
