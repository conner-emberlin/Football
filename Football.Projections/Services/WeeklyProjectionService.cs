﻿using Football;
using Football.Data.Models;
using Football.Enums;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Football.Models;
using Football.Players.Interfaces;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using Football.Statistics.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Serilog;

namespace Football.Projections.Services
{
    public class WeeklyProjectionService : IProjectionService<WeekProjection>
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;
        private readonly Season _season;
        private readonly WeeklyTunings _weeklyTunings;
        private readonly IFantasyDataService _fantasyService;
        private readonly IStatisticsService _statisticsService;
        private readonly IMatrixCalculator _matrixCalculator;
        private readonly IStatProjectionCalculator _statCalculator;
        private readonly IRegressionService _regressionService;
        private readonly IAdjustmentService _adjustmentService;
        private readonly IPlayersService _playersService;
        private readonly IProjectionRepository _projectionRepository;
        private readonly ISettingsService _settingsService;

        public WeeklyProjectionService(IRegressionService regressionService, IFantasyDataService fantasyService, 
        IMemoryCache cache, ILogger logger, IMatrixCalculator matrixCalculator, IStatProjectionCalculator statCalculator,
        IStatisticsService statisticsService, IOptionsMonitor<Season> season,
        IPlayersService playersService, IAdjustmentService adjustmentService, IProjectionRepository projectionRepository, 
        IOptionsMonitor<WeeklyTunings> weeklyTunings, ISettingsService settingsService)
        {
            _regressionService = regressionService;
            _season = season.CurrentValue;
            _weeklyTunings = weeklyTunings.CurrentValue;
            _cache = cache;
            _logger = logger;
            _fantasyService = fantasyService;
            _matrixCalculator = matrixCalculator;
            _statCalculator = statCalculator;
            _statisticsService = statisticsService;
            _playersService = playersService;
            _adjustmentService = adjustmentService;
            _projectionRepository = projectionRepository;
            _settingsService = settingsService;
        }
        public async Task<IEnumerable<WeekProjection>?> GetPlayerProjections(int playerId) => await _projectionRepository.GetWeeklyProjection(playerId);

        public async Task<int> PostProjections(List<WeekProjection> projections) => await _projectionRepository.PostWeeklyProjections(projections);
        public bool GetProjectionsFromSQL(PositionEnum position, int week, out IEnumerable<WeekProjection> projections)
        {
            projections = _projectionRepository.GetWeeklyProjectionsFromSQL(position, week);
            return projections.Any();
        }
        public bool GetProjectionsFromCache(PositionEnum position, out IEnumerable<WeekProjection> cachedValues)
        {
            if (_cache.TryGetValue(position.ToString() + Cache.WeeklyProjections.ToString(), out cachedValues))
            {
                return cachedValues.Any();
            }
            else return false;
        }
        public async Task<IEnumerable<WeekProjection>> GetProjections(PositionEnum position)
        {
            var currentWeek = await _playersService.GetCurrentWeek(_season.CurrentSeason);
            if (GetProjectionsFromCache(position, out var cachedValues))
            {
                return cachedValues;
            }
            else if (GetProjectionsFromSQL(position, currentWeek, out var projectionsSQL))
            {
                return projectionsSQL;
            }
            else
            {
                var projections = position switch
                {
                    PositionEnum.QB => await CalculateProjections(await QBWeeklyProjectionModel(currentWeek), position),
                    PositionEnum.RB => await CalculateProjections(await RBWeeklyProjectionModel(currentWeek), position),
                    PositionEnum.WR => await CalculateProjections(await WRWeeklyProjectionModel(currentWeek), position),
                    PositionEnum.TE => await CalculateProjections(await TEWeeklyProjectionModel(currentWeek), position),
                    _ => throw new NotImplementedException()
                };
                projections = await _adjustmentService.AdjustmentEngine(projections.ToList());
                var formattedProjections = projections.OrderByDescending(p => p.ProjectedPoints).Take(_settingsService.GetProjectionsCount(position));
                _cache.Set(position.ToString() + Cache.WeeklyProjections, formattedProjections);
                return formattedProjections;
            }
        }
        public async Task<IEnumerable<WeekProjection>> CalculateProjections<T>(List<T> model, PositionEnum position)
        {
            List<WeekProjection> projections = new();
            var regressorMatrix = _matrixCalculator.RegressorMatrix(model);
            var week = (int)_settingsService.GetValueFromModel(model.First(), Model.Week);
            var results = PerformProjection(regressorMatrix, await PerformRegression(regressorMatrix, position));
            for (int i = 0; i < results.Count; i++)
            {
                var playerId = (int)_settingsService.GetValueFromModel(model[i], Model.PlayerId);
                var projectedPoints = _settingsService.GetValueFromModel(model[i], Model.ProjectedPoints);

                if (playerId > 0)
                {
                    var player = await _playersService.GetPlayer(playerId);
                    if (player.Active == 1)
                    {
                        projections.Add(new WeekProjection
                        {
                            PlayerId = playerId,
                            Season = _season.CurrentSeason,
                            Week = week,
                            Name = player.Name,
                            Position = player.Position,
                            ProjectedPoints = WeightedWeeklyProjection(projectedPoints / _season.Games, results[i], week - 1)
                        }); ;
                    }
                }
            }
            return projections;
        }
        public async Task<Vector<double>> PerformRegression(Matrix<double> regressorMatrix, PositionEnum position)
        {
            var currentWeek = await _playersService.GetCurrentWeek(_season.CurrentSeason);
            try
            {
                var dependentVector = _matrixCalculator.DependentVector(await WeeklyFantasyProjectionModel(position, currentWeek));
                return _regressionService.CholeskyDecomposition(regressorMatrix, dependentVector);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString(), ex.StackTrace);
                throw;
            }
        }
        public Vector<double> PerformProjection(Matrix<double> model, Vector<double> coeff) => model * coeff;
        private async Task<List<WeeklyFantasy>> WeeklyFantasyProjectionModel(PositionEnum position, int currentWeek)
        {
            var players = await _playersService.GetPlayersByPosition(position);
            List<WeeklyFantasy> weeklyFantasy = new();
            foreach (var player in players)
            {
                var weeklyResults = await _fantasyService.GetWeeklyFantasy(player.PlayerId);
                if (weeklyResults.Any())
                {
                    weeklyFantasy.Add(_statCalculator.CalculateWeeklyAverage(weeklyResults, currentWeek));
                }
            }
            return weeklyFantasy;
        }
        private async Task<List<QBModelWeek>> QBWeeklyProjectionModel(int currentWeek)
        {
            var players = await _playersService.GetPlayersByPosition(PositionEnum.QB);
            List<QBModelWeek> qbModel = new();
            foreach (var player in players)
            {
                var stats = await _statisticsService.GetWeeklyData<WeeklyDataQB>(PositionEnum.QB, player.PlayerId);
                if (stats.Any())
                {
                    qbModel.Add(await _regressionService.QBModelWeek(_statCalculator.CalculateWeeklyAverage(stats, currentWeek)));
                }
            }
            return qbModel;
        }
        private async Task<List<RBModelWeek>> RBWeeklyProjectionModel(int currentWeek)
        {
            var players = await _playersService.GetPlayersByPosition(PositionEnum.RB);
            List<RBModelWeek> rbModel = new();
            foreach (var player in players)
            {
                var stats = await _statisticsService.GetWeeklyData<WeeklyDataRB>(PositionEnum.RB, player.PlayerId);
                if (stats.Any())
                {
                    rbModel.Add(await _regressionService.RBModelWeek(_statCalculator.CalculateWeeklyAverage(stats, currentWeek)));
                }
            }
            return rbModel;
        }
        private async Task<List<WRModelWeek>> WRWeeklyProjectionModel(int currentWeek)
        {
            var players = await _playersService.GetPlayersByPosition(PositionEnum.WR);
            List<WRModelWeek> wrModel = new();
            foreach (var player in players)
            {
                var stats = await _statisticsService.GetWeeklyData<WeeklyDataWR>(PositionEnum.WR, player.PlayerId);
                if (stats.Any())
                {
                    wrModel.Add(await _regressionService.WRModelWeek(_statCalculator.CalculateWeeklyAverage(stats, currentWeek)));
                }
            }
            return wrModel;
        }
        private async Task<List<TEModelWeek>> TEWeeklyProjectionModel(int currentWeek)
        {
            var players = await _playersService.GetPlayersByPosition(PositionEnum.TE);
            List<TEModelWeek> teModel = new();
            foreach (var player in players)
            {
                var stats = await _statisticsService.GetWeeklyData<WeeklyDataTE>(PositionEnum.TE, player.PlayerId);
                if (stats.Any())
                {
                    teModel.Add(await _regressionService.TEModelWeek(_statCalculator.CalculateWeeklyAverage(stats, currentWeek)));
                }
            }
            return teModel;
        }
        private double WeightedWeeklyProjection(double seasonProjection, double weeklyProjection, int week) => seasonProjection > 0 ?
            (_weeklyTunings.ProjectionWeight / week) * seasonProjection + (1 - (_weeklyTunings.ProjectionWeight / week)) * weeklyProjection
            : weeklyProjection;

    }    
}
