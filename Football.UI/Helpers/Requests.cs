using Football.Api.Models;
using Football.Enums;
using System.Text;
using System.Text.Json;

namespace Football.UI.Helpers
{
    public class Requests : IRequests
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;
        private readonly string _baseURL = "https://localhost:7028/api";
        public Requests(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
        public async Task<List<SnapCountAnalysisModel>?> SnapCountAnalysisRequest(string position) => await Get<List<SnapCountAnalysisModel>?>("/fantasy/snap-analysis/" + position);
        public async Task<List<FantasyAnalysisModel>?> GetFantasyAnalysesRequest(Position position) => await Get<List<FantasyAnalysisModel>?>("/fantasy/fantasy-analysis/" + position.ToString());
        public async Task<List<TargetShareModel>?> GetTargetShareRequest() => await Get<List<TargetShareModel>?>("/fantasy/targetshares");
        public async Task<List<MatchupRankingModel>?> GetMatchupRankingsRequest(string position) => await Get<List<MatchupRankingModel>?>("/fantasy/matchup-rankings/" + position);
        public async Task<List<FantasyPercentageModel>?> GetFantasyPercentageRequest(string position) => await Get<List<FantasyPercentageModel>?>("/fantasy/shares/" + position);
        public async Task<List<MarketShareModel>?> GetMarketShareRequest(string position) => await Get<List<MarketShareModel>?>("/fantasy/marketshare/" + position);
        public async Task<List<StartOrSitModel>?> PostStartOrSitRequest(List<int> playerIds) => await Post<List<StartOrSitModel>?, List<int>>("/fantasy/start-or-sit", playerIds);
        public async Task<List<WaiverWireCandidateModel>?> GetWaiverWireCandidatesRequest() => await Get<List<WaiverWireCandidateModel>?>("/fantasy/waiver-wire");

        public async Task<List<SeasonFantasyModel>?> GetSeasonTotalsRequest(string season = "")
        {
            var path = "/fantasy/season-totals";
            if (!string.IsNullOrEmpty(season)) path += string.Format("?fantasySeason={0}", season);
            return await Get<List<SeasonFantasyModel>?>(path);
        }

        public async Task<List<WeeklyFantasyModel>?> GetWeeklyFantasyRequest(string week, string season = "")
        {
            var path = "/fantasy/data/weekly/leaders/" + week;
            if (!string.IsNullOrEmpty(season)) path += string.Format("?season={0}", season);
            return await Get<List<WeeklyFantasyModel>?>(path);
        }
        public async Task<TuningsModel?> GetSeasonTuningsRequest() => await Get<TuningsModel>("/operations/season-tunings");
        public async Task<bool> PostSeasonTuningsRequest(TuningsModel tunings) => await Post<bool, TuningsModel>("/operations/season-tunings", tunings);
        public async Task<WeeklyTuningsModel?> GetWeeklyTuningsRequest() => await Get<WeeklyTuningsModel>("/operations/weekly-tunings");
        public async Task<bool> PostWeeklyTuningsRequest(WeeklyTuningsModel tunings) => await Post<bool, WeeklyTuningsModel>("/operations/weekly-tunings", tunings);
        public async Task<int> PutSeasonAdpRequest(string position) => await Put<int>("/operations/refresh-adp/" + position);

        public async Task<PlayerDataModel?> GetPlayerDataRequest(int playerId) => await Get<PlayerDataModel?>("/player/" + playerId.ToString());
        public async Task<List<TrendingPlayerModel>?> GetTrendingPlayersRequest() => await Get<List<TrendingPlayerModel>?>("/player/trending-players");
        public async Task<bool> CreateRookieRequest(RookiePlayerModel rookie) => await Post<bool, RookiePlayerModel>("/player/add-rookie", rookie);
        public async Task<List<SimplePlayerModel>?> GetSimplePlayersRequest(bool activeOnly = false)
        {
            var path = "/player/data/players";
            if (activeOnly) path += string.Format("?active=1");
            return await Get<List<SimplePlayerModel>?>(path);
        }

        public async Task<List<WeeklyProjectionAnalysisModel>?> GetCurrentSeasonWeeklyAnalysisRequest(string position) => await Get<List<WeeklyProjectionAnalysisModel>?>("/projection/weekly-analysis/" + position); 
        public async Task<List<SeasonProjectionAnalysisModel>?> GetPreviousSeasonsAnalysisRequest(string position) => await Get<List<SeasonProjectionAnalysisModel>?>("/projection/season-projection-analysis/all/" + position);        
        public async Task<List<SeasonProjectionModel>?> GetSeasonProjectionsRequest(string position) => await Get<List<SeasonProjectionModel>?>("/projection/season/" + position);
        public async Task<bool> DeleteSeasonProjectionRequest(int playerId, int season) => await Delete<bool>(string.Format("{0}/{1}/{2}", "/projection/season", playerId, season));
        public async Task<int> PostSeasonProjectionRequest(string position) => await PostWithoutBody<int>("/projection/season/" + position);
        public async Task<List<WeeklyProjectionErrorModel>?> GetWeeklyProjectionErrorRequest(string position, string week) => await Get<List<WeeklyProjectionErrorModel>?>(string.Format("{0}/{1}/{2}", "/projection/weekly-error", position, week));
        public async Task<List<WeekProjectionModel>?> GetWeekProjectionsRequest(string position) => await Get<List<WeekProjectionModel>?>("/projection/weekly/" + position);
        public async Task<bool> DeleteWeekProjectionRequest(int playerId, int season, int week) => await Delete<bool>(string.Format("{0}/{1}/{2}/{3}", "/projection/weekly", playerId, season, week));
        public async Task<int> PostWeekProjectionRequest(string position) => await PostWithoutBody<int>("/projection/weekly/" + position);

        public async Task<List<TeamMapModel>?> GetAllTeamsRequest() => await Get<List<TeamMapModel>?>("/team/all");
        public async Task<List<GameResultModel>?> GetGameResultsRequest() => await Get<List<GameResultModel>?>("/team/game-results");
        public async Task<List<TeamRecordModel>?> GetTeamRecordsRequest() => await Get<List<TeamRecordModel>?>("/team/team-records");
        public async Task<List<FantasyPerformanceModel>?> GetFantasyPerformancesRequest(int teamId) => await Get<List<FantasyPerformanceModel>?>("/team/fantasy-performances/" + teamId.ToString());
        public async Task<List<ScheduleDetailsModel>?> GetScheduleDetailsRequest() => await Get<List<ScheduleDetailsModel>?>("/team/schedule-details/current");

        private async Task<T?> Get<T>(string path)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _baseURL + path);
            request.Headers.Add("Accept", "application/json");
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<T>(responseStream, _options);
            }
            return default;
        }
        private async Task<T?> Delete<T>(string path)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, _baseURL + path);
            request.Headers.Add("Accept", "application/json");
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<T>(responseStream, _options);
            }
            return default;
        }

        private async Task<T?> Put<T>(string path)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, _baseURL + path);
            request.Headers.Add("Accept", "application/json");
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<T>(responseStream, _options);
            }
            return default;
        }

        private async Task<T?> PostWithoutBody<T>(string path)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _baseURL + path);
            request.Headers.Add("Accept", "application/json");
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<T>(responseStream, _options);
            }
            return default;
        }

        private async Task<T?> Post<T, T1>(string path, T1 model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, _baseURL + path) { Content = content };
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = response.Content.ReadAsStream();
                return await JsonSerializer.DeserializeAsync<T>(responseStream, _options);
            }
            return default;
        }
    }
}
