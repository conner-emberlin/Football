
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
        public Task<List<SeasonFlex>> SeasonFlexRankings();
        public Task<List<WeekProjection>> WeeklyFlexRankings();
        public Task<List<SeasonProjectionError>> GetSeasonProjectionError(Position position);

    }
}
