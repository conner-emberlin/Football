using Football.Data.Models;
using Football.Enums;
using Football.Statistics.Models;

namespace Football.Statistics.Interfaces
{
    public interface IStatisticsService
    {
        public Task<List<T>> GetWeeklyData<T>(PositionEnum position, int season, int week);
        public Task<List<T>> GetWeeklyData<T>(PositionEnum position, int playerId);
        public Task<List<T>> GetSeasonData<T>(PositionEnum position, int queryParam, bool isPlayer);
        public Task<List<GameResult>> GetGameResults(int season);
        public Task<List<TeamRecord>> GetTeamRecords(int season);
        public Task<List<WeeklyRosterPercent>> GetWeeklyRosterPercentages(int season, int week);

    }
}
