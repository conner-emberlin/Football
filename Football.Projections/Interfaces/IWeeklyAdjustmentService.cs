
using Football.Models;
using Football.Projections.Models;

namespace Football.Projections.Interfaces
{
    public interface IWeeklyAdjustmentService
    {
        Task<List<WeekProjection>> AdjustmentEngine(List<WeekProjection> weekProjections, WeeklyTunings tunings);
    }
}
