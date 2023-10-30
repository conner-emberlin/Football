using Football.LeagueAnalysis.Models;
using Football.LeagueAnalysis.Interfaces;
using Football.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Football.Players.Interfaces;
using System.Text.Json.Nodes;

namespace Football.LeagueAnalysis.Services
{
    public class SleeperLeagueService : ISleeperLeagueService
    {
        private readonly SleeperSettings _settings;
        private readonly Season _season;
        private readonly IPlayersService _playersService;
        public SleeperLeagueService(IOptionsMonitor<SleeperSettings> settings, IOptionsMonitor<Season> season, IPlayersService playersService)

        {
            _settings = settings.CurrentValue;
            _season = season.CurrentValue;
            _playersService = playersService;
        }

        public async Task<SleeperUser?> GetSleeperUser(string username)
        {
            var requestUrl = string.Format("{0}/user/{1}", _settings.SleeperBaseURL, username);
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Accept", "application/json");
            var client = new HttpClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<SleeperUser>(responseStream, options);
            }
            else return new SleeperUser();
        }

        public async Task<List<SleeperLeague>?> GetSleeperLeagues(string userId)
        {
            var requestUrl = string.Format("{0}/user/{1}/leagues/nfl/{2}", _settings.SleeperBaseURL, userId, _season.CurrentSeason.ToString());
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Accept", "application/json");
            var client = new HttpClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<List<SleeperLeague>>(responseStream, options);
            }
            else return Enumerable.Empty<SleeperLeague>().ToList();
        }

        public async Task<SleeperLeague?> GetSleeperLeague(string leagueId)
        {
            var requestUrl = string.Format("{0}/league/{1}", _settings.SleeperBaseURL, leagueId);
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Accept", "application/json");
            var client = new HttpClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<SleeperLeague>(responseStream, options);
            }
            else return new SleeperLeague();
        }
        public async Task<List<SleeperRoster>?> GetSleeperRosters(string leagueId)
        {
            var requestUrl = string.Format("{0}/league/{1}/rosters", _settings.SleeperBaseURL, leagueId);
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Accept", "application/json");
            var client = new HttpClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<List<SleeperRoster>>(responseStream, options);
            }
            else return Enumerable.Empty<SleeperRoster>().ToList();
        }

        public async Task<List<SleeperMatchup>?> GetSleeperMatchups(string leagueId, int week)
        {
            var requestUrl = string.Format("{0}/league/{1}/matchups/{2}", _settings.SleeperBaseURL, leagueId, week.ToString());
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Accept", "application/json");
            var client = new HttpClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<List<SleeperMatchup>>(responseStream, options);
            }
            else return Enumerable.Empty<SleeperMatchup>().ToList();
        }

        public async Task<List<SleeperPlayer>> GetSleeperPlayers()
        {
            List<SleeperPlayer> sleeperPlayers = new();            
            var requestUrl = string.Format("{0}/players/nfl", _settings.SleeperBaseURL);
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Accept", "application/json");
            var client = new HttpClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                using var responseStream = await response.Content.ReadAsStreamAsync();
                var dictionary = await JsonSerializer.DeserializeAsync<Dictionary<string, JsonObject>>(responseStream, options);
                if (dictionary != null)
                {
                    foreach (var item in dictionary)
                    {
                        var firstName = item.Value["first_name"];
                        var lastName = item.Value["last_name"];
                        if (int.TryParse(item.Key, out var value) && firstName != null && lastName != null && value > 0)
                        {
                            sleeperPlayers.Add(new SleeperPlayer
                            {
                                SleeperPlayerId = value,
                                PlayerName = firstName.ToString() + " " + lastName.ToString()
                            });
                        }
                    }
                }
            }          
            return sleeperPlayers;
        }
       
        public async Task<List<SleeperPlayerMap>> GetSleeperPlayerMap(List<SleeperPlayer> sleeperPlayers)
        {
            List<SleeperPlayerMap> playerMap = new();
            if (sleeperPlayers.Any())
            {
                foreach (var sp in sleeperPlayers)
                {
                    if (!string.IsNullOrWhiteSpace(sp.PlayerName))
                    {
                        var playerId = await _playersService.GetPlayerId(sp.PlayerName);
                        if (playerId > 0)
                        {
                            playerMap.Add(new SleeperPlayerMap
                            {
                                SleeperPlayerId = sp.SleeperPlayerId,
                                PlayerId = playerId
                            });
                        }
                    }
                }
                return playerMap;
            }
            else throw new NullReferenceException();
        }
        
    }
}
