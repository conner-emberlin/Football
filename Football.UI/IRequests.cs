using Football.Api.Models;
using Football.Enums;

namespace Football.UI
{
    public interface IRequests
    {
        public Task<List<SnapCountAnalysisModel>?> SnapCountAnalysisRequest(string position);
        public Task<List<FantasyAnalysisModel>?> GetFantasyAnalysesRequest(Position position);
        public Task<List<SeasonFantasyModel>?> GetSeasonTotalsRequest(string season = "");
        public Task<List<WeeklyFantasyModel>?> GetWeeklyFantasyRequest(string week, string season = "");
        public Task<List<WeeklyProjectionAnalysisModel>?> GetCurrentSeasonWeeklyAnalysisRequest(string position);
        public Task<List<SeasonProjectionAnalysisModel>?> GetPreviousSeasonsAnalysisRequest(string position);
        public Task<List<TargetShareModel>?> GetTargetShareRequest();
        public Task<List<SeasonProjectionModel>?> GetSeasonProjectionsRequest(string position);
        public Task<bool> DeleteSeasonProjectionRequest(int playerId, int season);
        public Task<int> PostSeasonProjectionRequest(string position);
        public Task<List<MatchupRankingModel>?> GetMatchupRankingsRequest(string position);
        public Task<List<FantasyPercentageModel>?> GetFantasyPercentageRequest(string position);
        public Task<List<MarketShareModel>?> GetMarketShareRequest(string position);
        public Task<List<WeeklyProjectionErrorModel>?> GetWeeklyProjectionErrorRequest(string position, string week);
    }
}
