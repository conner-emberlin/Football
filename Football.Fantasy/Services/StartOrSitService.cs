using Football.Models;
using Football.Enums;
using Football.News.Interfaces;
using Football.News.Models;
using Football.Players.Interfaces;
using Microsoft.Extensions.Options;
using Football.Players.Models;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Football.Fantasy.Services
{
    public class StartOrSitService(INewsService newsService, IPlayersService playersService, IMatchupAnalysisService matchupAnalysisService,
        IFantasyDataService fantasyDataService, ISettingsService settingsService, IMemoryCache cache,
        IOptionsMonitor<Season> season, IOptionsMonitor<NFLOddsAPI> oddsAPI) : IStartOrSitService
    {
        private readonly Season _season = season.CurrentValue;
        private readonly NFLOddsAPI _oddsAPI = oddsAPI.CurrentValue;

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
                var forecast = await newsService.GetWeatherAPI(homeLocation.Zip);
                if (forecast != null)
                {
                    var forecastDay = forecast.forecast.forecastday.FirstOrDefault(f => f.date == scheduleDetail.Date);
                    if (forecastDay != null)
                    {
                        var time = FormatTime(scheduleDetail.Time, scheduleDetail.Date);
                        var forecastHour = forecastDay.hour.First(h => h.time == time);
                        return forecastHour != null ? Weather(forecastHour) : new ();
                    }
                }
            }
            return new ();
        }

        private async Task<List<PlayerComparison>> GetPlayerComparisons(Player player, int currentWeek, ScheduleDetails schedule, int teamId, double playerProjection)
        {
            List<PlayerComparison> comparisons = [];
            if (schedule == null || playerProjection == 0) return comparisons;

            var opponentId = schedule.HomeTeamId == teamId ? schedule.AwayTeamId : schedule.HomeTeamId;
            var opponentSchedule = (await playersService.GetTeamGames(opponentId)).Where(s => s.Week < currentWeek && s.OpposingTeamId > 0);
            foreach (var match in opponentSchedule)
            {
                foreach (var op in await playersService.GetPlayersByTeam(match.OpposingTeam))
                {
                    var otherPlayer = await playersService.GetPlayer(op.PlayerId);
                    if (Enum.TryParse(otherPlayer.Position, out Position otherPlayerPosition) && otherPlayer.Position == player.Position)
                    {
                        var otherProjectedPoints = await playersService.GetWeeklyProjection(_season.CurrentSeason, match.Week, otherPlayer.PlayerId);
                        if (Math.Abs(playerProjection - otherProjectedPoints) <= settingsService.GetPlayerComparison(otherPlayerPosition))
                        {
                            var weeklyFantasy = await fantasyDataService.GetWeeklyFantasy(otherPlayer.PlayerId);
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
            return comparisons;
        }
        private async Task<MatchLines> GetMatchLines(TeamMap teamMap)
        {
            var odds = await newsService.GetNFLOdds();
            var teamOdds = odds.FirstOrDefault(o => o.home_team == teamMap.TeamDescription || o.away_team == teamMap.TeamDescription);
            if (teamOdds != null)
            {
                var bookmaker = teamOdds.bookmakers.FirstOrDefault(b => b.key == _oddsAPI.DefaultBookmaker);
                return bookmaker != null ? MatchLines(bookmaker, teamMap) : new ();
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
                            {
                                if (outcome.name == teamMap.TeamDescription)
                                    matchLines.Line = outcome.point;
                            }                                                      
                        break;
                        case Enums.Market.totals: 
                            if (market.outcomes.Count > 0)
                                matchLines.OverUnder = market.outcomes.First().point;
                        break;
                        default: break;
                    }
                }
            }
            if (matchLines.OverUnder != null && matchLines.Line != null)
                matchLines.ImpliedTeamTotal = matchLines.OverUnder / 2 - matchLines.Line / 2;
            return matchLines;
        }

        private static Weather Weather(Hour hour) =>
            new() {
                GameTime = hour.time,
                Temperature = string.Format("{0}°F", hour.temp_f),
                Condition = hour.condition.text,
                ConditionURL = string.Format("https:{0}", hour.condition.icon),
                Wind = string.Format("{0} mph winds", hour.wind_mph),
                RainChance = string.Format("{0}% chance of rain", hour.chance_of_rain),
                SnowChance = string.Format("{0}% chance of snow", hour.chance_of_snow)
                };

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

    }
}
