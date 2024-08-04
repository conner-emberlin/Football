using Football.Shared.Models.Projection;
using Football.UI.Interfaces;
namespace Football.UI.Services
{
    public class ProjectionService(IRequests requests) : IProjectionService
    {
        public async Task<List<WeeklyProjectionAnalysisModel>?> GetCurrentSeasonWeeklyAnalysisRequest(string position) => await requests.Get<List<WeeklyProjectionAnalysisModel>?>("/projection/weekly-analysis/" + position);
        public async Task<List<SeasonProjectionAnalysisModel>?> GetPreviousSeasonsAnalysisRequest(string position) => await requests.Get<List<SeasonProjectionAnalysisModel>?>("/projection/season-projection-analysis/all/" + position);
        public async Task<List<SeasonProjectionModel>?> GetSeasonProjectionsRequest(string position) => await requests.Get<List<SeasonProjectionModel>?>("/projection/season/" + position);
        public async Task<bool> DeleteSeasonProjectionRequest(int playerId, int season) => await requests.Delete<bool>(string.Format("{0}/{1}/{2}", "/projection/season", playerId, season));
        public async Task<int> PostSeasonProjectionRequest(string position) => await requests.PostWithoutBody<int>("/projection/season/" + position);
        public async Task<List<WeeklyProjectionErrorModel>?> GetWeeklyProjectionErrorRequest(string position, string week) => await requests.Get<List<WeeklyProjectionErrorModel>?>(string.Format("{0}/{1}/{2}", "/projection/weekly-error", position, week));
        public async Task<List<WeeklyProjectionErrorModel>?> GetWeeklyProjectionErrorRequest(string id) => await requests.Get<List<WeeklyProjectionErrorModel>?>("/projection/weekly-error/" + id);
        public async Task<WeeklyProjectionAnalysisModel?> GetWeeklyProjectionAnalysisRequest(string id) => await requests.Get<WeeklyProjectionAnalysisModel?>("/projection/weekly-analysis/player/" + id);
        public async Task<List<WeekProjectionModel>?> GetWeekProjectionsRequest(string position) => await requests.Get<List<WeekProjectionModel>?>("/projection/weekly/" + position);
        public async Task<bool> DeleteWeekProjectionRequest(int playerId, int season, int week) => await requests.Delete<bool>(string.Format("{0}/{1}/{2}/{3}", "/projection/weekly", playerId, season, week));
        public async Task<int> PostWeekProjectionRequest(string position) => await requests.PostWithoutBody<int>("/projection/weekly/" + position);
        public async Task<List<MatchupProjectionsModel>?> GetMatchupProjectionsRequest(string teamName) => await requests.Get<List<MatchupProjectionsModel>?>(string.Format("{0}/{1}/matchup", "/projection/sleeper-projections", teamName));
    }
}
