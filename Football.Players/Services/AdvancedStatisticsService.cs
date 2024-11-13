using Football.Players.Interfaces;
using Football.Enums;
using Football.Models;
using Microsoft.Extensions.Options;
using Football.Players.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Football.Players.Services
{
    public class AdvancedStatisticsService(IStatisticsService statisticsService, IPlayersService playersService, ITeamsService teamsService,
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

        public async Task<List<StrengthOfSchedule>> RemainingStrengthOfSchedule()
        {
            if (settingsService.GetFromCache<StrengthOfSchedule>(Cache.RemainingSOS, out var cachedSOS))
                return cachedSOS;
            var teams = await teamsService.GetAllTeams();
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            List<StrengthOfSchedule> sos = [];
            foreach (var team in teams)
            {
                var remainingGames = (await playersService.GetTeamGames(team.TeamId)).Where(g => g.Week >= currentWeek && g.OpposingTeamId > 0);
                
                if (remainingGames.Any())
                {
                    sos.Add(new StrengthOfSchedule
                    {
                        TeamMap = team,
                        CurrentWeek = currentWeek,
                        StrengthBCS = await RemainingStrengthOfScheduleBCS(team, remainingGames, currentWeek)
                    });
                }
            }
            var ordered = sos.OrderByDescending(s => s.StrengthBCS).ToList();
            cache.Set(Cache.RemainingSOS.ToString(), ordered);
            return ordered;
        }
        private async Task<double> RemainingStrengthOfScheduleBCS(TeamMap teamMap, IEnumerable<Schedule> remainingGames, int currentWeek)
        {
            var or = 0.0;
            var oor = 0.0;
            foreach (var s in remainingGames)
            {
                or += await TeamWinPercentage(s.OpposingTeamId);
                foreach (var oo in (await playersService.GetTeamGames(s.OpposingTeamId)).Where(g => g.Week < currentWeek && g.OpposingTeamId > 0))
                    oor += await TeamWinPercentage(oo.OpposingTeamId);
            }
            return (2 * or + oor) / 3;

        }

        private async Task<double> RemainingStrengthOfScheduleStandard(TeamMap teamMap, IEnumerable<Schedule> remainingGames, int currentWeek)
        {
            return 0;
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

        public async Task<IEnumerable<DivisionStanding>> GetStandingsByDivision(Division division)
        {
            var teams = await teamsService.GetTeamsByDivision(division);
            var teamMapDictionary = (await teamsService.GetAllTeams()).ToDictionary(t => t.TeamId, t => t.TeamDescription);
            var conferenceTeams = await teamsService.GetTeamsByConference(Enum.Parse<Conference>(teams.First().Conference));

            var allTeamRecords = await teamsService.GetTeamRecords(_season.CurrentSeason);
            var gameResults = await statisticsService.GetGameResults(_season.CurrentSeason);
            List<TeamRecord> divisionalRecords = [];
            foreach (var team in teams)
            {
                divisionalRecords.Add(await teamsService.GetTeamRecordInDivision(team.TeamId));
            }

            var winPercentages = CalculateWinPercentages(allTeamRecords, divisionalRecords);
            var teamsWithSameOverallWinPercentage = winPercentages.GroupBy(w => w.Value.Item1).Where(g => g.Count() > 1).Select(g => g.ToList());
            var orderedPercentages = winPercentages.OrderByDescending(w => w.Value.Item1).ToList();
            var divisionStandings = orderedPercentages
                                    .Select(w => new DivisionStanding
                                    {
                                        Division = division.ToString(),
                                        TeamId = w.Key,
                                        TeamDescription = teamMapDictionary[w.Key],
                                        Standing = orderedPercentages.IndexOf(w) + 1
                                    });

            var sameWinCount = teamsWithSameOverallWinPercentage.Count();
            if (sameWinCount == 0) return divisionStandings;

            Dictionary<int, List<(int, double?)>> groupHeadToHeadDictionary = [];
            foreach (var winGroup in teamsWithSameOverallWinPercentage)
            {

                for (int i = 0; i < winGroup.Count; i++)
                {
                    var teamId = winGroup.ElementAt(i).Key;
                    var h2hs = GetHeadToHeads(teamId, gameResults, winGroup.Where(t => t.Key != teamId).Select(t => t.Key));
                    groupHeadToHeadDictionary.Add(teamId, h2hs);
                }
            }


            return CalcuateDivisionStandingsWithHeadToHeads(divisionStandings.ToList(), groupHeadToHeadDictionary);
        }

        private Dictionary<int, (double, double)> CalculateWinPercentages(List<TeamRecord> allTeamRecords, List<TeamRecord> divisionalRecords)
        {
            var join = allTeamRecords.Join(divisionalRecords, a => a.TeamMap.TeamId, d => d.TeamMap.TeamId, (a, d) => new { a.TeamMap.TeamId, OverallRecord = a, DivisionalRecord = d });
            Dictionary<int, (double, double)> winPercentages = [];

            foreach (var j in join)
            {
                var totalGames = j.OverallRecord.Wins + j.OverallRecord.Losses + j.OverallRecord.Ties;
                var totalDivisionalGames = j.DivisionalRecord.Wins + j.DivisionalRecord.Losses + j.DivisionalRecord.Ties;
                var overallWinPercentage = totalGames >= 1 ? (double)j.OverallRecord.Wins / (double)totalGames : 0;
                var divisionalWinPercentage = totalDivisionalGames >= 1 ? (double)j.DivisionalRecord.Wins / (double)totalDivisionalGames : 0;
                winPercentages.Add(j.TeamId, (overallWinPercentage, divisionalWinPercentage));
            }
            return winPercentages;
        }

        private List<(int, double?)> GetHeadToHeads(int teamId, List<GameResult> gameResults, IEnumerable<int> otherTeamIds)
        {
            List<(int, double?)> h2hs = [];
            foreach (var otherId in otherTeamIds)
            {
                var h2hWins = gameResults.Where(g => g.WinnerId == teamId && g.LoserId == otherId);
                var total = gameResults.Where(g => (g.HomeTeamId == teamId && g.AwayTeamId == otherId) || (g.AwayTeamId == teamId && g.HomeTeamId == otherId));
                if (total.Any()) h2hs.Add((otherId, h2hWins.Count() / total.Count()));
                else h2hs.Add((otherId, null));

            }
            return h2hs;
        }

        private List<DivisionStanding> CalcuateDivisionStandingsWithHeadToHeads(List<DivisionStanding> divisionStandings, Dictionary<int, List<(int, double?)>> headToHeadDictionary)
        {
            List<List<DivisionStanding>> subDivisionStandings = [];
            List<DivisionStanding> divisionStandingsCopy = [];
            divisionStandingsCopy.AddRange(divisionStandings);

            var counter = 0;
            while (counter < divisionStandings.Count)
            {
                var teamId = divisionStandings.ElementAt(counter).TeamId;
                if (headToHeadDictionary.TryGetValue(teamId, out var h2h) && divisionStandingsCopy.Any(d => d.TeamId == teamId))
                {
                    if (counter > 0)
                    {
                        var subStandings = divisionStandings.Where(d => divisionStandings.IndexOf(d) < counter && divisionStandingsCopy.Contains(d)).ToList();
                        if (subStandings.Count > 0)
                        {
                            subDivisionStandings.Add(subStandings);
                            divisionStandingsCopy.RemoveAll(d => subStandings.Contains(d));
                        }
                    }
                    var otherTeamIds = h2h.Select(g => g.Item1);
                    var subStandingsFromGroup = divisionStandings.Where(d => otherTeamIds.Contains(d.TeamId) || d.TeamId == teamId).ToList();
                    if (subStandingsFromGroup.Count > 0)
                    {
                        subStandingsFromGroup = SortStandingsByHeadToHead(subStandingsFromGroup, headToHeadDictionary);
                        counter = 0;
                        subDivisionStandings.Add(subStandingsFromGroup);
                        divisionStandingsCopy.RemoveAll(d => subStandingsFromGroup.Contains(d));
                    }
                    else counter++;
                }
                else counter++;
            }

            List<DivisionStanding> finalStandings = [];
            for (int i = 0; i < subDivisionStandings.Count; i++)
            {
                finalStandings.AddRange(subDivisionStandings.ElementAt(i));
            }
            return [.. finalStandings.OrderBy(f => f.Standing)];
        }

        private List<DivisionStanding> SortStandingsByHeadToHead(List<DivisionStanding> standings, Dictionary<int, List<(int, double?)>> headToHeadDictionary)
        {
            for (int i = 0; i < standings.Count - 1; i++)
            {
                var winPercentVsNextTeam = headToHeadDictionary[standings.ElementAt(i).TeamId].FirstOrDefault(h => h.Item1 == standings.ElementAt(i + 1).TeamId);
                if (winPercentVsNextTeam.Item2 != null && winPercentVsNextTeam.Item2 < 0.5)
                {
                    var first = standings.ElementAt(i).Standing;
                    var second = standings.ElementAt(i + 1).Standing;
                    standings.ElementAt(i).Standing = second;
                    standings.ElementAt(i + 1).Standing = first;
                    standings = SortStandingsByHeadToHead([.. standings.OrderBy(s => s.Standing)], headToHeadDictionary);
                }
            }
            return standings;
        }
    }

}

