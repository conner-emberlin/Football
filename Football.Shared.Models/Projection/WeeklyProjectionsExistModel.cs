namespace Football.Shared.Models.Projection
{
    public class WeeklyProjectionsExistModel
    {
        public bool ProjectionExist { get; set; }
        public List<string> Filters { get; set; } = [];
    }
}
