using Football.Enums;
using Football.Projections.Models;


namespace Football.Projections.Interfaces
{
    public interface IProjectionRepository
    {
        public Task<int> PostSeasonProjections(List<SeasonProjection> projections);
        public Task<int> PostWeeklyProjections(List<WeekProjection> projection);
        public Task<SeasonProjection?> GetSeasonProjection(int playerId);
        public IEnumerable<WeekProjection> GetWeeklyProjectionsFromSQL(PositionEnum position, int week);
    }
}
