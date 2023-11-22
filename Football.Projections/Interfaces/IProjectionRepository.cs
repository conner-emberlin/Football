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
    }
}
