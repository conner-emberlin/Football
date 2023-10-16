using Football.Models;
using Football.Enums;
using Football.News.Interfaces;
using Football.News.Models;
using Football.Players.Interfaces;
using Football.Fantasy.Interfaces;
using Microsoft.Extensions.Options;
using Serilog;
using System.Net.Http.Headers;
using Football.Fantasy.Models;
using Football.Players.Models;

namespace Football.Fantasy.Services
{
    public class StartOrSitService : IStartOrSitService
    {
        private readonly INewsService _newsService;
        private readonly IPlayersService _playersService;
        private readonly ILogger _logger;
        private readonly Season _season;
        private readonly NFLOddsAPI _oddsAPI;
        public StartOrSitService(INewsService newsService, IPlayersService playersService, IOptionsMonitor<Season> season, ILogger logger, IOptionsMonitor<NFLOddsAPI> oddsAPI)
        {
            _newsService = newsService;
            _playersService = playersService;
            _season = season.CurrentValue;
            _logger = logger;
            _oddsAPI = oddsAPI.CurrentValue;
        }

        public async Task<Hour> GetGamedayForecast(int playerId)
        {
            var currentWeek = await _playersService.GetCurrentWeek(_season.CurrentSeason);
            var playerTeam = await _playersService.GetPlayerTeam(_season.CurrentSeason, playerId);
            if (playerTeam != null)
            {
                var playerTeamId = await _playersService.GetTeamId(playerTeam.Team);
                var allScheduleDetails = await _playersService.GetScheduleDetails(_season.CurrentSeason, currentWeek);
                var scheduleDetail = allScheduleDetails.Where(s => s.HomeTeamId == playerTeamId || s.AwayTeamId == playerTeamId).First();
                var homeLocation = await _playersService.GetTeamLocation(scheduleDetail.HomeTeamId);
                var forecast = await _newsService.GetWeatherAPI(homeLocation.Zip);
                if(forecast != null)
                {
                    var forecastDay = forecast.forecast.forecastday.Where(f => f.date == scheduleDetail.Date).First();
                    var time = FormatTime(scheduleDetail.Time, scheduleDetail.Date);
                    var forecastHour = forecastDay.hour.Where(h => h.time == time).First();
                    return forecastHour ?? new Hour { };
                }
                else
                {
                    _logger.Information("Unable to find forecast for zip {0}", homeLocation.Zip);
                    return new Hour { };
                }

            }
            else
            {
                _logger.Information("{0} is not assigned to a team. Unable to retrieve forecast.", playerId);
                return new Hour { };
            }
        }

        public async Task<MatchLines> GetMatchLines(int playerId)
        {
            var team = await _playersService.GetPlayerTeam(_season.CurrentSeason, playerId);
            if (team != null)
            {
                var teamId = await _playersService.GetTeamId(team.Team);
                var teamMap = await _playersService.GetTeam(teamId);
                var odds = await _newsService.GetNFLOdds();
                var teamOdds = odds.Where(o => o.home_team == teamMap.TeamDescription || o.away_team == teamMap.TeamDescription).FirstOrDefault();
                if (teamOdds != null)
                {
                    var bookmaker = teamOdds.bookmakers.Where(b => b.key == _oddsAPI.DefaultBookmaker).FirstOrDefault();
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
               if (Enum.TryParse(market.key, out MarketsEnum marketEnum))
                {
                    switch (marketEnum)
                    {
                        case MarketsEnum.spreads: 
                            foreach (var outcome in market.outcomes)
                            {
                                if (outcome.name == teamMap.TeamDescription)
                                {
                                    matchLines.Line = outcome.point;
                                }
                            }                                                      
                        break;
                        case MarketsEnum.totals: 
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
