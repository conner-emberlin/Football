using Football.Projections.Interfaces;
using Football.Fantasy.Interfaces;
using Football.Models;
using Football.Enums;
using Microsoft.Extensions.Options;
using Football.Projections.Models;
using Football.Fantasy.Models;

namespace Football.Projections.Services
{
    public class ProjectionAnalysisService : IProjectionAnalysisService
    {
        private readonly IFantasyDataService _fantasyService;
        private readonly IProjectionService _projectionService;
        private readonly Season _season;

        public ProjectionAnalysisService(IFantasyDataService fantasyService, IProjectionService projectionService, 
            IOptionsMonitor<Season> season)
        {
            _fantasyService = fantasyService;
            _projectionService = projectionService;
            _season = season.CurrentValue;
        }

        public async Task<List<WeeklyProjectionError>> GetWeeklyProjectionError(PositionEnum position, int week)
        {
            var projectionsExist = (_projectionService.GetWeeklyProjectionsFromSQL(position, week, out var projections));
            if (projectionsExist)
            {
                List<WeeklyProjectionError> weeklyProjectionErrors = new();
                var weeklyFantasy = (await _fantasyService.GetWeeklyFantasy(_season.CurrentSeason, week)).Where(w => w.Position == position.ToString());
                foreach (var projection in projections)
                {
                    var fantasy = weeklyFantasy.Where(w => w.Week == projection.Week && w.PlayerId == projection.PlayerId).FirstOrDefault();
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
                            Error = projection.ProjectedPoints - fantasy.FantasyPoints
                        });
                    }
                }
                return weeklyProjectionErrors;
            }
            else return Enumerable.Empty<WeeklyProjectionError>().ToList();  
        }

        public async Task<WeeklyProjectionAnalysis> GetWeeklyProjectionAnalysis(PositionEnum position, int week)
        {
            var projectionsExist = (_projectionService.GetWeeklyProjectionsFromSQL(position, week, out var projections));
            if (projectionsExist)
            {
                var weeklyFantasy = (await _fantasyService.GetWeeklyFantasy(_season.CurrentSeason, week)).Where(w => w.Position == position.ToString());
                return new()
                {
                    Season = _season.CurrentSeason,
                    Week = week,
                    Position = position.ToString(),
                    MSE = GetMeanSquaredError(projections, weeklyFantasy),
                    RSquared = GetRSquared(projections, weeklyFantasy),
                    MAE = GetMeanAbsoluteError(projections, weeklyFantasy),
                    MAPE = GetMeanAbsolutePercentageError(projections, weeklyFantasy)
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
        private static double GetMeanSquaredError(IEnumerable<WeekProjection> projections, IEnumerable<WeeklyFantasy> weeklyFantasy)
        {
            var sumOfSquares = 0.0;
            var count = 0;
            foreach (var projection in projections)
            {
                var fantasy = weeklyFantasy.Where(w => w.Week == projection.Week && w.PlayerId == projection.PlayerId).FirstOrDefault();
                if (fantasy != null)
                {
                    sumOfSquares += Math.Pow(fantasy.FantasyPoints - projection.ProjectedPoints, 2);
                    count++;
                }
            }
            return count > 0 ? Math.Round(sumOfSquares / count, 3) : 0;
        }
        private static double GetRSquared(IEnumerable<WeekProjection> projections, IEnumerable<WeeklyFantasy> weeklyFantasy)
        {
            List<double> observedFantasyPoints = new();
            var residualSumOfSquares = 0.0;
            var totalSumOfSquares = 0.0;
            var count = 0;
            foreach (var projection in projections)
            {
                var fantasy = weeklyFantasy.Where(w => w.Week == projection.Week && w.PlayerId == projection.PlayerId).FirstOrDefault();
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
                var fantasy = weeklyFantasy.Where(w => w.Week == projection.Week && w.PlayerId == projection.PlayerId).FirstOrDefault();
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
                var fantasy = weeklyFantasy.Where(w => w.Week == projection.Week && w.PlayerId == projection.PlayerId).FirstOrDefault();
                if (fantasy != null && fantasy.FantasyPoints > 0)
                {
                    sumOfError += Math.Abs((projection.ProjectedPoints - fantasy.FantasyPoints) / fantasy.FantasyPoints);
                    count++;
                }
            }
            return count > 0 ? (sumOfError/count)*100 : 0;
        }        
    }
}
