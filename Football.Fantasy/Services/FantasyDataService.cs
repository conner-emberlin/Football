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
        private readonly Season _season;
        public FantasyDataService(IFantasyDataRepository fantasyData, IFantasyCalculator calculator, 
            IStatisticsService statistics, IPlayersService playersService, ILogger logger, 
            IMemoryCache cache, IOptionsMonitor<Season> season)
        {
            _fantasyData = fantasyData;
            _calculator = calculator;
            _statisticsService = statistics;
            _playersService = playersService;
            _logger = logger;
            _cache = cache;
            _season = season.CurrentValue;
        }
        public async Task<SeasonFantasy> GetSeasonFantasy(int playerId, int season) => await _fantasyData.GetSeasonFantasy(playerId, season);
        public async Task<List<SeasonFantasy>> GetSeasonFantasy(int playerId) => await _fantasyData.GetSeasonFantasy(playerId);
        public async Task<int> PostSeasonFantasy(int season, PositionEnum position)
        {
            List<SeasonFantasy> seasonFantasy = new();
            switch (position)
            {
                case PositionEnum.QB: 
                    foreach(var data in await _statisticsService.GetSeasonData<SeasonDataQB>(position, season, false))
                    {
                        seasonFantasy.Add(_calculator.CalculateQBFantasy(data));
                    }
                    break;
                case PositionEnum.RB:
                    foreach (var data in await _statisticsService.GetSeasonData<SeasonDataRB>(position, season, false))
                    {
                        seasonFantasy.Add(_calculator.CalculateRBFantasy(data));
                    }
                    break;
                case PositionEnum.WR:
                    foreach (var data in await _statisticsService.GetSeasonData<SeasonDataWR>(position, season, false))
                    {
                        seasonFantasy.Add(_calculator.CalculateWRFantasy(data));
                    }
                    break;
                case PositionEnum.TE:
                    foreach (var data in await _statisticsService.GetSeasonData<SeasonDataTE>(position, season, false))
                    {
                        seasonFantasy.Add(_calculator.CalculateTEFantasy(data));
                    }
                    break;
                default: _logger.Error("Invalid position {position}", position.ToString()); break;
            }
            var count = 0;
            foreach(var fp in seasonFantasy)
            {
               count += await  _fantasyData.PostSeasonFantasy(fp);
            }
            return count;
        }
        public async Task<int> PostWeeklyFantasy(int season, int week, PositionEnum position)
        {
            List<WeeklyFantasy> weeklyFantasy = new();
            switch (position)
            {
                case PositionEnum.QB: 
                    foreach(var data in await _statisticsService.GetWeeklyData<WeeklyDataQB>(position, season, week))
                    {
                        weeklyFantasy.Add(_calculator.CalculateQBFantasy(data));
                    }
                    break;
                case PositionEnum.RB:
                    foreach (var data in await _statisticsService.GetWeeklyData<WeeklyDataRB>(position, season, week))
                    {
                        weeklyFantasy.Add(_calculator.CalculateRBFantasy(data));
                    }
                    break;
                case PositionEnum.WR:
                    foreach (var data in await _statisticsService.GetWeeklyData<WeeklyDataWR>(position, season, week))
                    {
                        weeklyFantasy.Add(_calculator.CalculateWRFantasy(data));
                    }
                    break;
                case PositionEnum.TE:
                    foreach (var data in await _statisticsService.GetWeeklyData<WeeklyDataTE>(position, season, week))
                    {
                        weeklyFantasy.Add(_calculator.CalculateTEFantasy(data));
                    }
                    break;
                case PositionEnum.DST:
                    var stats = await _statisticsService.GetWeeklyData<WeeklyDataDST>(position, season, week);
                    var teams = await _playersService.GetAllTeams();
                    foreach (var data in stats)
                    {
                        var teamId = await _playersService.GetTeamId(data.PlayerId);
                        var gameResult = (await _statisticsService.GetGameResults(season, week)).Where(g => g.WinnerId == teamId || g.LoserId == teamId).First();
                        var opponent = gameResult.WinnerId == teamId ? gameResult.LoserId : gameResult.WinnerId;
                        var opponentPID = teams.Where(t => t.TeamId == opponent).First().PlayerId;
                        var opponentStat = stats.Where(s => s.PlayerId == opponentPID).First();

                        if(teamId > 0 && gameResult != null) 
                        {
                            weeklyFantasy.Add(_calculator.CalculateDSTFantasy(data, opponentStat, gameResult, teamId));                               
                        }
                    }
                    break;
                default: _logger.Error("Invalid position {position}", position.ToString()); break;
            }
            var count = 0;
            foreach(var fp in weeklyFantasy)
            {
                count += await _fantasyData.PostWeeklyFantasy(fp);
            }
            return count;
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
            if (RetrieveFromCache().Any())
            {
                return RetrieveFromCache();
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
                _cache.Set("SeasonTotals", leaders);
                return leaders;
            }
        }

        private List<SeasonFantasy> RetrieveFromCache() =>
             _cache.TryGetValue("SeasonTotals", out List<SeasonFantasy> cachedTotals) ? cachedTotals
             : Enumerable.Empty<SeasonFantasy>().ToList();

    }
}
