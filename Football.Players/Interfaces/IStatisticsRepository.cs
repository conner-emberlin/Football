using Football.Players.Models;
using Football.Enums;

namespace Football.Players.Interfaces
{
    public interface IStatisticsRepository
    {
        public Task<List<WeeklyRosterPercent>> GetWeeklyRosterPercentages(int season, int week);
        public Task<List<GameResult>> GetGameResults(int season);
        public Task<List<T>> GetWeeklyData<T>(Position position, int season, int week);
        public Task<List<T>> GetWeeklyDataByPlayer<T>(Position position, int playerId, int season);
        public Task<List<T>> GetAllWeeklyDataByPosition<T>(Position position);
        public Task<List<T>> GetAllSeasonDataByPosition<T>(Position position);
        public Task<List<T>> GetSeasonData<T>(Position position, int queryParam, bool isPlayer);
        public Task<List<SnapCount>> GetSnapCounts(int playerId, int season);
        public Task<double> GetSnapsByGame(int playerId, int season, int week);
        public Task<List<T>> GetSeasonDataByTeamIdAndPosition<T>(int teamId, Position position, int season);
        public Task<IEnumerable<StarterMissedGames>> GetCurrentStartersThatMissedGamesLastSeason(int currentSeason, int previousSeason, int maxGames, double avgProjection);
        public Task<double> GetYearsExperience(int playerId, Position position);

    }
}
