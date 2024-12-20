using Football.Shared.Models.Projection;
using Football.Client.Interfaces;
namespace Football.Client.Services
{
    public class ProjectionService(IRequests requests) : IProjectionService
    {
        public async Task<List<WeeklyProjectionAnalysisModel>?> GetCurrentSeasonWeeklyAnalysisRequest(string position) => await requests.Get<List<WeeklyProjectionAnalysisModel>?>("/projection/weekly-analysis/" + position);
        public async Task<List<SeasonProjectionAnalysisModel>?> GetPreviousSeasonsAnalysisRequest(string position) => await requests.Get<List<SeasonProjectionAnalysisModel>?>("/projection/season-projection-analysis/all/" + position);
        public async Task<List<SeasonProjectionModel>?> GetSeasonProjectionsRequest(string position, List<string> filter) => await requests.Post<List<SeasonProjectionModel>?, List<string>>("/projection/season/" + position, filter);
        public async Task<bool> DeleteSeasonProjectionRequest(int playerId, int season) => await requests.Delete<bool>(string.Format("{0}/{1}/{2}", "/projection/season", playerId, season));
        public async Task<int> PostSeasonProjectionRequest(string position, List<string> filters) => await requests.Post<int, List<string>>("/projection/season/upload/" + position, filters);
        public async Task<List<WeeklyProjectionErrorModel>?> GetWeeklyProjectionErrorRequest(string position, string week) => await requests.Get<List<WeeklyProjectionErrorModel>?>(string.Format("{0}/{1}/{2}", "/projection/weekly-error", position, week));
        public async Task<List<WeeklyProjectionErrorModel>?> GetWeeklyProjectionErrorRequest(string id) => await requests.Get<List<WeeklyProjectionErrorModel>?>("/projection/weekly-error/" + id);
        public async Task<WeeklyProjectionAnalysisModel?> GetWeeklyProjectionAnalysisRequest(string id) => await requests.Get<WeeklyProjectionAnalysisModel?>("/projection/weekly-analysis/player/" + id);
        public async Task<List<WeekProjectionModel>?> GetWeekProjectionsRequest(string position, List<string> filter) => await requests.Post<List<WeekProjectionModel>?, List<string>>("/projection/weekly/" + position, filter);
        public async Task<bool> DeleteWeekProjectionRequest(int playerId, int season, int week) => await requests.Delete<bool>(string.Format("{0}/{1}/{2}/{3}", "/projection/weekly", playerId, season, week));
        public async Task<int> PostWeekProjectionRequest(string position, List<string> filters) => await requests.Post<int, List<string>>("/projection/weekly/upload/" + position, filters);
        public async Task<List<MatchupProjectionsModel>?> GetMatchupProjectionsRequest(string teamName) => await requests.Get<List<MatchupProjectionsModel>?>(string.Format("{0}/{1}/matchup", "/projection/sleeper-projections", teamName));
        public async Task<List<string>?> GetSeasonModelVariablesRequest(string position) => await requests.Get<List<string>?>("/projection/season-model-variables/" + position);
        public async Task<SeasonProjectionsExistModel?> GetSeasonProjectionsExistRequest(string position) => await requests.Get<SeasonProjectionsExistModel?>("/projection/season/projections-exist/" + position);
        public async Task<List<string>?> GetWeeklyModelVariablesRequest(string position) => await requests.Get<List<string>?>("/projection/weekly-model-variables/" + position);
        public async Task<WeeklyProjectionsExistModel?> GetWeeklyProjectionsExistRequest(string position) => await requests.Get<WeeklyProjectionsExistModel?>("/projection/weekly/projections-exist/" + position);
        public async Task<List<AdjustmentDescriptionModel>?> GetSeasonAdjustmentDescriptionsRequest() => await requests.Get<List<AdjustmentDescriptionModel>?>("/projection/season-adjustment-descriptions");
        public async Task<List<PlayerWeeklyProjectionAnalysisModel>?> GetPlayerWeeklyProjectionAnalysisRequest(string position) => await requests.Get<List<PlayerWeeklyProjectionAnalysisModel>?>("/projection/weekly-player-analysis/" + position);
    }
}
