using Football.Models;
using Football.Players.Models;
using Football.Projections.Models;

namespace Football.Projections.Interfaces
{
    public interface IAdjustmentService
    {
        public Task<IEnumerable<SeasonProjection>> AdjustmentEngine(IEnumerable<SeasonProjection> seasonProjections, Tunings tunings, int seasonGames, SeasonAdjustments adjustments);
        public Task<List<WeekProjection>> AdjustmentEngine(List<WeekProjection> weekProjections, WeeklyTunings tunings);
        public Task<IEnumerable<AdjustmentDescription>> GetAdjustmentDescriptions();

    }
}
