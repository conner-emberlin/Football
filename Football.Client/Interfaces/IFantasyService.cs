﻿using Football.Shared.Models.Fantasy;

namespace Football.Client.Interfaces
{
    public interface IFantasyService
    {
        public Task<List<SnapCountAnalysisModel>?> SnapCountAnalysisRequest(string position);
        public Task<List<FantasyAnalysisModel>?> GetFantasyAnalysesRequest(string position);
        public Task<List<SeasonFantasyModel>?> GetSeasonTotalsRequest(string season = "");
        public Task<List<WeeklyFantasyModel>?> GetWeeklyFantasyRequest(string week, string season = "");
        public Task<List<TargetShareModel>?> GetTargetShareRequest();
        public Task<List<MatchupRankingModel>?> GetMatchupRankingsRequest(string position);
        public Task<List<FantasyPercentageModel>?> GetFantasyPercentageRequest(string position);
        public Task<List<MarketShareModel>?> GetMarketShareRequest(string position);        
        public Task<List<StartOrSitModel>?> PostStartOrSitRequest(List<int> playerIds);
        public Task<List<WaiverWireCandidateModel>?> GetWaiverWireCandidatesRequest();
        public Task<List<WeeklyFantasyModel>?> GetWeeklyFantasyByPlayerRequest(string playerId);
        public Task<List<TopOpponentsModel>?> GetTopOpponentsRequest(string teamId, string position);
    }
}
