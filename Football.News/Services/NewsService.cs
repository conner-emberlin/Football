using Microsoft.Extensions.Options;
using Football.News.Interfaces;
using Football.News.Models;
using System.Text.Json;

namespace Football.News.Services
{
    public class NewsService : INewsService
    {
        private readonly ESPN _espn;
        private readonly WeatherAPI _weatherAPI;

        public NewsService(IOptionsMonitor<ESPN> espn, IOptionsMonitor<WeatherAPI> weatherAPI)
        {
            _espn = espn.CurrentValue;
            _weatherAPI = weatherAPI.CurrentValue;
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
            else return new EspnNews { };
        }

        public async Task<WeatherAPIRoot> GetWeatherAPI(string zip)
        {
            var url = string.Format("{0}?key={1}&q={2}&days={3}&aqi=no&alerts=no", _weatherAPI.WeatherAPIURL, _weatherAPI.WeatherAPIKey, zip, _weatherAPI.ForecastDays);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/json");
            var client = new HttpClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<WeatherAPIRoot>(responseStream, options);
            }
            return new WeatherAPIRoot { };
        }
    }
}
