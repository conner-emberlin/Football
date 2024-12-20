using Football.Shared.Models.Projection;

namespace Football.Client.Interfaces
{
    public interface IProjectionService
    {
        Task<List<WeeklyProjectionAnalysisModel>?> GetCurrentSeasonWeeklyAnalysisRequest(string position);
        Task<List<SeasonProjectionAnalysisModel>?> GetPreviousSeasonsAnalysisRequest(string position);
        Task<List<SeasonProjectionModel>?> GetSeasonProjectionsRequest(string position, List<string> filter);
        Task<bool> DeleteSeasonProjectionRequest(int playerId, int season);
        Task<int> PostSeasonProjectionRequest(string position, List<string> filters);
        Task<List<WeeklyProjectionErrorModel>?> GetWeeklyProjectionErrorRequest(string position, string week);
        Task<List<WeeklyProjectionErrorModel>?> GetWeeklyProjectionErrorRequest(string id);
        Task<WeeklyProjectionAnalysisModel?> GetWeeklyProjectionAnalysisRequest(string id);
        Task<List<WeekProjectionModel>?> GetWeekProjectionsRequest(string position, List<string> filter);
        Task<bool> DeleteWeekProjectionRequest(int playerId, int season, int week);
        Task<int> PostWeekProjectionRequest(string position, List<string> filters);
        Task<List<MatchupProjectionsModel>?> GetMatchupProjectionsRequest(string teamName);
        Task<List<string>?> GetSeasonModelVariablesRequest(string position);
        Task<SeasonProjectionsExistModel?> GetSeasonProjectionsExistRequest(string position);
        Task<List<string>?> GetWeeklyModelVariablesRequest(string position);
        Task<WeeklyProjectionsExistModel?> GetWeeklyProjectionsExistRequest(string position);
        Task<List<AdjustmentDescriptionModel>?> GetSeasonAdjustmentDescriptionsRequest();
        Task<List<PlayerWeeklyProjectionAnalysisModel>?> GetPlayerWeeklyProjectionAnalysisRequest(string position);
    }
}
