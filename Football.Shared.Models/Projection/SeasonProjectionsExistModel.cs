
namespace Football.Shared.Models.Projection
{
    public class SeasonProjectionsExistModel
    {
        public bool ProjectionExist { get; set; }
        public List<string> Filters { get; set; } = [];
    }
}
