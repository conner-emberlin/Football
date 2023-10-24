using Football.Enums;
using Football.Fantasy.Models;
using Football.Fantasy.Interfaces;
using Football.Statistics.Interfaces;
using Football.Players.Interfaces;
using Serilog;
using Microsoft.Extensions.Caching.Memory;
using Football.Data.Models;
using Football.Models;
using Microsoft.Extensions.Options;

namespace Football.Fantasy.Services
{
    public class FantasyDataService : IFantasyDataService
    {
        private readonly IFantasyDataRepository _fantasyData;
        private readonly IFantasyCalculator _calculator;
        private readonly IStatisticsService _statisticsService;
        private readonly IPlayersService _playersService;
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;
        private readonly ISettingsService _settingsService;
        private readonly Season _season;
        public FantasyDataService(IFantasyDataRepository fantasyData, IFantasyCalculator calculator, 
            IStatisticsService statistics, IPlayersService playersService, ILogger logger, 
            IMemoryCache cache, IOptionsMonitor<Season> season, ISettingsService settingsService)
        {
            _fantasyData = fantasyData;
            _calculator = calculator;
            _statisticsService = statistics;
            _playersService = playersService;
            _logger = logger;
            _cache = cache;
            _season = season.CurrentValue;
            _settingsService = settingsService;
        }
        public async Task<List<SeasonFantasy>> GetSeasonFantasy(int playerId) => await _fantasyData.GetSeasonFantasy(playerId);       
        public async Task<int> PostSeasonFantasy(int season, PositionEnum position)
        {
            List<SeasonFantasy> seasonFantasy = new();
            switch (position)
            {
                case PositionEnum.QB: 
                    foreach(var data in await _statisticsService.GetSeasonData<SeasonDataQB>(position, season, false))
                    {
                        seasonFantasy.Add(_calculator.CalculateFantasy(data));
                    }
                    break;
                case PositionEnum.RB:
                    foreach (var data in await _statisticsService.GetSeasonData<SeasonDataRB>(position, season, false))
                    {
                        seasonFantasy.Add(_calculator.CalculateFantasy(data));
                    }
                    break;
                case PositionEnum.WR:
                    foreach (var data in await _statisticsService.GetSeasonData<SeasonDataWR>(position, season, false))
                    {
                        seasonFantasy.Add(_calculator.CalculateFantasy(data));
                    }
                    break;
                case PositionEnum.TE:
                    foreach (var data in await _statisticsService.GetSeasonData<SeasonDataTE>(position, season, false))
                    {
                        seasonFantasy.Add(_calculator.CalculateFantasy(data));
                    }
                    break;
                default: throw new NotImplementedException();
            }
            return await _fantasyData.PostSeasonFantasy(seasonFantasy);
        }
        public async Task<int> PostWeeklyFantasy(int season, int week, PositionEnum position)
        {
            List<WeeklyFantasy> weeklyFantasy = new();
            switch (position)
            {
                case PositionEnum.QB: 
                    foreach(var data in await _statisticsService.GetWeeklyData<WeeklyDataQB>(position, season, week))
                    {
                        weeklyFantasy.Add(_calculator.CalculateFantasy(data));
                    }
                    break;
                case PositionEnum.RB:
                    foreach (var data in await _statisticsService.GetWeeklyData<WeeklyDataRB>(position, season, week))
                    {
                        weeklyFantasy.Add(_calculator.CalculateFantasy(data));
                    }
                    break;
                case PositionEnum.WR:
                    foreach (var data in await _statisticsService.GetWeeklyData<WeeklyDataWR>(position, season, week))
                    {
                        weeklyFantasy.Add(_calculator.CalculateFantasy(data));
                    }
                    break;
                case PositionEnum.TE:
                    foreach (var data in await _statisticsService.GetWeeklyData<WeeklyDataTE>(position, season, week))
                    {
                        weeklyFantasy.Add(_calculator.CalculateFantasy(data));
                    }
                    break;
                case PositionEnum.DST:
                    var stats = await _statisticsService.GetWeeklyData<WeeklyDataDST>(position, season, week);
                    var teams = await _playersService.GetAllTeams();
                    foreach (var data in stats)
                    {
                        var teamId = await _playersService.GetTeamId(data.PlayerId);
                        var gameResult = (await _statisticsService.GetGameResults(season)).First(g => g.Week == week && (g.WinnerId == teamId || g.LoserId == teamId));
                        var opponent = gameResult.WinnerId == teamId ? gameResult.LoserId : gameResult.WinnerId;
                        var opponentPID = teams.First(t => t.TeamId == opponent).PlayerId;
                        var opponentStat = stats.First(s => s.PlayerId == opponentPID);

                        if(teamId > 0 && gameResult != null) 
                        {
                            weeklyFantasy.Add(_calculator.CalculateFantasy(data, opponentStat, gameResult, teamId));                               
                        }
                    }
                    break;
                default: throw new NotImplementedException();
            }
            return await _fantasyData.PostWeeklyFantasy(weeklyFantasy);
        }
        public async Task<List<WeeklyFantasy>> GetWeeklyFantasy(int playerId) => await _fantasyData.GetWeeklyFantasy(playerId);
        public async Task<List<WeeklyFantasy>> GetWeeklyFantasy(int season, int week) => await _fantasyData.GetWeeklyFantasy(season, week);
        public async Task<List<WeeklyFantasy>> GetWeeklyFantasy(PositionEnum position)
        {
            var currentWeek = await _playersService.GetCurrentWeek(_season.CurrentSeason);
            List<WeeklyFantasy> weeklyFantasy = new();
            for (int i = 1; i < currentWeek; i++)
            {
                foreach (var s in await GetWeeklyFantasy(_season.CurrentSeason, i))
                {
                    if (s.Position == position.ToString())
                    {
                        weeklyFantasy.Add(s);
                    }                   
                }
            }
            return weeklyFantasy;
        }
        public async Task<List<SeasonFantasy>> GetCurrentFantasyTotals(int season)
        {
            if (_settingsService.GetFromCache<SeasonFantasy>(Cache.SeasonTotals, out var cachedTotals))
            {
                return cachedTotals;
            }
            else
            {
                var currentWeek = await _playersService.GetCurrentWeek(season);
                List<WeeklyFantasy> weeklyFantasy = new();
                for (int i = 1; i < currentWeek; i++)
                {
                    foreach (var s in await GetWeeklyFantasy(season, i))
                    {
                        weeklyFantasy.Add(s);
                    }
                }
                var leaders = weeklyFantasy.GroupBy(w => w.PlayerId)
                                    .Select(gw => new SeasonFantasy
                                    {
                                        PlayerId = gw.Key,
                                        Season = season,
                                        Games = gw.Count(),
                                        FantasyPoints = Math.Round(gw.Sum(f => f.FantasyPoints), 2),
                                        Name = gw.Select(g => g.Name).First(),
                                        Position = gw.Select(g => g.Position).First()
                                    }).OrderByDescending(w => w.FantasyPoints).ToList();
                _cache.Set(Cache.SeasonTotals.ToString(), leaders);
                return leaders;
            }
        }
    }
}
