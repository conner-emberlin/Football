using Football.Enums;
using Football.Projections.Models;

namespace Football.Projections.Interfaces
{
    public interface IProjectionRepository
    {
        public Task<int> PostSeasonProjections(List<SeasonProjection> projections);
        public Task<int> PostWeeklyProjections(List<WeekProjection> projection);
        public Task<IEnumerable<SeasonProjection>?> GetSeasonProjection(int playerId);
        public Task<IEnumerable<WeekProjection>?> GetWeeklyProjection(int playerId);
        public IEnumerable<WeekProjection> GetWeeklyProjectionsFromSQL(Position position, int week);
        public IEnumerable<SeasonProjection> GetSeasonProjectionsFromSQL(Position position, int season);
        public Task<bool> DeleteWeeklyProjection(int playerId, int week, int season);
        public Task<bool> DeleteSeasonProjection(int playerId, int season);
        public Task<bool> PostSeasonProjectionConfiguration(SeasonProjectionConfiguration config);
        public Task<bool> PostWeeklyProjectionConfiguration(WeeklyProjectionConfiguration config);
        public Task<string?> GetCurrentSeasonProjectionFilter(string position, int season);
        public Task<string?> GetCurrentWeekProjectionFilter(string position, int week, int season);
    }
}
