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
        public IEnumerable<WeekProjection> GetWeeklyProjectionsFromSQL(PositionEnum position, int week);
        public IEnumerable<SeasonProjection> GetSeasonProjectionsFromSQL(PositionEnum position, int season);
    }
}
