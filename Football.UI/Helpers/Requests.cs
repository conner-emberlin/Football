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
        public async Task<List<SnapCountAnalysisModel>?> SnapCountAnalysisRequest(string position)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7028/api/Fantasy/snap-analysis/" + position);
            request.Headers.Add("Accept", "application/json");    
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<List<SnapCountAnalysisModel>>(responseStream, _options);
            }
            return null;
        }

        public async Task<List<FantasyAnalysisModel>?> GetFantasyAnalysesRequest(Position position)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7028/api/Fantasy/fantasy-analysis/" + position.ToString());
            request.Headers.Add("Accept", "application/json");
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<List<FantasyAnalysisModel>>(responseStream, _options);
            }
            return null;
        }

        public async Task<List<SeasonFantasy>?> GetSeasonTotals(string season = "")
        {
            var path = "https://localhost:7028/api/Fantasy/season-totals";
            if (!string.IsNullOrEmpty(season)) path += string.Format("?fantasySeason={0}", season);
            var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Add("Accept", "application/json");
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return (await JsonSerializer.DeserializeAsync<List<SeasonFantasy>>(responseStream, _options)).OrderByDescending(p => p.FantasyPoints).ToList();
            }
            return null;
        }

        public async Task<List<WeeklyFantasy>?> GetWeeklyFantasy(string week, string season = "")
        {
            var path = "https://localhost:7028/api/Fantasy/data/weekly/leaders/" + week;
            if (!string.IsNullOrEmpty(season)) path += string.Format("?season={0}", season);
            var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Add("Accept", "application/json");
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<List<WeeklyFantasy>>(responseStream, _options);
            }
            return null;
        }
        public async Task<List<WeeklyProjectionAnalysis>?> GetCurrentSeasonWeeklyAnalysisRequest(string position)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7028/api/Projection/weekly-analysis/" + position);
            request.Headers.Add("Accept", "application/json");
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<List<WeeklyProjectionAnalysis>>(responseStream, _options);
            }
            return null;
        }

        public async Task<List<SeasonProjectionAnalysis>?> GetPreviousSeasonsAnalysisRequest(string position)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7028/api/Projection/season-projection-analysis/all/" + position);
            request.Headers.Add("Accept", "application/json");
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<List<SeasonProjectionAnalysis>>(responseStream, _options);
            }
            return null;
        }

        public async Task<List<TargetShareModel>?> GetTargetShareRequest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7028/api/Fantasy/targetshares/");
            request.Headers.Add("Accept", "application/json");
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<List<TargetShareModel>>(responseStream, _options);
            }
            return null;
        }

    }
}
