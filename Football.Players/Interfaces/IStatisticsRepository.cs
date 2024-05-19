using Football.Players.Models;
using Football.Enums;

namespace Football.Players.Interfaces
{
    public interface IStatisticsRepository
    {
        public Task<List<WeeklyRosterPercent>> GetWeeklyRosterPercentages(int season, int week);
        public Task<List<GameResult>> GetGameResults(int season);
        public Task<List<T>> GetWeeklyData<T>(Position position, int season, int week);
        public Task<List<T>> GetWeeklyData<T>(Position position, int playerId);
        public Task<List<T>> GetWeeklyData<T>(Position position);
        public Task<List<T>> GetSeasonData<T>(Position position, int queryParam, bool isPlayer);
        public Task<List<SnapCount>> GetSnapCounts(int playerId);

    }
}
