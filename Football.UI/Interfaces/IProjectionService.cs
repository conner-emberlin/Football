using Football.Api.Models.Projection;

namespace Football.UI.Interfaces
{
    public interface IProjectionService
    {
        public Task<List<WeeklyProjectionAnalysisModel>?> GetCurrentSeasonWeeklyAnalysisRequest(string position);
        public Task<List<SeasonProjectionAnalysisModel>?> GetPreviousSeasonsAnalysisRequest(string position);
        public Task<List<SeasonProjectionModel>?> GetSeasonProjectionsRequest(string position);
        public Task<bool> DeleteSeasonProjectionRequest(int playerId, int season);
        public Task<int> PostSeasonProjectionRequest(string position);
        public Task<List<WeeklyProjectionErrorModel>?> GetWeeklyProjectionErrorRequest(string position, string week);
        public Task<List<WeekProjectionModel>?> GetWeekProjectionsRequest(string position);
        public Task<bool> DeleteWeekProjectionRequest(int playerId, int season, int week);
        public Task<int> PostWeekProjectionRequest(string position);
    }
}
