
using Football.Enums;
using Football.Projections.Models;

namespace Football.Projections.Interfaces
{
    public interface IProjectionAnalysisService
    {
        public Task<List<WeeklyProjectionError>> GetWeeklyProjectionError(PositionEnum position, int week);
        public Task<WeeklyProjectionAnalysis> GetWeeklyProjectionAnalysis(PositionEnum position, int week);
    }
}
