using Football.Data.Models;
using Football.Enums;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Football.Models;
using Football.Players.Interfaces;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using Football.Statistics.Interfaces;
using MathNet.Numerics.LinearRegression;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Serilog;

namespace Football.Projections.Services
{
    public class WeeklyProjectionService(IRegressionModelService regressionService, IFantasyDataService fantasyService,
    IMemoryCache cache,IMatrixCalculator matrixCalculator, IStatProjectionCalculator statCalculator,
    IStatisticsService statisticsService, IOptionsMonitor<Season> season,
    IPlayersService playersService, IAdjustmentService adjustmentService, IProjectionRepository projectionRepository,
    IOptionsMonitor<WeeklyTunings> weeklyTunings, ISettingsService settingsService, IOptionsMonitor<ProjectionLimits> settings) : IProjectionService<WeekProjection>
    {
        private readonly Season _season = season.CurrentValue;
        private readonly WeeklyTunings _weeklyTunings = weeklyTunings.CurrentValue;
        private readonly ProjectionLimits _settings = settings.CurrentValue;

        public async Task<bool> DeleteProjection(WeekProjection projection) 
        { 
            var recordDeleted = await projectionRepository.DeleteWeeklyProjection(projection.PlayerId, projection.Week, projection.Season);
            if (recordDeleted) 
                cache.Remove(projection.Position + Cache.WeeklyProjections.ToString());
            return recordDeleted;
        } 
        public async Task<IEnumerable<WeekProjection>?> GetPlayerProjections(int playerId) => await projectionRepository.GetWeeklyProjection(playerId);
        public async Task<int> PostProjections(List<WeekProjection> projections) => await projectionRepository.PostWeeklyProjections(projections);

        public bool GetProjectionsFromSQL(Position position, int week, out IEnumerable<WeekProjection> projections)
        {
            projections = projectionRepository.GetWeeklyProjectionsFromSQL(position, week);
            return projections.Any();
        }
        public bool GetProjectionsFromCache(Position position, out IEnumerable<WeekProjection> cachedValues)
        {
            if (cache.TryGetValue(position.ToString() + Cache.WeeklyProjections.ToString(), out cachedValues!))
                return cachedValues.Any();
            else return false;
        }
        public async Task<IEnumerable<WeekProjection>> GetProjections(Position position)
        {
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            if (GetProjectionsFromCache(position, out var cachedValues))
                return cachedValues;
            else if (GetProjectionsFromSQL(position, currentWeek, out var projectionsSQL))
            {
                cache.Set(position.ToString() + Cache.WeeklyProjections.ToString(), projectionsSQL);
                return projectionsSQL;
            }
            else
            {
                var projections = position switch
                {
                    Position.QB => await CalculateProjections(await QBProjectionModel(currentWeek), position),
                    Position.RB => await CalculateProjections(await RBProjectionModel(currentWeek), position),
                    Position.WR => await CalculateProjections(await WRProjectionModel(currentWeek), position),
                    Position.TE => await CalculateProjections(await TEProjectionModel(currentWeek), position),
                    Position.DST => await CalculateProjections(await DSTProjectionModel(currentWeek), position),
                    Position.K => await CalculateProjections(await KProjectionModel(currentWeek), position),
                    _ => throw new NotImplementedException()
                };
                projections = await adjustmentService.AdjustmentEngine(projections.ToList());
                var formattedProjections = projections.OrderByDescending(p => p.ProjectedPoints).Take(settingsService.GetProjectionsCount(position));
                cache.Set(position.ToString() + Cache.WeeklyProjections.ToString(), formattedProjections);
                return formattedProjections;
            }
        }
        public async Task<IEnumerable<WeekProjection>> CalculateProjections<T>(List<T> model, Position position)
        {
            List<WeekProjection> projections = [];
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            var regressorMatrix = matrixCalculator.RegressorMatrix(model);
            var fantasyModel = await FantasyProjectionModel(model, currentWeek);
            var dependentVector = matrixCalculator.DependentVector(fantasyModel, Model.FantasyPoints);
            var coefficients = MultipleRegression.NormalEquations(regressorMatrix, dependentVector);
            var results = regressorMatrix * coefficients;
            
            for (int i = 0; i < results.Count; i++)
            {
                var playerId = (int)settingsService.GetValueFromModel(model[i], Model.PlayerId);
                var player = await playersService.GetPlayer(playerId);                
                if (player != null && player.Position != Position.DST.ToString() && player.Position != Position.K.ToString())
                {                    
                    if (player.Active == 1)
                    {
                        var projectedPoints = settingsService.GetValueFromModel(model[i], Model.ProjectedPoints);
                        var projection = WeightedWeeklyProjection(projectedPoints / _season.Games, results[i], currentWeek - 1);
                        var avgError = await GetAverageProjectionError(playerId);

                        projections.Add(new WeekProjection
                        {
                            PlayerId = playerId,
                            Season = _season.CurrentSeason,
                            Week = currentWeek,
                            Name = player.Name,
                            Position = player.Position,
                            ProjectedPoints =  (2 * projection - avgError)/2
                        }); ;
                    }
                }
                else if(player != null && (player.Position == Position.DST.ToString() || player.Position == Position.K.ToString()))
                {
                    var avgError = await GetAverageProjectionError(playerId);
                    projections.Add(new WeekProjection
                    {
                        PlayerId = playerId,
                        Season = _season.CurrentSeason,
                        Week = currentWeek,
                        Name = player.Name,
                        Position = player.Position,
                        ProjectedPoints = (2 * results[i] - avgError) / 2
                    });
                }
            }
            return projections;
        }

        private async Task<double> GetAverageProjectionError(int playerId)
        {
            var weeklyProjections = await GetPlayerProjections(playerId);
            var weeklyFantasy = await fantasyService.GetWeeklyFantasy(playerId);
            if (weeklyProjections != null && weeklyFantasy.Count > 0 && weeklyProjections.Count() > _settings.ErrorAdjustmentWeek - 1)
            {
                var diffs = weeklyProjections.Join(weeklyFantasy, wp => wp.Week, wf => wf.Week, (wp, wf) => new { WeekProjection = wp, WeeklyFantasy = wf })
                                            .Select(r => r.WeekProjection.ProjectedPoints - r.WeeklyFantasy.FantasyPoints);
                return diffs.Any() ? diffs.Average() : 0;                                                          
            }
            else return 0;
        }
        private async Task<List<WeeklyFantasy>> FantasyProjectionModel<T>(List<T> statsModel, int currentWeek)
        {
            List<WeeklyFantasy> weeklyFantasy = [];
            foreach (var stat in statsModel)
            {
                var playerId = (int)settingsService.GetValueFromModel(stat, Model.PlayerId);
                if (playerId > 0)
                {
                    var weeklyResults = await fantasyService.GetWeeklyFantasy(playerId);
                    if (weeklyResults.Count > 0)
                    {
                        var weeklyAverage = statCalculator.CalculateWeeklyAverage(weeklyResults, currentWeek);
                        weeklyFantasy.Add(weeklyAverage);
                    }
                }
            }
            return weeklyFantasy;
        }
        private async Task<List<QBModelWeek>> QBProjectionModel(int currentWeek)
        {
            var players = await playersService.GetPlayersByPosition(Position.QB);
            List<QBModelWeek> qbModel = [];
            foreach (var player in players)
            {
                var stats = await statisticsService.GetWeeklyData<WeeklyDataQB>(Position.QB, player.PlayerId);
                if (stats.Count > 0)
                    qbModel.Add(await regressionService.QBModelWeek(statCalculator.CalculateWeeklyAverage(stats, currentWeek)));
            }
            return qbModel;
        }
        private async Task<List<RBModelWeek>> RBProjectionModel(int currentWeek)
        {
            var players = await playersService.GetPlayersByPosition(Position.RB);
            List<RBModelWeek> rbModel = [];
            foreach (var player in players)
            {
                var stats = await statisticsService.GetWeeklyData<WeeklyDataRB>(Position.RB, player.PlayerId);
                if (stats.Count > 0)
                    rbModel.Add(await regressionService.RBModelWeek(statCalculator.CalculateWeeklyAverage(stats, currentWeek)));
            }
            return rbModel;
        }
        private async Task<List<WRModelWeek>> WRProjectionModel(int currentWeek)
        {
            var players = await playersService.GetPlayersByPosition(Position.WR);
            List<WRModelWeek> wrModel = [];
            foreach (var player in players)
            {
                var stats = await statisticsService.GetWeeklyData<WeeklyDataWR>(Position.WR, player.PlayerId);
                if (stats.Count > 0)
                    wrModel.Add(await regressionService.WRModelWeek(statCalculator.CalculateWeeklyAverage(stats, currentWeek)));
            }
            return wrModel;
        }
        private async Task<List<TEModelWeek>> TEProjectionModel(int currentWeek)
        {
            var players = await playersService.GetPlayersByPosition(Position.TE);
            List<TEModelWeek> teModel = [];
            foreach (var player in players)
            {
                var stats = await statisticsService.GetWeeklyData<WeeklyDataTE>(Position.TE, player.PlayerId);
                if (stats.Count > 0)
                    teModel.Add(await regressionService.TEModelWeek(statCalculator.CalculateWeeklyAverage(stats, currentWeek)));
            }
            return teModel;
        }

        private async Task<List<DSTModelWeek>> DSTProjectionModel(int currentWeek)
        {
            var players = await playersService.GetPlayersByPosition(Position.DST);
            List<DSTModelWeek> dstModel = [];
            foreach(var player in players)
            {
                var stats = await statisticsService.GetWeeklyData<WeeklyDataDST>(Position.DST, player.PlayerId);
                if (stats.Count > 0)
                    dstModel.Add(await regressionService.DSTModelWeek(statCalculator.CalculateWeeklyAverage(stats, currentWeek)));
            }
            return dstModel;
        }

        private async Task<List<KModelWeek>> KProjectionModel(int currentWeek)
        {
            var players = await playersService.GetPlayersByPosition(Position.K);
            List<KModelWeek> kModel = [];
            foreach(var player in players)
            {
                var stats = await statisticsService.GetWeeklyData<WeeklyDataK>(Position.K, player.PlayerId);
                if (stats.Count > 0)
                    kModel.Add(await regressionService.KModelWeek(statCalculator.CalculateWeeklyAverage(stats, currentWeek)));
                
            }
            return kModel;
        }
        private double WeightedWeeklyProjection(double seasonProjection, double weeklyProjection, int week) 
        {
            return seasonProjection > 0 ?
            (_weeklyTunings.ProjectionWeight / week) * seasonProjection + (1 - (_weeklyTunings.ProjectionWeight / week)) * weeklyProjection
            : weeklyProjection;
        } 

    }    
}
