using Football.Enums;
using Football.Fantasy.Models;
using Football.Fantasy.Interfaces;
using Football.Players.Interfaces;
using Serilog;
using Microsoft.Extensions.Caching.Memory;
using Football.Players.Models;
using Football.Models;
using Microsoft.Extensions.Options;

namespace Football.Fantasy.Services
{
    public class FantasyDataService(IFantasyDataRepository fantasyData, IFantasyCalculator calculator,
        IStatisticsService statistics, IPlayersService playersService,
        IMemoryCache cache, IOptionsMonitor<Season> season, ISettingsService settingsService, ILogger logger) : IFantasyDataService
    {
        private readonly Season _season = season.CurrentValue;

        public async Task<List<SeasonFantasy>> GetSeasonFantasy(int playerId) => await fantasyData.GetSeasonFantasy(playerId);
        public async Task<List<SeasonFantasy>> GetAllSeasonFantasyByPosition(Position position) => await fantasyData.GetAllSeasonFantasyByPosition(position.ToString());
        public async Task<List<WeeklyFantasy>> GetWeeklyFantasy(int playerId) => await fantasyData.GetWeeklyFantasyByPlayer(playerId, _season.CurrentSeason);
        public async Task<List<WeeklyFantasy>> GetWeeklyFantasy(int season, int week) => await fantasyData.GetWeeklyFantasy(season, week);
        public async Task<List<WeeklyFantasy>> GetAllWeeklyFantasyByPosition(Position position) => await fantasyData.GetAllWeeklyFantasyByPosition(position.ToString());
        public async Task<int> PostSeasonFantasy(int season, Position position)
        {
            List<SeasonFantasy> seasonFantasy = [];
            switch (position)
            {
                case Position.QB: 
                    foreach(var data in await statistics.GetSeasonData<SeasonDataQB>(position, season, false))
                        seasonFantasy.Add(calculator.CalculateFantasy(data));
                    break;
                case Position.RB:
                    foreach (var data in await statistics.GetSeasonData<SeasonDataRB>(position, season, false))
                        seasonFantasy.Add(calculator.CalculateFantasy(data));
                    break;
                case Position.WR:
                    foreach (var data in await statistics.GetSeasonData<SeasonDataWR>(position, season, false))
                        seasonFantasy.Add(calculator.CalculateFantasy(data));
                    break;
                case Position.TE:
                    foreach (var data in await statistics.GetSeasonData<SeasonDataTE>(position, season, false))
                        seasonFantasy.Add(calculator.CalculateFantasy(data));
                    break;
                case Position.DST:
                case Position.K:
                    var weeklyFantasy = (await fantasyData.GetWeeklyFantasy(season, 0)).Where(w => w.Position == position.ToString())
                                        .GroupBy(f => f.PlayerId, f => f, (key, f) => new { PlayerId = key, Fantasy = f.ToList() });
                    foreach (var group in weeklyFantasy)
                    {
                        var total = 0.0;
                        foreach (var week in group.Fantasy)
                            total += week.FantasyPoints;
                        seasonFantasy.Add(new SeasonFantasy
                        {
                            PlayerId = group.PlayerId,
                            Season = _season.CurrentSeason - 1,
                            Games = _season.Games,
                            FantasyPoints = total,
                            Name = group.Fantasy.First().Name,
                            Position = position.ToString()
                        }) ;
                    }
                    break;

                default: break;
            }
            return await fantasyData.PostSeasonFantasy(seasonFantasy);
        }
        public async Task<int> PostWeeklyFantasy(int season, int week, Position position)
        {
            List<WeeklyFantasy> weeklyFantasy = [];
            logger.Information("Upoading week {0} fantasy for position {1}", week, position.ToString());
            switch (position)
            {
                case Position.QB: 
                    foreach(var data in await statistics.GetWeeklyData<WeeklyDataQB>(position, season, week))
                        weeklyFantasy.Add(calculator.CalculateFantasy(data));
                    break;
                case Position.RB:
                    foreach (var data in await statistics.GetWeeklyData<WeeklyDataRB>(position, season, week))
                        weeklyFantasy.Add(calculator.CalculateFantasy(data));
                    break;
                case Position.WR:
                    foreach (var data in await statistics.GetWeeklyData<WeeklyDataWR>(position, season, week))
                        weeklyFantasy.Add(calculator.CalculateFantasy(data));
                    break;
                case Position.TE:
                    foreach (var data in await statistics.GetWeeklyData<WeeklyDataTE>(position, season, week))
                        weeklyFantasy.Add(calculator.CalculateFantasy(data));
                    break;
                case Position.K:
                    foreach (var data in await statistics.GetWeeklyData<WeeklyDataK>(position, season, week))
                        weeklyFantasy.Add(calculator.CalculateFantasy(data));
                    break;
                case Position.DST:
                    var stats = await statistics.GetWeeklyData<WeeklyDataDST>(position, season, week);
                    var teams = await playersService.GetAllTeams();
                    foreach (var data in stats)
                    {
                        var teamId = await playersService.GetTeamId(data.PlayerId);
                        var gameResult = (await statistics.GetGameResults(season)).First(g => g.Week == week && (g.WinnerId == teamId || g.LoserId == teamId));
                        var opponent = gameResult.WinnerId == teamId ? gameResult.LoserId : gameResult.WinnerId;
                        var opponentPID = teams.First(t => t.TeamId == opponent).PlayerId;
                        var opponentStat = stats.First(s => s.PlayerId == opponentPID);

                        if(teamId > 0 && gameResult != null) 
                            weeklyFantasy.Add(calculator.CalculateFantasy(data, opponentStat, gameResult, teamId));
                    }
                    break;
                default: break;
            }
            var added = await fantasyData.PostWeeklyFantasy(weeklyFantasy);
            logger.Information("{0} fantasy upload complete. {1} records added", position.ToString(), added);
            return added;
        }
        public async Task<List<WeeklyFantasy>> GetWeeklyFantasy(int playerId, string team)
        {
            var allFantasy = await GetWeeklyFantasy(playerId);
            var teamChanges = await playersService.GetInSeasonTeamChanges();
            if (teamChanges.Any(t => t.PlayerId == playerId))
            {
                List<WeeklyFantasy> filteredFantasy = [];
                var teamChange = teamChanges.First(t => t.PlayerId == playerId);
                foreach (var fantasy in allFantasy)
                {
                    if (teamChange.PreviousTeam == team && fantasy.Week < teamChange.WeekEffective)
                        filteredFantasy.Add(fantasy);
                    else if(teamChange.NewTeam == team && fantasy.Week >= teamChange.WeekEffective)
                        filteredFantasy.Add(fantasy);
                }
                return filteredFantasy;
            }
            else return allFantasy;
        }

        public async Task<List<WeeklyFantasy>> GetWeeklyTeamFantasy(string team, int week)
        {
            List<WeeklyFantasy> teamFantasy = [];
            var players = await playersService.GetPlayersByTeam(team);
            foreach (var player in players)
            {
                var weeklyFantasy = (await GetWeeklyFantasy(player.PlayerId, team)).FirstOrDefault(f => f.Week == week);
                if(weeklyFantasy != null) teamFantasy.Add(weeklyFantasy);
            }
            return teamFantasy;
        }
        
        public async Task<List<WeeklyFantasy>> GetWeeklyFantasy(Position position)
        {
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            List<WeeklyFantasy> weeklyFantasy = [];
            for (int i = 1; i < currentWeek; i++)
            {
                foreach (var s in await GetWeeklyFantasy(_season.CurrentSeason, i))
                    if (s.Position == position.ToString()) weeklyFantasy.Add(s);
            }
            return weeklyFantasy;
        }
        public async Task<List<SeasonFantasy>> GetCurrentFantasyTotals(int season)
        {
            if (settingsService.GetFromCache<SeasonFantasy>(Cache.SeasonTotals, out var cachedTotals))
                return cachedTotals;
            else
            {
                var currentWeek = await playersService.GetCurrentWeek(season);
                List<WeeklyFantasy> weeklyFantasy = [];
                for (int i = 1; i < currentWeek; i++) weeklyFantasy.AddRange(await GetWeeklyFantasy(season, i));
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
                cache.Set(Cache.SeasonTotals.ToString(), leaders);
                return leaders;
            }
        }
    }
}
