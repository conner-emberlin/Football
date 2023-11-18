using Football.Models;
using Football.Enums;
using Football.News.Interfaces;
using Football.News.Models;
using Football.Players.Interfaces;
using Football.Fantasy.Analysis.Interfaces;
using Microsoft.Extensions.Options;
using Serilog;
using Football.Fantasy.Analysis.Models;
using Football.Players.Models;
using Football.Fantasy.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Football.Fantasy.Analysis.Services
{
    public class StartOrSitService : IStartOrSitService
    {
        private readonly INewsService _newsService;
        private readonly IPlayersService _playersService;
        private readonly IMatchupAnalysisService _matchupAnalysisService;
        private readonly IFantasyDataService _fantasyDataService;
        private readonly ISettingsService _settingsService;
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;
        private readonly Season _season;
        private readonly NFLOddsAPI _oddsAPI;
        public StartOrSitService(INewsService newsService, IPlayersService playersService, IMatchupAnalysisService matchupAnalysisService,
            IFantasyDataService fantasyDataService, ISettingsService settingsService, IMemoryCache cache,
            IOptionsMonitor<Season> season, ILogger logger, IOptionsMonitor<NFLOddsAPI> oddsAPI)
        {
            _newsService = newsService;
            _playersService = playersService;
            _matchupAnalysisService = matchupAnalysisService;
            _fantasyDataService = fantasyDataService;
            _settingsService = settingsService;
            _cache = cache;
            _season = season.CurrentValue;
            _logger = logger;
            _oddsAPI = oddsAPI.CurrentValue;
        }
        public async Task<List<StartOrSit>> GetStartOrSits(List<int> playerIds)
        {
            List<StartOrSit> startOrSits = new();
            var currentWeek = await _playersService.GetCurrentWeek(_season.CurrentSeason);
            foreach (var playerId in playerIds)
            {           
                if (_cache.TryGetValue(playerId.ToString() + Cache.StartOrSit, out StartOrSit startOrSit))
                {
                    startOrSits.Add(startOrSit);
                }
                else {
                    var team = await _playersService.GetPlayerTeam(_season.CurrentSeason, playerId);
                    if (team != null)
                    {
                        var schedule = await _playersService.GetScheduleDetails(_season.CurrentSeason, currentWeek);
                        var teamId = await _playersService.GetTeamId(team.Team);
                        var projection = await _playersService.GetWeeklyProjection(_season.CurrentSeason, currentWeek, playerId);
                        try
                        {
                            var sos = new StartOrSit
                            {
                                Player = await _playersService.GetPlayer(playerId),
                                TeamMap = await _playersService.GetTeam(teamId),
                                ScheduleDetails = schedule.FirstOrDefault(s => s.AwayTeamId == teamId || s.HomeTeamId == teamId),
                                MatchLines = await GetMatchLines(playerId),
                                Weather = await GetWeather(playerId),
                                MatchupRanking = await _matchupAnalysisService.GetMatchupRanking(playerId),
                                ProjectedPoints = Math.Round(projection, 2),
                                PlayerComparisons = await GetPlayerComparisons(playerId)
                            };
                            _cache.Set(playerId.ToString() + Cache.StartOrSit, sos);
                            startOrSits.Add(sos);

                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex.Message, ex.StackTrace, ex.ToString(), ex.InnerException);
                            throw;
                        }
                    }
                    else
                    {
                        _logger.Information("PlayerId {0} has not been assigned a team.", playerId);
                    }
                }

            }
            if (startOrSits.Any())
            {
                var starter = startOrSits.First(s => s.ProjectedPoints == startOrSits.Max(s => s.ProjectedPoints)).Player.PlayerId;
                startOrSits.ForEach(s => s.Start = s.Player.PlayerId == starter);
                return startOrSits;
            }
            else
            {
                return Enumerable.Empty<StartOrSit>().ToList();
            }

        }

        public async Task<Weather> GetWeather(int playerId)
        {
            var currentWeek = await _playersService.GetCurrentWeek(_season.CurrentSeason);
            var playerTeam = await _playersService.GetPlayerTeam(_season.CurrentSeason, playerId);
            if (playerTeam != null)
            {
                var playerTeamId = await _playersService.GetTeamId(playerTeam.Team);
                var allScheduleDetails = await _playersService.GetScheduleDetails(_season.CurrentSeason, currentWeek);
                var scheduleDetail = allScheduleDetails.FirstOrDefault(s => s.HomeTeamId == playerTeamId || s.AwayTeamId == playerTeamId);
                if (scheduleDetail != null)
                {
                    var homeLocation = await _playersService.GetTeamLocation(scheduleDetail.HomeTeamId);
                    if (homeLocation.Indoor == 1)
                    {
                        return new Weather
                        {
                            GameTime = FormatTime(scheduleDetail.Time, scheduleDetail.Date),
                            Temperature = "Indoor"
                        };
                    }
                    var forecast = await _newsService.GetWeatherAPI(homeLocation.Zip);
                    if (forecast != null)
                    {
                        var forecastDay = forecast.forecast.forecastday.FirstOrDefault(f => f.date == scheduleDetail.Date);
                        if (forecastDay != null)
                        {
                            var time = FormatTime(scheduleDetail.Time, scheduleDetail.Date);
                            var forecastHour = forecastDay.hour.First(h => h.time == time);
                            return forecastHour != null ? Weather(forecastHour) : new Weather { };
                        }
                        else
                        {
                            return new Weather { };
                        }
                    }
                    else
                    {
                        _logger.Information("Unable to find forecast for zip {0}", homeLocation.Zip);
                        return new Weather { };
                    }
                }
                else 
                { 
                    _logger.Information("Team {0} is on BYE", playerTeam.Team); 
                    return new Weather { }; 
                }

            }
            else
            {
                _logger.Information("{0} is not assigned to a team. Unable to retrieve forecast.", playerId);
                return new Weather { };
            }
        }

        public async Task<List<PlayerComparison>> GetPlayerComparisons(int playerId)
        {
            List<PlayerComparison> comparisons = new();
            var player = await _playersService.GetPlayer(playerId);            
            var team = await _playersService.GetPlayerTeam(_season.CurrentSeason, playerId);            
            if (team != null)
            {
                var teamId = await _playersService.GetTeamId(team.Team);
                var currentWeek = await _playersService.GetCurrentWeek(_season.CurrentSeason);
                var schedule = (await _playersService.GetScheduleDetails(_season.CurrentSeason, currentWeek))
                               .FirstOrDefault(s => s.AwayTeamId == teamId || s.HomeTeamId == teamId);
                var playerProjection = await _playersService.GetWeeklyProjection(_season.CurrentSeason, currentWeek, player.PlayerId);
                if (schedule != null && playerProjection > 0)
                {       
                    var opponentId = schedule.HomeTeamId == teamId ? schedule.AwayTeamId : schedule.HomeTeamId;
                    var opponentSchedule = (await _playersService.GetTeamGames(opponentId)).Where(s => s.Week < currentWeek);
                    foreach (var match in opponentSchedule)
                    {
                        var otherPlayers = await _playersService.GetPlayersByTeam(match.OpposingTeam);
                        if (otherPlayers != null)
                        {
                            foreach(var op in otherPlayers)
                            {
                                var otherPlayer = await _playersService.GetPlayer(op.PlayerId);
                                _ = Enum.TryParse(otherPlayer.Position, out Position otherPlayerPosition);
                                var otherProjectedPoints = await _playersService.GetWeeklyProjection(_season.CurrentSeason, match.Week, otherPlayer.PlayerId);
                                if (otherPlayer.Position == player.Position && Math.Abs(playerProjection - otherProjectedPoints) <= _settingsService.GetPlayerComparison(otherPlayerPosition))
                                {
                                    var weeklyFantasy = await _fantasyDataService.GetWeeklyFantasy(otherPlayer.PlayerId);
                                    comparisons.Add(new PlayerComparison
                                    {
                                        Player = otherPlayer,
                                        Team = match.OpposingTeam,
                                        Schedule = match,
                                        WeeklyFantasy = weeklyFantasy.FirstOrDefault(w => w.Week == match.Week),
                                        ProjectedPoints = otherProjectedPoints
                                    });
                                }
                            }
                        }
                    }


                }
            }
            return comparisons;
        }
        public async Task<MatchLines> GetMatchLines(int playerId)
        {
            var team = await _playersService.GetPlayerTeam(_season.CurrentSeason, playerId);
            if (team != null)
            {
                var teamId = await _playersService.GetTeamId(team.Team);
                var teamMap = await _playersService.GetTeam(teamId);
                var odds = await _newsService.GetNFLOdds();
                var teamOdds = odds.FirstOrDefault(o => o.home_team == teamMap.TeamDescription || o.away_team == teamMap.TeamDescription);
                if (teamOdds != null)
                {
                    var bookmaker = teamOdds.bookmakers.FirstOrDefault(b => b.key == _oddsAPI.DefaultBookmaker);
                    return bookmaker != null ? MatchLines(bookmaker, teamMap) : new MatchLines { };
                }
                else
                {
                    return new MatchLines { };
                }
            }
            else
            {
                return new MatchLines { };
            }
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
                            {
                                if (outcome.name == teamMap.TeamDescription)
                                {
                                    matchLines.Line = outcome.point;
                                }
                            }                                                      
                        break;
                        case Enums.Market.totals: 
                            if (market.outcomes.Any())
                            {
                                matchLines.OverUnder = market.outcomes.First().point;
                            }   
                        break;
                        default: break;
                    }
                }
            }
            if (matchLines.OverUnder != null && matchLines.Line != null)
            {
                matchLines.ImpliedTeamTotal = matchLines.OverUnder / 2 - matchLines.Line / 2;
            }
            return matchLines;
        }

        private static Weather Weather(Hour hour) =>
            new() {
                GameTime = hour.time,
                Temperature = string.Format("{0}°F", hour.temp_f),
                Condition = hour.condition.text,
                ConditionURL = string.Format("https{0}", hour.condition.icon),
                Wind = string.Format("{0} mph winds", hour.wind_mph),
                RainChance = string.Format("{0}% chance of rain", hour.chance_of_rain),
                SnowChance = string.Format("{0}% chance of snow", hour.chance_of_snow)
                };

        private static string FormatTime(string time, string date)
        {
            var ind = time.IndexOf(":");
            if (ind > 0)
            {
                var sub = time[..ind];
                return sub.Length == 1 ? date + " 0" + sub + ":00" : date + " " + sub + ":00";
            }
            else
            {
                return string.Empty;
            }
        }

    }
}
