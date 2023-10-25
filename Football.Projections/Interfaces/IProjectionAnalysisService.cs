
using Football.Enums;
using Football.Projections.Models;

namespace Football.Projections.Interfaces
{
    public interface IProjectionAnalysisService
    {
        public Task<List<WeeklyProjectionError>> GetWeeklyProjectionError(Position position, int week);
        public Task<WeeklyProjectionAnalysis> GetWeeklyProjectionAnalysis(Position position, int week);
        public Task<List<SeasonFlex>> SeasonFlexRankings();
        public Task<List<WeekProjection>> WeeklyFlexRankings();
    }
}
