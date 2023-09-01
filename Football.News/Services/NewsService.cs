using Microsoft.Extensions.Options;
using News.Interfaces;
using News.Models;
using System.Text.Json;

namespace News.Services
{
    public class NewsService : INewsService
    {
        private readonly ESPN _espn;

        public NewsService(IOptionsMonitor<ESPN> espn)
        {
            _espn = espn.CurrentValue;
        }
        public async Task<EspnNews> GetEspnNews()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _espn.ESPNNewsURL);
            request.Headers.Add("Accept", "application/json");
            var client = new HttpClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<EspnNews>(responseStream, options);
            }
            else return null;
        }
    }
}
