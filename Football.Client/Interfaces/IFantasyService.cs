using Football.Shared.Models.Fantasy;

namespace Football.Client.Interfaces
{
    public interface IFantasyService
    {
        Task<List<SnapCountAnalysisModel>?> SnapCountAnalysisRequest(string position);
        Task<List<FantasyAnalysisModel>?> GetFantasyAnalysesRequest(string position);
        Task<List<SeasonFantasyModel>?> GetSeasonTotalsRequest(string season = "");
        Task<List<WeeklyFantasyModel>?> GetWeeklyFantasyRequest(string week, string season = "");
        Task<List<TargetShareModel>?> GetTargetShareRequest();
        Task<List<MatchupRankingModel>?> GetMatchupRankingsRequest(string position);
        Task<List<FantasyPercentageModel>?> GetFantasyPercentageRequest(string position);
        Task<List<MarketShareModel>?> GetMarketShareRequest(string position);        
        Task<List<StartOrSitModel>?> PostStartOrSitRequest(List<int> playerIds);
        Task<List<WaiverWireCandidateModel>?> GetWaiverWireCandidatesRequest();
        Task<List<WeeklyFantasyModel>?> GetWeeklyFantasyByPlayerRequest(string playerId);
        Task<List<TopOpponentsModel>?> GetTopOpponentsRequest(string teamId, string position);
        Task<List<QualityStartsModel>?> GetQualityStartsRequest(string position);
        Task<List<WeeklyFantasyModel>?> GetTopWeeklyPerformancesRequest(string position = "");
    }
}
