using Football.Players.Models;
using Football.Projections.Models;

namespace Football.Projections.Interfaces
{
    public interface IAdjustmentService
    {
        public Task<IEnumerable<SeasonProjection>> AdjustmentEngine(IEnumerable<SeasonProjection> seasonProjections);
        public Task<List<WeekProjection>> AdjustmentEngine(List<WeekProjection> weekProjections);

        public Task<IEnumerable<QuarterbackChange>> GetQuarterbackChanges();
    }
}
