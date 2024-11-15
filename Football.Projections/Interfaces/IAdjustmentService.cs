using Football.Models;
using Football.Projections.Models;

namespace Football.Projections.Interfaces
{
    public interface IAdjustmentService
    {
        Task<IEnumerable<SeasonProjection>> AdjustmentEngine(IEnumerable<SeasonProjection> seasonProjections, Tunings tunings, int seasonGames, SeasonAdjustments adjustments);
        Task<IEnumerable<AdjustmentDescription>> GetAdjustmentDescriptions();

    }
}
