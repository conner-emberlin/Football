using Football.Client.Interfaces;
using System.Text;
using System.Text.Json;

namespace Football.Client.Helpers
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

        public async Task<T?> Get<T>(string path)
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
        public async Task<T?> Delete<T>(string path)
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

        public async Task<T?> Put<T>(string path)
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

        public async Task<T?> PutWithBody<T, T1>(string path, T1 body)
        {
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, _baseURL + path) { Content = content };
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = response.Content.ReadAsStream();
                return await JsonSerializer.DeserializeAsync<T>(responseStream, _options);
            }
            return default;
        }

        public async Task<T?> PostWithoutBody<T>(string path)
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

        public async Task<T?> Post<T, T1>(string path, T1 model)
        {
            var json = JsonSerializer.Serialize(model);           
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, _baseURL + path) { Content = content };
            request.Headers.Add("Accept", "application/json");
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
