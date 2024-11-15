using Football.Enums;
using Football.Projections.Models;

namespace Football.Projections.Interfaces
{
    public interface IProjectionRepository
    {
        Task<int> PostSeasonProjections(List<SeasonProjection> projections);
        Task<int> PostWeeklyProjections(List<WeekProjection> projection);
        Task<IEnumerable<SeasonProjection>?> GetSeasonProjection(int playerId);
        Task<IEnumerable<WeekProjection>?> GetWeeklyProjection(int playerId);
        IEnumerable<WeekProjection> GetWeeklyProjectionsFromSQL(Position position, int week);
        IEnumerable<SeasonProjection> GetSeasonProjectionsFromSQL(Position position, int season);
        Task<bool> DeleteWeeklyProjection(int playerId, int week, int season);
        Task<bool> DeleteSeasonProjection(int playerId, int season);
        Task<bool> PostSeasonProjectionConfiguration(SeasonProjectionConfiguration config);
        Task<bool> PostWeeklyProjectionConfiguration(WeeklyProjectionConfiguration config);
        Task<string?> GetCurrentSeasonProjectionFilter(string position, int season);
        Task<string?> GetCurrentWeekProjectionFilter(string position, int week, int season);
        Task<IEnumerable<AdjustmentDescription>> GetAdjustmentDescriptions();
    }
}
