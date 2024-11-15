using Football.Players.Models;
using Football.Enums;

namespace Football.Players.Interfaces
{
    public interface IStatisticsService
    {
        Task<List<T>> GetWeeklyData<T>(Position position, int season, int week);
        Task<List<T>> GetWeeklyData<T>(Position position, int playerId);
        Task<List<T>> GetWeeklyData<T>(Position position, int playerId, string team);
        Task<List<T>> GetWeeklyDataByPlayer<T>(Position position, int playerId, int season);
        Task<List<T>> GetAllWeeklyDataByPosition<T>(Position position);
        Task<List<T>> GetAllSeasonDataByPosition<T>(Position position);
        Task<List<T>> GetSeasonData<T>(Position position, int queryParam, bool isPlayer);
        Task<List<WeeklyRosterPercent>> GetWeeklyRosterPercentages(int season, int week);
        Task<List<GameResult>> GetGameResults(int season);
        Task<List<SnapCount>> GetSnapCounts(int playerId);
        Task<IEnumerable<SnapCount>> GetSnapCountsBySeason(IEnumerable<int> playerIds, int season);
        Task<double> GetSnapsByGame(int playerId, int season, int week);
        Task<List<T>> GetSeasonDataByTeamIdAndPosition<T>(int teamId, Position position, int season);
        Task<IEnumerable<StarterMissedGames>> GetCurrentStartersThatMissedGamesLastSeason(int currentSeason, int previousSeason, int maxGames, double avgProjection);
        Task<double> GetYearsExperience(int playerId, Position position);
        Task<double> GetAverageGamesMissed(int playerId);
        Task<IEnumerable<SeasonADP>> GetAdpByPosition(int season, Position position);
        Task<bool> DeleteAdpByPosition(int season, Position position);
        Task<IEnumerable<ConsensusProjections>> GetConsensusProjectionsByPosition(int season, Position position);
        Task<bool> DeleteConsensusProjectionsByPosition(int season, Position position);
        Task<IEnumerable<ConsensusWeeklyProjections>> GetConsensusWeeklyProjectionsByPosition(int season, int week, Position position);
        Task<bool> DeleteConsensusWeeklyProjectionsByPosition(int season, int week, Position position);
            
    }
}
