using Football.Api.Models;
using Football.Enums;
using Football.Fantasy.Models;
using Football.Projections.Models;
using System.Text.Json;

namespace Football.UI.Helpers
{
    public class Requests : IRequests
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;
        public Requests(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
        public async Task<List<SnapCountAnalysisModel>?> SnapCountAnalysisRequest(string position) => await Get<List<SnapCountAnalysisModel>?>("https://localhost:7028/api/Fantasy/snap-analysis/" + position);
        public async Task<List<FantasyAnalysisModel>?> GetFantasyAnalysesRequest(Position position) => await Get<List<FantasyAnalysisModel>?>("https://localhost:7028/api/Fantasy/fantasy-analysis/" + position.ToString());
        public async Task<List<SeasonFantasyModel>?> GetSeasonTotalsRequest(string season = "")
        {
            var path = "https://localhost:7028/api/Fantasy/season-totals";
            if (!string.IsNullOrEmpty(season)) path += string.Format("?fantasySeason={0}", season);
            return await Get<List<SeasonFantasyModel>?>(path);
        }

        public async Task<List<WeeklyFantasyModel>?> GetWeeklyFantasyRequest(string week, string season = "")
        {
            var path = "https://localhost:7028/api/Fantasy/data/weekly/leaders/" + week;
            if (!string.IsNullOrEmpty(season)) path += string.Format("?season={0}", season);
            return await Get<List<WeeklyFantasyModel>?>(path);
        }
        public async Task<List<WeeklyProjectionAnalysis>?> GetCurrentSeasonWeeklyAnalysisRequest(string position) => await Get<List<WeeklyProjectionAnalysis>?>("https://localhost:7028/api/Projection/weekly-analysis/" + position); 
        public async Task<List<SeasonProjectionAnalysis>?> GetPreviousSeasonsAnalysisRequest(string position) => await Get<List<SeasonProjectionAnalysis>?>("https://localhost:7028/api/Projection/season-projection-analysis/all/" + position);
        public async Task<List<TargetShareModel>?> GetTargetShareRequest() => await Get<List<TargetShareModel>?>("https://localhost:7028/api/Fantasy/targetshares");
        private async Task<T?> Get<T>(string path)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Add("Accept", "application/json");
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<T>(responseStream, _options);
            }
            return default;
        }
    }
}
