using Football.Enums;
using Football.Projections.Models;

namespace Football.Projections.Interfaces
{
    public interface IProjectionAnalysisService
    {
        Task<List<WeeklyProjectionError>> GetWeeklyProjectionError(int playerId);
        Task<List<WeeklyProjectionError>> GetWeeklyProjectionError(Position position, int week);
        Task<WeeklyProjectionAnalysis> GetWeeklyProjectionAnalysis(Position position, int week);
        Task<WeeklyProjectionAnalysis> GetWeeklyProjectionAnalysis(int playerId);
        Task<Dictionary<int, double>> GetAverageWeeklyProjectionErrorsByPosition(Position position, int season);
        Task<IEnumerable<SeasonFlex>> SeasonFlexRankings();
        Task<IEnumerable<WeekProjection>> WeeklyFlexRankings();
        Task<List<SeasonProjectionError>> GetSeasonProjectionError(Position position, int season = 0);
        Task<SeasonProjectionAnalysis> GetSeasonProjectionAnalysis(Position position, int season = 0);
        Task<List<SeasonProjectionAnalysis>> GetAllSeasonProjectionAnalyses(Position position);
        Task<List<WeekProjection>> GetSleeperLeagueProjections(string username);
        Task<List<MatchupProjections>> GetMatchupProjections(string username, int week);

    }
}
