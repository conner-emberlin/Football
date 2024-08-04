using Football.Shared.Models.Fantasy;
using Football.Client.Interfaces;

namespace Football.Client.Services
{
    public class FantasyService(IRequests requests) : IFantasyService
    {
        public async Task<List<SnapCountAnalysisModel>?> SnapCountAnalysisRequest(string position) => await requests.Get<List<SnapCountAnalysisModel>?>("/fantasy/snap-analysis/" + position);
        public async Task<List<FantasyAnalysisModel>?> GetFantasyAnalysesRequest(string position) => await requests.Get<List<FantasyAnalysisModel>?>("/fantasy/fantasy-analysis/" + position.ToString());
        public async Task<List<TargetShareModel>?> GetTargetShareRequest() => await requests.Get<List<TargetShareModel>?>("/fantasy/targetshares");
        public async Task<List<MatchupRankingModel>?> GetMatchupRankingsRequest(string position) => await requests.Get<List<MatchupRankingModel>?>("/fantasy/matchup-rankings/" + position);
        public async Task<List<FantasyPercentageModel>?> GetFantasyPercentageRequest(string position) => await requests.Get<List<FantasyPercentageModel>?>("/fantasy/shares/" + position);
        public async Task<List<MarketShareModel>?> GetMarketShareRequest(string position) => await requests.Get<List<MarketShareModel>?>("/fantasy/marketshare/" + position);
        public async Task<List<StartOrSitModel>?> PostStartOrSitRequest(List<int> playerIds) => await requests.Post<List<StartOrSitModel>?, List<int>>("/fantasy/start-or-sit", playerIds);
        public async Task<List<WaiverWireCandidateModel>?> GetWaiverWireCandidatesRequest() => await requests.Get<List<WaiverWireCandidateModel>?>("/fantasy/waiver-wire");
        public async Task<List<WeeklyFantasyModel>?> GetWeeklyFantasyByPlayerRequest(string id) => await requests.Get<List<WeeklyFantasyModel>?>("/fantasy/data/weekly/" + id);
        public async Task<List<TopOpponentsModel>?> GetTopOpponentsRequest(string teamId, string position) => await requests.Get<List<TopOpponentsModel>?>(string.Format("{0}/{1}/{2}", "/fantasy/top-opponents", teamId, position));

        public async Task<List<SeasonFantasyModel>?> GetSeasonTotalsRequest(string season = "")
        {
            var path = "/fantasy/season-totals";
            if (!string.IsNullOrEmpty(season)) path += string.Format("?fantasySeason={0}", season);
            return await requests.Get<List<SeasonFantasyModel>?>(path);
        }

        public async Task<List<WeeklyFantasyModel>?> GetWeeklyFantasyRequest(string week, string season = "")
        {
            var path = "/fantasy/data/weekly/leaders/" + week;
            if (!string.IsNullOrEmpty(season)) path += string.Format("?season={0}", season);
            return await requests.Get<List<WeeklyFantasyModel>?>(path);
        }
    }
}
