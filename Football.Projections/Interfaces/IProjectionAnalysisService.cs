
using Football.Enums;
using Football.Projections.Models;

namespace Football.Projections.Interfaces
{
    public interface IProjectionAnalysisService
    {
        public Task<List<WeeklyProjectionError>> GetWeeklyProjectionError(int playerId);
        public Task<List<WeeklyProjectionError>> GetWeeklyProjectionError(Position position, int week);
        public Task<WeeklyProjectionAnalysis> GetWeeklyProjectionAnalysis(Position position, int week);
        public Task<WeeklyProjectionAnalysis> GetWeeklyProjectionAnalysis(int playerId);
        public Task<IEnumerable<SeasonFlex>> SeasonFlexRankings();
        public Task<IEnumerable<WeekProjection>> WeeklyFlexRankings();
        public Task<List<SeasonProjectionError>> GetSeasonProjectionError(Position position, int season = 0);
        public Task<SeasonProjectionAnalysis> GetSeasonProjectionAnalysis(Position position, int season = 0);
        public Task<List<SeasonProjectionAnalysis>> GetAllSeasonProjectionAnalyses(Position position);

    }
}
