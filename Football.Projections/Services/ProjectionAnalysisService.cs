using Football.Enums;
using Football.Models;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Football.Players.Interfaces;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using Microsoft.Extensions.Options;
using Football.Players.Models;

namespace Football.Projections.Services
{
    public class ProjectionAnalysisService(IFantasyDataService fantasyService,
        IOptionsMonitor<Season> season, IProjectionService<SeasonProjection> seasonProjection,
        IProjectionService<WeekProjection> weekProjection, IPlayersService playersService, IStatisticsService statisticsService,
        ISettingsService settingsService, ISleeperLeagueService sleeperLeagueService) : IProjectionAnalysisService
    {
        private readonly Season _season = season.CurrentValue;

        public async Task<List<WeeklyProjectionError>> GetWeeklyProjectionError(int playerId)
        {
            List<WeeklyProjectionError> errors = [];
            var projections = (await weekProjection.GetPlayerProjections(playerId)).Where(p => p.Season == _season.CurrentSeason);
            if (projections != null)
            {
                var fantasy = (await fantasyService.GetWeeklyFantasy(playerId)).ToDictionary(f => f.Week);
                foreach (var projection in projections)
                {
                    if (fantasy.TryGetValue(projection.Week, out var weeklyFantasy) && projection.ProjectedPoints > 0)
                    {
                        errors.Add(new WeeklyProjectionError
                        {
                            Season = projection.Season,
                            Week = projection.Week,
                            Position = projection.Position,
                            PlayerId = playerId,
                            Name = projection.Name,
                            ProjectedPoints = projection.ProjectedPoints,
                            FantasyPoints = fantasy[projection.Week].FantasyPoints,
                            Error = Math.Abs(projection.ProjectedPoints - weeklyFantasy.FantasyPoints)
                        });
                    }
                }
            }
            return errors;
        }
        public async Task<List<WeeklyProjectionError>> GetWeeklyProjectionError(Position position, int week)
        {
            var projectionsExist = (weekProjection.GetProjectionsFromSQL(position, week, out var projections));
            if (projectionsExist)
            {
                List<WeeklyProjectionError> weeklyProjectionErrors = [];
                var weeklyFantasy = (await fantasyService.GetWeeklyFantasy(_season.CurrentSeason, week)).Where(w => w.Position == position.ToString());
                foreach (var projection in projections)
                {
                    var fantasy = weeklyFantasy.FirstOrDefault(w => w.Week == projection.Week && w.PlayerId == projection.PlayerId);
                    if (fantasy != null)
                    {
                        weeklyProjectionErrors.Add(new WeeklyProjectionError
                        {
                            Season = _season.CurrentSeason,
                            Week = week,
                            Position = position.ToString(),
                            PlayerId = fantasy.PlayerId,
                            Name = fantasy.Name,
                            ProjectedPoints = projection.ProjectedPoints,
                            FantasyPoints = fantasy.FantasyPoints,
                            Error = Math.Abs(projection.ProjectedPoints - fantasy.FantasyPoints)
                        });
                    }
                }
                return weeklyProjectionErrors;
            }
            return Enumerable.Empty<WeeklyProjectionError>().ToList();  
        }

        public async Task<Dictionary<int, double>> GetAverageWeeklyProjectionErrorsByPosition(Position position, int season)
        {
            var weeklyFantasy = await fantasyService.GetWeeklyFantasy(position, season);           
            if (weeklyFantasy.Count > 0)
            {
                var weeks = weeklyFantasy.Select(w => w.Week).Distinct();
                List<WeekProjection> projections = [];
                foreach (var week in weeks)
                {
                    if (weekProjection.GetProjectionsFromSQL(position, week, out var weekProj))
                    {
                        projections.AddRange(weekProj);
                    }
                }

                var weeklyData = weeklyFantasy.Join(projections, f => new { f.PlayerId, f.Week, f.Season }, p => new { p.PlayerId, p.Week, p.Season }, (f, p) => new { WeeklyFantasy = f, WeeklyProjection = p })
                                              .GroupBy(w => new { w.WeeklyFantasy.PlayerId })
                                              .Select(p => new { p.Key.PlayerId, AverageDiff = p.Average(f => Math.Abs(f.WeeklyFantasy.FantasyPoints - f.WeeklyProjection.ProjectedPoints)) })
                                              .ToDictionary(d => d.PlayerId, d => d.AverageDiff);
                return weeklyData;
            }

            return [];
        }

        public async Task<WeeklyProjectionAnalysis> GetWeeklyProjectionAnalysis(Position position, int week)
        {
            var projectionsExist = (weekProjection.GetProjectionsFromSQL(position, week, out var projections));
            if (projectionsExist)
            {
                var weeklyFantasy = (await fantasyService.GetWeeklyFantasy(_season.CurrentSeason, week)).Where(w => w.Position == position.ToString());
                var fantasyDictionary = weeklyFantasy.ToDictionary(w => (w.Week, w.PlayerId), w => w.FantasyPoints);
                return new WeeklyProjectionAnalysis
                {
                    Season = _season.CurrentSeason,
                    Week = week,
                    Position = position.ToString(),
                    ProjectionCount = projections.Count(p => weeklyFantasy.Any(w => w.PlayerId == p.PlayerId)),
                    MSE = GetMeanSquaredError(projections, fantasyDictionary),
                    RSquared = GetRSquared(projections, fantasyDictionary),
                    MAE = GetMeanAbsoluteError(projections, fantasyDictionary),
                    MAPE = GetMeanAbsolutePercentageError(projections, fantasyDictionary),
                    AvgError = GetAverageError(projections, fantasyDictionary),
                    AvgRankError = GetAverageRankError(projections, weeklyFantasy),
                };
            }
            else
            {
                return new WeeklyProjectionAnalysis 
                { 
                    Season = _season.CurrentSeason,
                    Week = week,
                    Position = position.ToString()
                };
            }
        }

        public async Task<WeeklyProjectionAnalysis> GetWeeklyProjectionAnalysis(int playerId)
        {
            var weeklyFantasy = await fantasyService.GetWeeklyFantasy(playerId);
            var fantasyDictionary = weeklyFantasy.ToDictionary(w => (w.Week, w.PlayerId), w => w.FantasyPoints);
            List<WeekProjection> projections = [];
            foreach (var wf in weeklyFantasy)
            {
                var proj = await playersService.GetWeeklyProjection(_season.CurrentSeason, wf.Week, playerId);
                if (proj > 0)
                {
                    projections.Add(new WeekProjection
                    {
                        Season = wf.Season,
                        Week = wf.Week,
                        PlayerId = playerId,
                        ProjectedPoints = proj
                    });
                }
            }
            if (projections.Count > 0)
            {
                return new WeeklyProjectionAnalysis
                {
                    Season = _season.CurrentSeason,
                    MSE = GetMeanSquaredError(projections, fantasyDictionary),
                    RSquared = GetRSquared(projections, fantasyDictionary),
                    MAE = GetMeanAbsoluteError(projections, fantasyDictionary),
                    MAPE = GetMeanAbsolutePercentageError(projections, fantasyDictionary),
                    AvgError = GetAverageError(projections, fantasyDictionary),
                    AvgRankError = GetAverageRankError(projections, weeklyFantasy),
                    AdjAvgError = GetAdjustedAverageError(projections, weeklyFantasy)
                };
            }
            return new();
        }
        public async Task<IEnumerable<SeasonFlex>> SeasonFlexRankings()
        {
            List<SeasonFlex> flexRankings = [];
            if (seasonProjection.GetProjectionsFromSQL(Position.QB, _season.CurrentSeason, out var qb)
                && seasonProjection.GetProjectionsFromSQL(Position.RB, _season.CurrentSeason, out var rb)
                && seasonProjection.GetProjectionsFromSQL(Position.WR, _season.CurrentSeason, out var wr)
                && seasonProjection.GetProjectionsFromSQL(Position.TE, _season.CurrentSeason, out var te))
            {
                var rankings = qb.Union(rb).Union(wr).Union(te);
                var replacementPointsDictionary = GetReplacementPointsDictionary(rankings, await settingsService.GetSeasonTunings(_season.CurrentSeason));
                foreach (var rank in rankings)
                {
                    if (Enum.TryParse(rank.Position, out Position position))
                    {
                        flexRankings.Add(new SeasonFlex
                        {
                            PlayerId = rank.PlayerId,
                            Name = rank.Name,
                            Position = rank.Position,
                            ProjectedPoints = rank.ProjectedPoints,
                            Vorp = (rank.ProjectedPoints - replacementPointsDictionary[position])
                        });
                    }
                }
                var ordered = flexRankings.OrderByDescending(f => f.Vorp).ToList();
                return ordered;
            }
            return Enumerable.Empty<SeasonFlex>().ToList();           
        }

        public async Task<List<SeasonProjectionError>> GetSeasonProjectionError(Position position, int priorSeason = 0)
        {
            var players = await playersService.GetPlayersByPosition(position);
            var season = priorSeason > 0 ? priorSeason : _season.CurrentSeason;
            var seasonProjectionDictionary = await playersService.GetSeasonProjections(players.Select(p => p.PlayerId), season);
            var gamesPlayedDictionary = await GamesPlayedDictionary(position, season);

            List<SeasonProjectionError> analyses = [];
            foreach (var player in players)
            {
                if (seasonProjectionDictionary.TryGetValue(player.PlayerId, out var seasonProjection)
                    && gamesPlayedDictionary.TryGetValue(player.PlayerId, out var games) && games > 14)
                {
                    var weeklyFantasy = await fantasyService.GetWeeklyFantasyBySeason(player.PlayerId, season);
                    analyses.Add(new SeasonProjectionError
                    {
                        Player = player,
                        TotalFantasy = weeklyFantasy.Sum(w => w.FantasyPoints),
                        WeeksPlayed = weeklyFantasy.Count,
                        SeasonFantasyProjection = seasonProjection
                    });
                }
            }
            return analyses;
        }
        public async Task<SeasonProjectionAnalysis> GetSeasonProjectionAnalysis(Position position, int season = 0)
        {
            var projectionErrors = await GetSeasonProjectionError(position, season);
            var count = projectionErrors.Count;
            if (count > 0)
            {
                var seasonGames = season > 0 ? await playersService.GetGamesBySeason(season) : await playersService.GetCurrentSeasonGames();
                var totalError = projectionErrors.Sum(p => Math.Abs(p.TotalFantasy - p.SeasonFantasyProjection));
                var sumOfSquares = projectionErrors.Sum(p => Math.Pow(p.TotalFantasy - p.SeasonFantasyProjection, 2));
                var observedPoints = projectionErrors.Select(p => p.TotalFantasy);
                var totalSumOfSquares = observedPoints.Sum(p => Math.Pow(p - observedPoints.Average(), 2));
                return new SeasonProjectionAnalysis
                {
                    Season = season > 0 ? season : _season.CurrentSeason,
                    Position = position.ToString(),
                    MSE = count > 0 ? Math.Round(sumOfSquares / count, 3) : 0,
                    RSquared = totalSumOfSquares > 0 ? 1 - (sumOfSquares / totalSumOfSquares) : 0,
                    AvgError = count > 0 ? Math.Round(totalError / count) : 0,
                    AvgErrorPerGame = count > 0 ? Math.Round(totalError / count / seasonGames, 2) : 0,
                    ProjectionCount = count
                };
            }
            return new SeasonProjectionAnalysis { };
        }

        public async Task<List<SeasonProjectionAnalysis>> GetAllSeasonProjectionAnalyses(Position position)
        {
            List<SeasonProjectionAnalysis> spa = [];
            var seasons = ((await playersService.GetSeasons()).Where(s => s < _season.CurrentSeason)).ToList();
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            var weeks = await playersService.GetWeeksBySeason(_season.CurrentSeason);
            if (currentWeek == weeks + 1) seasons.Add(_season.CurrentSeason);

            foreach (var season in seasons) 
            { 
                if (seasonProjection.GetProjectionsFromSQL(position, season, out var projections))
                {
                    spa.Add(await GetSeasonProjectionAnalysis(position, season));
                }
            
            }           
            return spa;
        }

        public async Task<IEnumerable<WeekProjection>> WeeklyFlexRankings()
        {            
            var rbProjections = await weekProjection.GetProjections(Position.RB);
            var wrProjections = await weekProjection.GetProjections(Position.WR);
            var teProjections = await weekProjection.GetProjections(Position.TE);
            var rankings = rbProjections.Union(wrProjections).Union(teProjections);
            return rankings.OrderByDescending(r => r.ProjectedPoints);
        }

        public async Task<List<MatchupProjections>> GetMatchupProjections(string username, int week)
        {
            List<MatchupProjections> matchupProjections = [];
            var leagueTuple = await GetCurrentSleeperLeague(username);
            if (leagueTuple != null)
            {
                var (user, league) = leagueTuple;
                if (league != null)
                {
                    var rosters = await sleeperLeagueService.GetSleeperRosters(league.LeagueId);
                    var matchups = await sleeperLeagueService.GetSleeperMatchups(league.LeagueId, week);
                    if (rosters != null && matchups != null)
                    {
                        var rosterMatchup = rosters.Join(matchups, r => r.RosterId, m => m.RosterId, (r, m) => new { SleeperRoster = r, SleeperMatchup = m })
                                           .GroupBy(rm => rm.SleeperMatchup.MatchupId,
                                                    rm => rm.SleeperRoster,
                                                    (key, r) => new { MatchupId = key, Rosters = r.ToList() })
                                           .First(g => g.Rosters.Any(r => r.OwnerId == user.UserId));

                        var userRoster = rosterMatchup.Rosters.First(r => r.OwnerId == user.UserId);
                        var userProjections = await GetProjectionsFromRoster(userRoster, week);
                        var opponentRoster = rosterMatchup.Rosters.First(r => r.OwnerId != user.UserId);
                        var opponentProjections = await GetProjectionsFromRoster(opponentRoster, week);
                        var opponent = await sleeperLeagueService.GetSleeperUser(opponentRoster.OwnerId);
                        matchupProjections.Add(new MatchupProjections { LeagueName = league.Name, TeamName = user.DisplayName, TeamProjections = userProjections });
                        matchupProjections.Add(new MatchupProjections { LeagueName = league.Name, TeamName = opponent!.DisplayName, TeamProjections = opponentProjections });
                    }
                }
            }
            return matchupProjections;
        }

        public async Task<IEnumerable<PlayerWeeklyProjectionAnalysis>> GetInSeasonProjectionAnalysesByPosition(Position position)
        {
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            List<WeekProjection> allProjections = [];
            for (int i = 1; i < currentWeek; i++)
            {
                if (weekProjection.GetProjectionsFromSQL(position, i, out var weeklyProjections))
                {
                    allProjections.AddRange(weeklyProjections);
                }
            }

            var groupedProjections = allProjections.GroupBy(ap => ap.PlayerId, ap => ap, (key, p) => new { PlayerId = key, Projections = p});
            var allFantasyDictionary = (await fantasyService.GetWeeklyFantasy(position, _season.CurrentSeason)).ToDictionary(f => (f.Week, f.PlayerId), f => f.FantasyPoints);
            List<PlayerWeeklyProjectionAnalysis> analyses = [];
            foreach (var grouping in groupedProjections)
            {
                analyses.Add(new PlayerWeeklyProjectionAnalysis
                {
                    Season = _season.CurrentSeason,
                    PlayerId = grouping.PlayerId,
                    Name = grouping.Projections.First().Name,
                    ProjectionCount = grouping.Projections.Count(),
                    MSE = GetMeanSquaredError(grouping.Projections, allFantasyDictionary),
                    RSquared = GetRSquared(grouping.Projections, allFantasyDictionary),
                    MAE = GetMeanAbsoluteError(grouping.Projections, allFantasyDictionary),
                    MAPE = GetMeanAbsolutePercentageError(grouping.Projections, allFantasyDictionary),
                    AvgError = GetAverageError(grouping.Projections, allFantasyDictionary),
                });
            }
            return analyses.OrderByDescending(a => a.RSquared).ThenBy(a => a.AvgError);
        }
        private static double GetMeanSquaredError(IEnumerable<WeekProjection> projections, Dictionary<(int Week, int PlayerId), double> fantasyDictionary)
        {
            var sumOfSquares = 0.0;
            var count = 0;
            foreach (var projection in projections)
            {
                if (fantasyDictionary.TryGetValue((projection.Week, projection.PlayerId), out var fantasy))
                {
                    sumOfSquares += Math.Pow(fantasy - projection.ProjectedPoints, 2);
                    count++;
                }
            }
            return count > 0 ? Math.Round(sumOfSquares / count, 3) : 0;
        }

        private static double GetAverageError(IEnumerable<WeekProjection> projections, Dictionary<(int Week, int PlayerId), double> fantasyDictionary)
        {

            var count = 0;
            var totalError = 0.0;
            foreach (var projection in projections)
            {
                if (fantasyDictionary.TryGetValue((projection.Week, projection.PlayerId), out var fantasy))
                {
                    totalError += Math.Abs(projection.ProjectedPoints - fantasy);
                    count++;
                }
            }
            return count > 0 ? Math.Round(totalError / count, 2) : 0;
        }

        private static double GetAdjustedAverageError(IEnumerable<WeekProjection> projections, IEnumerable<WeeklyFantasy> weeklyFantasy)
        {
            
            var maxWeek = weeklyFantasy.OrderByDescending(w => w.FantasyPoints).First().Week;
            var minWeek = weeklyFantasy.OrderBy(w => w.FantasyPoints).First().Week;
            var adjustedProjections = projections.Where(p => p.Week != maxWeek && p.Week != minWeek);
            var adjustedFantasyDictionary = weeklyFantasy.Where(p => p.Week != maxWeek && p.Week != minWeek).ToDictionary(w => (w.Week, w.PlayerId), w => w.FantasyPoints);
            return GetAverageError(adjustedProjections, adjustedFantasyDictionary);

        }
        private static double GetRSquared(IEnumerable<WeekProjection> projections, Dictionary<(int Week, int PlayerId), double> fantasyDictionary)
        {
            var residualSumOfSquares = 0.0;
            var observedSum = 0.0;
            var observedSumSquared = 0.0;
            var count = 0;

            foreach (var projection in projections)
            {
                
                if (fantasyDictionary.TryGetValue((projection.Week, projection.PlayerId), out var observedPoints))
                {
                    residualSumOfSquares += Math.Pow(observedPoints - projection.ProjectedPoints, 2);
                    observedSum += observedPoints;
                    observedSumSquared += Math.Pow(observedPoints, 2);
                    count++;
                }
            }

            if (count == 0) return 0;
            var totalSumOfSquares = observedSumSquared - (Math.Pow(observedSum, 2)/ count);
            return totalSumOfSquares > 0 ? 1 - (residualSumOfSquares / totalSumOfSquares) : 0;
        }
    

        private static double GetMeanAbsoluteError(IEnumerable<WeekProjection> projections, Dictionary<(int Week, int PlayerId), double> fantasyDictionary)
        {
            var sumOfAbsDifference = 0.0;
            var count = 0;
            foreach (var projection in projections)
            {
                if (fantasyDictionary.TryGetValue((projection.Week, projection.PlayerId), out var fantasyPoints))
                {
                    sumOfAbsDifference += Math.Abs(projection.ProjectedPoints - fantasyPoints);
                    count++;
                }
            }
            return count > 0 ? sumOfAbsDifference / count : 0;
        }

        private static double GetMeanAbsolutePercentageError(IEnumerable<WeekProjection> projections, Dictionary<(int Week, int PlayerId), double> fantasyDictionary)
        {
            var sumOfError = 0.0;
            var count = 0;
            foreach (var projection in projections)
            {
                if (fantasyDictionary.TryGetValue((projection.Week, projection.PlayerId), out var fantasyPoints) && fantasyPoints > 0)
                {
                    sumOfError += Math.Abs((projection.ProjectedPoints - fantasyPoints) / fantasyPoints);
                    count++;
                }
            }
            return count > 0 ? (sumOfError/count) * 100 : 0;
        }

        private static double GetAverageRankError(IEnumerable<WeekProjection> projections, IEnumerable<WeeklyFantasy> weeklyFantasy)
        {
            var orderedFantasy = weeklyFantasy.OrderByDescending(w => w.FantasyPoints)
                                              .Select((w, index) => new { w.PlayerId, w.Week, Rank = index })
                                              .ToDictionary(x => (x.Week, x.PlayerId), x => x.Rank);

            var orderedProjections = projections.OrderByDescending(p => p.ProjectedPoints)
                                                .Select((p, index) => new { p.PlayerId, p.Week, Rank = index });
            var error = 0.0;
            var count = 0;

            foreach(var projection in orderedProjections)
            {
                if (orderedFantasy.TryGetValue((projection.Week, projection.PlayerId), out var actualRank))
                {
                    error += Math.Abs(projection.Rank - actualRank);
                    count ++;
                }
            }
            return count > 0 ? Math.Round(error / count, 2) : 0;
        }
        private Dictionary<Position, double> GetReplacementPointsDictionary(IEnumerable<SeasonProjection> rankings, Tunings tunings)
        {
            var rankingsByPosition = rankings.GroupBy(p => p.Position, p => p.ProjectedPoints, (key, p) => new {Position = key, Projections = p.OrderByDescending(p => p)});
            Dictionary<Position, double> replacementPointsDictionary = [];
            foreach (var rank in rankingsByPosition)
            {
                _ = Enum.TryParse(rank.Position, out Position position);
                var replacementLevel = settingsService.GetReplacementLevel(position, tunings);
                var projectionCount = rank.Projections.Count();
                var replacementIndex = projectionCount < replacementLevel ? (int)Math.Floor((double)(projectionCount / 2)) : replacementLevel;
                replacementPointsDictionary.Add(position, rank.Projections.ElementAt(replacementIndex));
            }
            return replacementPointsDictionary;
        }

        public async Task<List<WeekProjection>> GetSleeperLeagueProjections(string username)
        {
            List<WeekProjection> projections = [];
            var sleeperStarters = await GetSleeperLeagueStarters(username);
            if (sleeperStarters.Count > 0)
            {
                var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
                foreach (var starter in sleeperStarters)
                {
                    var projection = await playersService.GetWeeklyProjection(_season.CurrentSeason, currentWeek, starter.PlayerId);
                    projections.Add(new WeekProjection
                    {
                        PlayerId = starter.PlayerId,
                        Season = _season.CurrentSeason,
                        Week = currentWeek,
                        Name = starter.Name,
                        Position = starter.Position,
                        ProjectedPoints = projection
                    });
                }
            }
            return projections;
        }

        private async Task<Tuple<SleeperUser, SleeperLeague?>?> GetCurrentSleeperLeague(string username)
        {
            var sleeperUser = await sleeperLeagueService.GetSleeperUser(username);
            if (sleeperUser != null)
            {
                var userLeagues = await sleeperLeagueService.GetSleeperLeagues(sleeperUser.UserId);
                if (userLeagues != null)
                {
                    var league = userLeagues.FirstOrDefault(u => u.Season == _season.CurrentSeason.ToString() && u.Status == "in_season");
                    return Tuple.Create(sleeperUser, league);
                }
                else return null;
            }
            else return null;
        }
        private async Task<List<Player>> GetSleeperLeagueStarters(string username)
        {
            List<Player> sleeperStarters = [];
            var tuple = await GetCurrentSleeperLeague(username);
            if (tuple != null)
            {
                var (sleeperUser, currentLeague) = tuple;
                if (currentLeague != null)
                {
                    var roster = (await sleeperLeagueService.GetSleeperRosters(currentLeague.LeagueId))!
                                .FirstOrDefault(r => r.OwnerId == sleeperUser.UserId);
                    if (roster != null)
                    {
                        foreach (var starter in roster.Starters)
                        {
                            if (int.TryParse(starter, out var sleeperId))
                            {
                                var sleeperMap = await playersService.GetSleeperPlayerMap(sleeperId);
                                if (sleeperMap != null)
                                {
                                    sleeperStarters.Add(await playersService.GetPlayer(sleeperMap.PlayerId));
                                }
                            }
                        }
                    }
                }
            }
            return sleeperStarters;
        }
        private async Task<List<WeekProjection>> GetProjectionsFromRoster(SleeperRoster roster, int week)
        {
            List<WeekProjection> projections = [];
            foreach (var starter in roster.Starters)
            {
                if (int.TryParse(starter, out var sleeperId))
                {
                    var sleeperMap = await playersService.GetSleeperPlayerMap(sleeperId);
                    if (sleeperMap != null)
                    {
                        var player = await playersService.GetPlayer(sleeperMap.PlayerId);
                        var projection = await playersService.GetWeeklyProjection(_season.CurrentSeason, week, player.PlayerId);
                        projections.Add(new WeekProjection
                        {
                            PlayerId = player.PlayerId,
                            Season = _season.CurrentSeason,
                            Week = week,
                            Name = player.Name,
                            Position = player.Position,
                            ProjectedPoints = projection
                        });
                    }
                }
            }
            return projections;
        }

        private async Task<Dictionary<int, double>> GamesPlayedDictionary(Position position, int season)
        {
            return position switch
            {
                Position.QB => (await statisticsService.GetSeasonData<SeasonDataQB>(position, season, false)).ToDictionary(s => s.PlayerId, s => s.Games),
                Position.RB => (await statisticsService.GetSeasonData<SeasonDataRB>(position, season, false)).ToDictionary(s => s.PlayerId, s => s.Games),
                Position.WR => (await statisticsService.GetSeasonData<SeasonDataWR>(position, season, false)).ToDictionary(s => s.PlayerId, s => s.Games),
                Position.TE => (await statisticsService.GetSeasonData<SeasonDataTE>(position, season, false)).ToDictionary(s => s.PlayerId, s => s.Games),
                _ => []
            };
        }

    }
}
