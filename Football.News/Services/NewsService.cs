using News.Interfaces;
using News.Models;
using System.Text.Json;

namespace News.Services
{
    public class NewsService : INewsService
    {
        public async Task<Root> GetEspnNews()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://site.api.espn.com/apis/site/v2/sports/football/nfl/news");
            request.Headers.Add("Accept", "application/json");
            var client = new HttpClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<Root>(responseStream, options);
            }
            else return null;
        }
    }
}
