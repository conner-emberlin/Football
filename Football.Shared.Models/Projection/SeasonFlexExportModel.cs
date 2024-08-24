namespace Football.Shared.Models.Projection
{
    public class SeasonFlexExportModel
    {
        public int Rank { get; set; }
        public string Name { get; set; } = "";
        public string Position { get; set; } = "";
        public double ProjectedPoints { get; set; }
    }
}
