using Football.Players.Interfaces;
using Football.Statistics.Interfaces;
using Football.Enums;
using Football.Data.Models;
using Football.Models;
using Microsoft.Extensions.Options;
using Football.Statistics.Models;
using Football.Players.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Football.Statistics.Services
{
    public class AdvancedStatisticsService(IStatisticsService statisticsService, IPlayersService playersService, 
        IOptionsMonitor<FiveThirtyEightValueSettings> value, IOptionsMonitor<Season> season, IMemoryCache cache, ISettingsService settingsService) : IAdvancedStatisticsService
    {
        private readonly FiveThirtyEightValueSettings _value = value.CurrentValue;
        private readonly Season _season = season.CurrentValue;

        public async Task<List<AdvancedQBStatistics>> GetAdvancedQBStatistics()
        {
            List<AdvancedQBStatistics> stats = [];
            var qbs = await playersService.GetPlayersByPosition(Position.QB);
           foreach (var qb in qbs)
            {
                stats.Add(new AdvancedQBStatistics
                {
                    PlayerId = qb.PlayerId,
                    Name = qb.Name,
                    PasserRating = await PasserRating(qb.PlayerId),
                    YardsPerPlay = await QBYardsPerPlay(qb.PlayerId),
                    FiveThirtyEightRating = await FiveThirtyEightQBValue(qb.PlayerId)
                });
            }
            return stats;
        }

        public async Task<double> FiveThirtyEightQBValue(int playerId)
        {
            var weeklyStats = await statisticsService.GetWeeklyData<WeeklyDataQB>(Position.QB, playerId);
            if (weeklyStats.Count > 0)
            {
                return Math.Round(
                         weeklyStats.Average(w => w.Attempts) * _value.PAtts
                       + weeklyStats.Average(w => w.Completions) * _value.PComps
                       + weeklyStats.Average(w => w.TD) * _value.PTds
                       + weeklyStats.Average(w => w.Int) * _value.Ints
                       + weeklyStats.Average(w => w.Sacks) * _value.Sacks
                       + weeklyStats.Average(w => w.RushingAttempts) * _value.RAtts
                       + weeklyStats.Average(w => w.RushingYards) * _value.RYds
                       + weeklyStats.Average(w => w.RushingTD) * _value.RTds);
            }
            else return 0;
        }

        public async Task<double> PasserRating(int playerId)
        {
            var weeklyStats = await statisticsService.GetWeeklyData<WeeklyDataQB>(Position.QB, playerId);
            if (weeklyStats.Count > 0)
            {
                var aActual = (weeklyStats.Average(w => w.Completions) / weeklyStats.Average(w => w.Attempts) - 0.3) * 5;
                var bActual = (weeklyStats.Average(w => w.Yards) / weeklyStats.Average(w => w.Attempts) - 3) * 0.25;
                var cActual = (weeklyStats.Average(w => w.TD) / weeklyStats.Average(w => w.TD)) * 20;
                var dActual = 2.375 - (weeklyStats.Average(w => w.Int) / (weeklyStats.Average(w => w.Attempts)) * 25);

                var a = aActual > 2.375 ? 2.375 : aActual < 0 || double.IsNaN(aActual) ? 0 : aActual;
                var b = bActual > 2.375 ? 2.375 : bActual < 0 || double.IsNaN(bActual) ? 0 : bActual;
                var c = cActual > 2.375 ? 2.375 : cActual < 0 || double.IsNaN(cActual) ? 0 : cActual;
                var d = dActual > 2.375 ? 2.375 : dActual < 0 || double.IsNaN(dActual) ? 0 : dActual;

                return ((a + b + c + d) / 6) * 100;
            }
            else return 0;
        }

        public async Task<double> QBYardsPerPlay(int playerId)
        {
            var weeklyStats = await statisticsService.GetWeeklyData<WeeklyDataQB>(Position.QB, playerId);
            if (weeklyStats.Count > 0)
            {
                var totalPlays = weeklyStats.Sum(w => w.RushingAttempts) + weeklyStats.Sum(w => w.Attempts);
                var totalYards = weeklyStats.Sum(w => w.Yards) + weeklyStats.Sum(w => w.RushingYards);
                return totalPlays > 0 ? totalYards / totalPlays : 0;
            }
            else return 0;
        }

        public async Task<double> YardsAllowedPerGame(int teamId)
        {
            var yards = 0.0;
            var gameResults = (await statisticsService.GetGameResults(_season.CurrentSeason)).Where(g => g.HomeTeamId == teamId || g.AwayTeamId == teamId);
            foreach (var g in gameResults)
                yards += g.WinnerId == teamId ? g.LoserYards : g.WinnerYards;
            return gameResults.Any() ? yards / gameResults.Count() : 0;
        }

        public async Task<double> YardsAllowed(int teamId, int week)
        {
            var gameResults = (await statisticsService.GetGameResults(_season.CurrentSeason))
                              .Where(g => (g.HomeTeamId == teamId || g.AwayTeamId == teamId) 
                                          && g.Week == week).FirstOrDefault();
            if (gameResults != null)
                return gameResults.WinnerId == teamId ? gameResults.LoserYards : gameResults.WinnerYards;
            else return 0;
        }

        public async Task<List<StrengthOfSchedule>> RemainingStrengthOfSchedule()
        {
            if (settingsService.GetFromCache<StrengthOfSchedule>(Cache.RemainingSOS, out var cachedSOS))
                return cachedSOS;
            var teams = await playersService.GetAllTeams();
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            List<StrengthOfSchedule> sos = [];
            foreach (var team in teams)
            {
                sos.Add(new StrengthOfSchedule
                {
                    TeamMap = team,
                    CurrentWeek = currentWeek,
                    Strength = await RemainingStrengthOfSchedule(team, currentWeek)
                });
            }
            cache.Set(Cache.RemainingSOS.ToString(), sos);
            return sos;
        }
        private async Task<double> RemainingStrengthOfSchedule(TeamMap teamMap, int currentWeek)
        {
            var schedule = (await playersService.GetTeamGames(teamMap.TeamId)).Where(g => g.Week >= currentWeek && g.OpposingTeamId > 0);
            var or = 0.0;
            var oor = 0.0;
            foreach (var s in schedule)
            {
                or += await TeamWinPercentage(s.OpposingTeamId);
                foreach (var oo in (await playersService.GetTeamGames(s.OpposingTeamId)).Where(g => g.Week < currentWeek && g.OpposingTeamId > 0))
                    oor += await TeamWinPercentage(oo.OpposingTeamId);
            }
            return (2 * or + oor) / 3;

        }

        public async Task<double> StrengthOfSchedule(int teamId, int atWeek)
        {
            var schedule = (await playersService.GetTeamGames(teamId)).Where(g => g.Week <= atWeek && g.OpposingTeamId > 0);
            var or = 0.0;
            var oor = 0.0;
            foreach (var s in schedule)
            {
                or += await TeamWinPercentage(s.TeamId, atWeek);
                foreach (var oo in (await playersService.GetTeamGames(s.OpposingTeamId)).Where(g => g.Week < atWeek && g.OpposingTeamId > 0))
                    oor += await TeamWinPercentage(oo.OpposingTeamId, atWeek);
            }
            return (2 * or + oor) / 3;
        }
        private async Task<double> TeamWinPercentage(int teamId)
        {
            var gameResults = (await statisticsService.GetGameResults(_season.CurrentSeason)).Where(g => g.HomeTeamId == teamId || g.AwayTeamId == teamId);
            return (double)gameResults.Where(g => g.WinnerId == teamId).Count() / (double) gameResults.Count();                          
        }

        private async Task<double> TeamWinPercentage(int teamId, int week)
        {
            var gameResults = (await statisticsService.GetGameResults(_season.CurrentSeason)).Where(g => (g.HomeTeamId == teamId || g.AwayTeamId == teamId) && g.Week <= week);
            return (double)gameResults.Where(g => g.WinnerId == teamId).Count() / (double)gameResults.Count();
        }
    }
}
