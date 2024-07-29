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
        public Task<bool> CreateRookieRequest(RookiePlayerModel rookie);
        public Task<List<TeamMapModel>?> GetAllTeamsRequest();
        public Task<List<GameResultModel>?> GetGameResultsRequest();
        public Task<List<TeamRecordModel>?> GetTeamRecordsRequest();
        public Task<List<FantasyPerformanceModel>?> GetFantasyPerformancesRequest(int teamId);
        public Task<List<ScheduleDetailsModel>?> GetScheduleDetailsRequest();
        public Task<TuningsModel?> GetSeasonTuningsRequest();
        public Task<bool> PostSeasonTuningsRequest(TuningsModel model);
        public Task<WeeklyTuningsModel?> GetWeeklyTuningsRequest();
        public Task<bool> PostWeeklyTuningsRequest(WeeklyTuningsModel model);
        public Task<int> PutSeasonAdpRequest(string position);
    }
}
