﻿using Football.Projections.Interfaces;
using Football.Fantasy.Interfaces;
using Football.Models;
using Football.Enums;
using Microsoft.Extensions.Options;
using Football.Projections.Models;
using Football.Fantasy.Models;
using Serilog;
using Football.Players.Interfaces;

namespace Football.Projections.Services
{
    public class ProjectionAnalysisService(IFantasyDataService fantasyService,
        IOptionsMonitor<Season> season, IProjectionService<SeasonProjection> seasonProjection,
        IProjectionService<WeekProjection> weekProjection, ILogger logger, IOptionsMonitor<Starters> starters, IPlayersService playersService) : IProjectionAnalysisService
    {
        private readonly Season _season = season.CurrentValue;
        private readonly Starters _starters = starters.CurrentValue;

        public async Task<List<WeeklyProjectionError>> GetWeeklyProjectionError(int playerId)
        {
            List<WeeklyProjectionError> errors = [];
            var projections = await weekProjection.GetPlayerProjections(playerId);
            if (projections != null)
            {
                var fantasy = await fantasyService.GetWeeklyFantasy(playerId);
                foreach (var projection in projections)
                {
                    var weeklyFantasy = fantasy.FirstOrDefault(w => w.Week == projection.Week);
                    if (weeklyFantasy != null && projection.ProjectedPoints > 0)
                    {
                        errors.Add(new WeeklyProjectionError
                        {
                            Season = projection.Season,
                            Week = projection.Week,
                            Position = projection.Position,
                            PlayerId = playerId,
                            Name = projection.Name,
                            ProjectedPoints = projection.ProjectedPoints,
                            FantasyPoints = weeklyFantasy.FantasyPoints,
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
            else return Enumerable.Empty<WeeklyProjectionError>().ToList();  
        }

        public async Task<WeeklyProjectionAnalysis> GetWeeklyProjectionAnalysis(Position position, int week)
        {
            var projectionsExist = (weekProjection.GetProjectionsFromSQL(position, week, out var projections));
            if (projectionsExist)
            {
                var weeklyFantasy = (await fantasyService.GetWeeklyFantasy(_season.CurrentSeason, week)).Where(w => w.Position == position.ToString());
                return new()
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
                return new() 
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
                return new ()
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
            else return new();
        }
        public async Task<List<SeasonFlex>> SeasonFlexRankings()
        {
            List<SeasonFlex> flexRankings = [];
            var qbProjections = (await seasonProjection.GetProjections(Position.QB)).ToList();
            var rbProjections = (await seasonProjection.GetProjections(Position.RB)).ToList();
            var wrProjections = (await seasonProjection.GetProjections(Position.WR)).ToList();
            var teProjections = (await seasonProjection.GetProjections(Position.TE)).ToList();
            try
            {
                var rankings = qbProjections.Concat(rbProjections).Concat(wrProjections).Concat(teProjections).ToList();
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
                            Vorp = rank.ProjectedPoints - await GetReplacementPoints(position)
                        });
                    }
                }
                return flexRankings.OrderByDescending(f => f.Vorp).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString() + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }

        public async Task<List<WeekProjection>> WeeklyFlexRankings()
        {            
            var rbProjections = (await weekProjection.GetProjections(Position.RB)).ToList();
            var wrProjections = (await weekProjection.GetProjections(Position.WR)).ToList();
            var teProjections = (await weekProjection.GetProjections(Position.TE)).ToList();
            var rankings = rbProjections.Concat(wrProjections).Concat(teProjections);
            return rankings.OrderByDescending(r => r.ProjectedPoints).ToList();
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
            var mean = observedFantasyPoints.Average();
            foreach (var ofp in observedFantasyPoints)
            {
                totalSumOfSquares += Math.Pow(ofp - mean, 2);
            }
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
        private async Task<double> GetReplacementPoints(Position position)
        {
            return position switch
            {
                Position.QB => (await seasonProjection.GetProjections(position)).ElementAt(_starters.QBStarters - 1).ProjectedPoints,
                Position.RB => (await seasonProjection.GetProjections(position)).ElementAt(_starters.RBStarters - 1).ProjectedPoints,
                Position.WR => (await seasonProjection.GetProjections(position)).ElementAt(_starters.WRStarters - 1).ProjectedPoints,
                Position.TE => (await seasonProjection.GetProjections(position)).ElementAt(_starters.TEStarters - 1).ProjectedPoints,
                _ => 0,
            };
        }
    }
}
