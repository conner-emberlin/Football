using Football.Enums;
using Football.Models;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Football.Players.Interfaces;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using Microsoft.Extensions.Options;

namespace Football.Projections.Services
{
    public class ProjectionAnalysisService(IFantasyDataService fantasyService,
        IOptionsMonitor<Season> season, IProjectionService<SeasonProjection> seasonProjection,
        IProjectionService<WeekProjection> weekProjection, IOptionsMonitor<Starters> starters, IPlayersService playersService) : IProjectionAnalysisService
    {
        private readonly Season _season = season.CurrentValue;
        private readonly Starters _starters = starters.CurrentValue;

        public async Task<List<WeeklyProjectionError>> GetWeeklyProjectionError(int playerId)
        {
            List<WeeklyProjectionError> errors = [];
            var projections = await weekProjection.GetPlayerProjections(playerId);
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

        public async Task<WeeklyProjectionAnalysis> GetWeeklyProjectionAnalysis(Position position, int week)
        {
            var projectionsExist = (weekProjection.GetProjectionsFromSQL(position, week, out var projections));
            if (projectionsExist)
            {
                var weeklyFantasy = (await fantasyService.GetWeeklyFantasy(_season.CurrentSeason, week)).Where(w => w.Position == position.ToString());
                return new WeeklyProjectionAnalysis
                {
                    Season = _season.CurrentSeason,
                    Week = week,
                    Position = position.ToString(),
                    MSE = GetMeanSquaredError(projections, weeklyFantasy),
                    RSquared = GetRSquared(projections, weeklyFantasy),
                    MAE = GetMeanAbsoluteError(projections, weeklyFantasy),
                    MAPE = GetMeanAbsolutePercentageError(projections, weeklyFantasy),
                    AvgError = GetAverageError(projections, weeklyFantasy),
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
                    MSE = GetMeanSquaredError(projections, weeklyFantasy),
                    RSquared = GetRSquared(projections, weeklyFantasy),
                    MAE = GetMeanAbsoluteError(projections, weeklyFantasy),
                    MAPE = GetMeanAbsolutePercentageError(projections, weeklyFantasy),
                    AvgError = GetAverageError(projections, weeklyFantasy),
                    AvgRankError = GetAverageRankError(projections, weeklyFantasy),
                    AdjAvgError = GetAdjustedAverageError(projections, weeklyFantasy)
                };
            }
            return new();
        }
        public async Task<IEnumerable<SeasonFlex>> SeasonFlexRankings()
        {
            List<SeasonFlex> flexRankings = [];
            var rankings = (await seasonProjection.GetProjections(Position.QB))
                           .Union(await seasonProjection.GetProjections(Position.RB))
                           .Union(await seasonProjection.GetProjections(Position.WR))
                           .Union(await seasonProjection.GetProjections(Position.TE));

            var replacementPointsDictionary = GetReplacementPointsDictionary(rankings);

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
                        Vorp = rank.ProjectedPoints - replacementPointsDictionary[position]
                    });
                }
            }
            return flexRankings.OrderByDescending(f => f.Vorp).ToList();
        }

        public async Task<List<SeasonProjectionError>> GetSeasonProjectionError(Position position, int priorSeason = 0)
        {
            var players = await playersService.GetPlayersByPosition(position);
            var season = priorSeason > 0 ? priorSeason : _season.CurrentSeason;
            List<SeasonProjectionError> analyses = [];
            foreach (var player in players)
            {
                var seasonProjection = await playersService.GetSeasonProjection(season, player.PlayerId);
                if (seasonProjection > 0)
                {
                    var weeklyFantasy = (await fantasyService.GetWeeklyFantasy(player.PlayerId)).Where(w => w.Season == season);
                    analyses.Add(new SeasonProjectionError
                    {
                        Player = player,
                        TotalFantasy = weeklyFantasy.Sum(w => w.FantasyPoints),
                        WeeksPlayed = weeklyFantasy.Count(),
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
                    AvgError = count > 0 ? Math.Round(totalError / count) : 0
                };
            }
            return new SeasonProjectionAnalysis { };
        }

        public async Task<List<SeasonProjectionAnalysis>> GetAllSeasonProjectionAnalyses(Position position)
        {
            var seasons = await playersService.GetSeasons();
            List<SeasonProjectionAnalysis> spa = [];
            foreach (var season in seasons)
                spa.Add(await GetSeasonProjectionAnalysis(position, season));
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
        private static double GetMeanSquaredError(IEnumerable<WeekProjection> projections, IEnumerable<WeeklyFantasy> weeklyFantasy)
        {
            var sumOfSquares = 0.0;
            var count = 0;
            foreach (var projection in projections)
            {
                var fantasy = weeklyFantasy.FirstOrDefault(w => w.Week == projection.Week && w.PlayerId == projection.PlayerId);
                if (fantasy != null)
                {
                    sumOfSquares += Math.Pow(fantasy.FantasyPoints - projection.ProjectedPoints, 2);
                    count++;
                }
            }
            return count > 0 ? Math.Round(sumOfSquares / count, 3) : 0;
        }

        private static double GetAverageError(IEnumerable<WeekProjection> projections, IEnumerable<WeeklyFantasy> weeklyFantasy)
        {
            var count = 0;
            var totalError = 0.0;
            foreach (var projection in projections)
            {
                var fantasy = weeklyFantasy.FirstOrDefault(w => w.Week == projection.Week && w.PlayerId == projection.PlayerId);
                if (fantasy != null)
                {
                    totalError += Math.Abs(projection.ProjectedPoints - fantasy.FantasyPoints);
                    count++;
                }
            }
            return count > 0 ? Math.Round(totalError / count) : 0;
        }

        private static double GetAdjustedAverageError(IEnumerable<WeekProjection> projections, IEnumerable<WeeklyFantasy> weeklyFantasy)
        {
            var maxWeek = weeklyFantasy.OrderByDescending(w => w.FantasyPoints).First().Week;
            var minWeek = weeklyFantasy.OrderBy(w => w.FantasyPoints).First().Week;
            var adjustedProjections = projections.Where(p => p.Week != maxWeek && p.Week != minWeek);
            var adjustedFantasy = weeklyFantasy.Where(p => p.Week != maxWeek && p.Week != minWeek);
            return GetAverageError(adjustedProjections, adjustedFantasy);
        }
        private static double GetRSquared(IEnumerable<WeekProjection> projections, IEnumerable<WeeklyFantasy> weeklyFantasy)
        {
            List<double> observedFantasyPoints = [];
            var residualSumOfSquares = 0.0;
            var totalSumOfSquares = 0.0;
            var count = 0;
            foreach (var projection in projections)
            {
                var fantasy = weeklyFantasy.FirstOrDefault(w => w.Week == projection.Week && w.PlayerId == projection.PlayerId);
                if (fantasy != null)
                {
                    observedFantasyPoints.Add(fantasy.FantasyPoints);
                    residualSumOfSquares += Math.Pow(fantasy.FantasyPoints - projection.ProjectedPoints, 2);
                    count++;
                }
            }
            foreach (var ofp in observedFantasyPoints)
                totalSumOfSquares += Math.Pow(ofp - observedFantasyPoints.Average(), 2);
            return totalSumOfSquares > 0 ? 1 - (residualSumOfSquares / totalSumOfSquares) : 0;
        }
    

        private static double GetMeanAbsoluteError(IEnumerable<WeekProjection> projections, IEnumerable<WeeklyFantasy> weeklyFantasy)
        {
            var sumOfAbsDifference = 0.0;
            var count = 0;
            foreach (var projection in projections)
            {
                var fantasy = weeklyFantasy.FirstOrDefault(w => w.Week == projection.Week && w.PlayerId == projection.PlayerId);
                if (fantasy != null)
                {
                    sumOfAbsDifference += Math.Abs(projection.ProjectedPoints - fantasy.FantasyPoints);
                    count++;
                }
            }
            return count > 0 ? sumOfAbsDifference / count : 0;
        }

        private static double GetMeanAbsolutePercentageError(IEnumerable<WeekProjection> projections, IEnumerable<WeeklyFantasy> weeklyFantasy)
        {
            var sumOfError = 0.0;
            var count = 0;
            foreach (var projection in projections)
            {
                var fantasy = weeklyFantasy.FirstOrDefault(w => w.Week == projection.Week && w.PlayerId == projection.PlayerId);
                if (fantasy != null && fantasy.FantasyPoints > 0)
                {
                    sumOfError += Math.Abs((projection.ProjectedPoints - fantasy.FantasyPoints) / fantasy.FantasyPoints);
                    count++;
                }
            }
            return count > 0 ? (sumOfError/count)*100 : 0;
        }

        private static double GetAverageRankError(IEnumerable<WeekProjection> projections, IEnumerable<WeeklyFantasy> weeklyFantasy)
        {
            var orderedFantasy = weeklyFantasy.OrderByDescending(w => w.FantasyPoints);
            var orderedProjections = projections.OrderByDescending(p => p.ProjectedPoints);
            var error = 0.0;
            var count = 0;
            foreach (var op in orderedProjections)
            {
                var projectedRank = orderedProjections.ToList().IndexOf(op);
                var fantasy = orderedFantasy.FirstOrDefault(of => of.PlayerId == op.PlayerId);
                if (fantasy != null)
                {
                    var actualRank = orderedFantasy.ToList().IndexOf(fantasy);
                    error += Math.Abs(projectedRank - actualRank);
                    count++;
                }                
            }
            return count > 0 ? Math.Round(error / count, 2) : 0;
        }
        private Dictionary<Position, double> GetReplacementPointsDictionary(IEnumerable<SeasonProjection> rankings)
        {
            var rankingsByPosition = rankings.GroupBy(p => p.Position, p => p.ProjectedPoints, (key, p) => new {Position = key, Projections = p.OrderByDescending(p => p)});
            Dictionary<Position, double> replacementPointsDictionary = [];
            foreach (var rank in rankingsByPosition)
            {
                _ = Enum.TryParse(rank.Position, out Position position);
                var replacementIndex = (int)Math.Floor((double)rank.Projections.Count() / 2);
                replacementPointsDictionary.Add(position, rank.Projections.ElementAt(replacementIndex));
            }
            return replacementPointsDictionary;
        }


    }
}
