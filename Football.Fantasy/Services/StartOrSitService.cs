using Football.Enums;
using Football.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text.Json;


namespace Football.Fantasy.Services
{
    public class StartOrSitService(IPlayersService playersService, IMatchupAnalysisService matchupAnalysisService,
        IFantasyDataService fantasyDataService, ISettingsService settingsService, IMemoryCache cache,
        IOptionsMonitor<Season> season, IOptionsMonitor<NFLOddsAPI> oddsAPI, IOptionsMonitor<WeatherAPI> weatherAPI, JsonOptions options, IHttpClientFactory clientFactory, IMapper mapper) : IStartOrSitService
    {
        private readonly Season _season = season.CurrentValue;
        private readonly NFLOddsAPI _oddsAPI = oddsAPI.CurrentValue;
        private readonly WeatherAPI _weatherAPI = weatherAPI.CurrentValue;

        public async Task<List<StartOrSit>> GetStartOrSits(List<int> playerIds)
        {
            List<StartOrSit> startOrSits = [];
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            var schedule = await playersService.GetScheduleDetails(_season.CurrentSeason, currentWeek);
            foreach (var playerId in playerIds)
            {           
                if (settingsService.GetFromCache<StartOrSit>(playerId, Cache.StartOrSit, out var startOrSit)) startOrSits.Add(startOrSit);
                else 
                {
                    var team = await playersService.GetPlayerTeam(_season.CurrentSeason, playerId);
                    if (team != null)
                    {                       
                        var projection = await playersService.GetWeeklyProjection(_season.CurrentSeason, currentWeek, playerId);
                        var scheduleDetail = schedule.FirstOrDefault(s => s.AwayTeamId == team.TeamId || s.HomeTeamId == team.TeamId);
                        var teamMap = await playersService.GetTeam(team.TeamId);
                        var player = await playersService.GetPlayer(playerId);
                        var sos = new StartOrSit
                        {
                            Player = player,
                            TeamMap = teamMap,
                            ScheduleDetails = scheduleDetail,
                            MatchLines = await GetMatchLines(teamMap),
                            Weather = await GetWeather(scheduleDetail),
                            MatchupRanking = await matchupAnalysisService.GetMatchupRanking(playerId),
                            ProjectedPoints = Math.Round(projection, 2),
                            PlayerComparisons = await GetPlayerComparisons(player, currentWeek, scheduleDetail, teamMap.TeamId, projection)
                        };
                        cache.Set(playerId.ToString() + Cache.StartOrSit, sos);
                        startOrSits.Add(sos);
                    }
                }

            }
            if (startOrSits.Count > 0)
            {
                var starter = startOrSits.First(s => s.ProjectedPoints == startOrSits.Max(s => s.ProjectedPoints)).Player.PlayerId;
                startOrSits.ForEach(s => s.Start = s.Player.PlayerId == starter);
                return startOrSits;
            }
            return Enumerable.Empty<StartOrSit>().ToList();
        }

        private async Task<Weather> GetWeather(ScheduleDetails scheduleDetail)
        {
            if (scheduleDetail != null)
            {
                var homeLocation = await playersService.GetTeamLocation(scheduleDetail.HomeTeamId);
                if (homeLocation.Indoor == 1)
                    return new Weather
                    {
                        GameTime = FormatTime(scheduleDetail.Time, scheduleDetail.Date),
                        Temperature = "Indoor"
                    };
                if (cache.TryGetValue(Cache.WeatherZip.ToString() + homeLocation.Zip, out Weather? zipWeather) && zipWeather != null) return zipWeather;
                var forecast = await GetWeatherAPI(homeLocation.Zip);
                if (forecast != null)
                {
                    var forecastDay = forecast.forecast.forecastday.FirstOrDefault(f => f.date == scheduleDetail.Date);
                    if (forecastDay != null)
                    {
                        var time = FormatTime(scheduleDetail.Time, scheduleDetail.Date);
                        var forecastHour = forecastDay.hour.First(h => h.time == time);
                        if (forecastHour != null)
                        {
                            var weather = mapper.Map<Weather>(forecastHour);
                            cache.Set(Cache.WeatherZip.ToString() + homeLocation.Zip, weather);
                            return weather;
                        }
                    }
                }
            }
            return new ();
        }

        private async Task<List<PlayerComparison>> GetPlayerComparisons(Player player, int currentWeek, ScheduleDetails schedule, int teamId, double playerProjection)
        {
            if (cache.TryGetValue(Cache.PlayerComparisons.ToString() + player.PlayerId.ToString(), out List<PlayerComparison>? comps) && comps != null) return comps;

            List<PlayerComparison> comparisons = [];
            if (schedule == null || playerProjection == 0) return comparisons;

            _ = Enum.TryParse(player.Position, out Position position);
            var playerComparison = settingsService.GetPlayerComparison(position);

            var opponentId = schedule.HomeTeamId == teamId ? schedule.AwayTeamId : schedule.HomeTeamId;
            var opponentSchedule = (await playersService.GetTeamGames(opponentId)).Where(s => s.Week < currentWeek && s.OpposingTeamId > 0);
            foreach (var match in opponentSchedule)
            {
                var opposingTeamPlayers = await playersService.GetPlayersByTeamIdAndPosition(match.OpposingTeamId, position);

                foreach (var op in opposingTeamPlayers)
                {
                    var otherProjectedPoints = await playersService.GetWeeklyProjection(_season.CurrentSeason, match.Week, op.PlayerId);
                    if (Math.Abs(playerProjection - otherProjectedPoints) <= playerComparison)
                    {
                        var weeklyFantasy = await fantasyDataService.GetWeeklyFantasy(op.PlayerId);
                        comparisons.Add(new PlayerComparison
                        {
                            Player = await playersService.GetPlayer(op.PlayerId),
                            Team = match.OpposingTeam,
                            Schedule = match,
                            WeeklyFantasy = weeklyFantasy.FirstOrDefault(w => w.Week == match.Week),
                            ProjectedPoints = otherProjectedPoints
                        });
                    }
                }
            }
            cache.Set(Cache.PlayerComparisons.ToString() + player.PlayerId.ToString(), comparisons);
            return comparisons;
        }
        private async Task<MatchLines> GetMatchLines(TeamMap teamMap)
        {
            var odds = cache.TryGetValue(Cache.NFLOdds.ToString(), out List<NFLOddsRoot>? nflOdds) && nflOdds != null ? nflOdds : await GetNFLOdds();
            var teamOdds = odds.FirstOrDefault(o => o.home_team == teamMap.TeamDescription || o.away_team == teamMap.TeamDescription);
            if (teamOdds != null)
            {
                var bookmaker = teamOdds.bookmakers.FirstOrDefault(b => b.key == _oddsAPI.DefaultBookmaker);
                if (bookmaker != null) return MatchLines(bookmaker, teamMap);
            }
            return new ();
        }

        private static MatchLines MatchLines(Bookmaker bookMaker, TeamMap teamMap)
        {
            MatchLines matchLines = new();
            foreach (var market in bookMaker.markets)
            {
               if (Enum.TryParse(market.key, out Enums.Market marketEnum))
                {
                    switch (marketEnum)
                    {
                        case Enums.Market.spreads: 
                            foreach (var outcome in market.outcomes)
                                if (outcome.name == teamMap.TeamDescription) matchLines.Line = outcome.point;
                        break;
                        case Enums.Market.totals: if (market.outcomes.Count > 0) matchLines.OverUnder = market.outcomes.First().point;                            
                        break;
                        default: break;
                    }
                }
            }
            if (matchLines.OverUnder != null && matchLines.Line != null)
                matchLines.ImpliedTeamTotal = matchLines.OverUnder / 2 - matchLines.Line / 2;
            return matchLines;
        }

        private static string FormatTime(string time, string date)
        {
            var ind = time.IndexOf(':');
            if (ind > 0)
            {
                var sub = time[..ind];
                var gameTime = " " + (int.Parse(sub) + 12).ToString() + ":00";
                return date + gameTime;

            }
            return string.Empty;
        }

        private async Task<WeatherAPIRoot?> GetWeatherAPI(string zip)
        {
            var url = string.Format("{0}?key={1}&q={2}&days={3}&aqi=no&alerts=no", _weatherAPI.WeatherAPIURL, _weatherAPI.WeatherAPIKey, zip, _weatherAPI.ForecastDays);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/json");
            var client = clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<WeatherAPIRoot>(responseStream, options.Options);
            }
            return new();
        }

        private async Task<List<NFLOddsRoot>> GetNFLOdds()
        {
            var url = string.Format("{0}&apiKey={1}", _oddsAPI.NFLOddsAPIURL, _oddsAPI.NFLOddsAPIKey);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/json");
            var client = clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                var odds = await JsonSerializer.DeserializeAsync<List<NFLOddsRoot>>(responseStream, options.Options);
                if (odds != null) 
                {
                    cache.Set(Cache.NFLOdds.ToString(), odds);
                    return odds;
                }
            }
            return Enumerable.Empty<NFLOddsRoot>().ToList();
        }

    }
}
