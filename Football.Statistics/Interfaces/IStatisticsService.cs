using Football.Data.Models;
using Football.Enums;

namespace Football.Statistics.Interfaces
{
    public interface IStatisticsService
    {
        public Task<List<T>> GetWeeklyData<T>(PositionEnum position, int season, int week);
        public Task<List<T>> GetWeeklyData<T>(PositionEnum position, int playerId);
        public Task<List<T>> GetSeasonData<T>(PositionEnum position, int queryParam, bool isPlayer);
        public Task<List<GameResult>> GetGameResults(int season, int week);
        public Task<List<WeeklyRosterPercent>> GetWeeklyRosterPercentages(int season, int week);

    }
}
