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
    public class SeasonProjectionService : IProjectionService<SeasonProjection>
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;
        private readonly Season _season;
        private readonly IFantasyDataService _fantasyService;
        private readonly IStatisticsService _statisticsService;
        private readonly IMatrixCalculator _matrixCalculator;
        private readonly IStatProjectionCalculator _statCalculator;
        private readonly IRegressionService _regressionService;
        private readonly IAdjustmentService _adjustmentService;
        private readonly IPlayersService _playersService;
        private readonly IProjectionRepository _projectionRepository;
        private readonly ISettingsService _settingsService;

        public SeasonProjectionService(IRegressionService regressionService, IFantasyDataService fantasyService,
        IMemoryCache cache, ILogger logger, IMatrixCalculator matrixCalculator, IStatProjectionCalculator statCalculator,
        IStatisticsService statisticsService, IOptionsMonitor<Season> season, IPlayersService playersService, 
        IAdjustmentService adjustmentService, IProjectionRepository projectionRepository, ISettingsService settingsService)
        {
            _regressionService = regressionService;
            _season = season.CurrentValue;
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

        public async Task<IEnumerable<SeasonProjection>?> GetPlayerProjections(int playerId) => await _projectionRepository.GetSeasonProjection(playerId);
        public bool GetProjectionsFromSQL(PositionEnum position, int season, out IEnumerable<SeasonProjection> projections)
        {
            projections =  _projectionRepository.GetSeasonProjectionsFromSQL(position, season);
            return projections.Any();
        }
        public bool GetProjectionsFromCache(PositionEnum position, out IEnumerable<SeasonProjection> cachedValues)
        {
            if (_cache.TryGetValue(position.ToString() + Cache.SeasonProjections.ToString(), out cachedValues))
            {
                return cachedValues.Any();
            }
            else return false;
        }
        public async Task<int> PostProjections(List<SeasonProjection> projections) => await _projectionRepository.PostSeasonProjections(projections);
        public async Task<IEnumerable<SeasonProjection>> GetProjections(PositionEnum position)
        {
            if (GetProjectionsFromCache(position, out var cachedProj))
            {
                return cachedProj;
            }
            else if(GetProjectionsFromSQL(position, _season.CurrentSeason, out var projectionsSQL))
            {
                _cache.Set(position.ToString() + Cache.SeasonProjections.ToString(), projectionsSQL);
                return projectionsSQL;
            }
            else
            {
                var projections = position switch
                {
                    PositionEnum.QB => await CalculateProjections(await QBProjectionModel(), position),
                    PositionEnum.RB => await CalculateProjections(await RBProjectionModel(), position),
                    PositionEnum.WR => await CalculateProjections(await WRProjectionModel(), position),
                    PositionEnum.TE => await CalculateProjections(await TEProjectionModel(), position),
                    _ => throw new NotImplementedException()
                };
                projections = await _adjustmentService.AdjustmentEngine(projections.ToList());
                var withRookieProjections = (await RookieSeasonProjections(position)).Union(projections);
                var formattedProjections = withRookieProjections.OrderByDescending(p => p.ProjectedPoints).Take(_settingsService.GetProjectionsCount(position));
                _cache.Set(position.ToString() + Cache.SeasonProjections.ToString(), formattedProjections);
                return formattedProjections;
            }
        }
        public async Task<IEnumerable<SeasonProjection>> CalculateProjections<T1>(List<T1> model, PositionEnum position)
        {
            List<SeasonProjection> projections = new();
            var regressorMatrix = _matrixCalculator.RegressorMatrix(model);
            var results = PerformProjection(regressorMatrix, await PerformRegression(regressorMatrix, position));
            for (int i = 0; i < results.Count; i++)
            {
                var playerId = (int)_settingsService.GetValueFromModel(model[i], Model.PlayerId);
                if (playerId > 0)
                {
                    var player = await _playersService.GetPlayer(playerId);
                    if (player.Active == 1)
                    {
                        projections.Add(new SeasonProjection
                        {
                            PlayerId = playerId,
                            Season = _season.CurrentSeason,
                            Name = player.Name,
                            Position = player.Position,
                            ProjectedPoints = results[i]
                        });
                    }
                }
            }
            return projections;
        }
        public async Task<Vector<double>> PerformRegression(Matrix<double> regressorMatrix, PositionEnum position)
        {
            try
            {
                var dependentVector = _matrixCalculator.DependentVector(await SeasonFantasyProjectionModel(position));
                return _regressionService.CholeskyDecomposition(regressorMatrix, dependentVector);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString(), ex.StackTrace);
                throw;
            }
        }

        public Vector<double> PerformProjection(Matrix<double> model, Vector<double> coeff) => model * coeff;

        private async Task<List<SeasonFantasy>> SeasonFantasyProjectionModel(PositionEnum position)
        {
            var players = await _playersService.GetPlayersByPosition(position);
            List<SeasonFantasy> seasonFantasy = new();
            foreach (var player in players)
            {
                var seasonFantasyResults = await _fantasyService.GetSeasonFantasy(player.PlayerId);
                if (seasonFantasyResults.Any())
                {
                    seasonFantasy.Add(_statCalculator.CalculateStatProjection(seasonFantasyResults));
                }
            }
            return seasonFantasy;
        }
        private async Task<List<QBModelSeason>> QBProjectionModel()
        {
            var players = await _playersService.GetPlayersByPosition(PositionEnum.QB);
            List<QBModelSeason> qbModel = new();
            foreach (var player in players)
            {
                var stats = (await _statisticsService.GetSeasonData<SeasonDataQB>(PositionEnum.QB, player.PlayerId, true));
                if (stats.Any())
                {
                    qbModel.Add(_regressionService.QBModelSeason(_statCalculator.CalculateStatProjection(stats)));
                }
            }
            return qbModel;
        }
        private async Task<List<RBModelSeason>> RBProjectionModel()
        {
            var players = await _playersService.GetPlayersByPosition(PositionEnum.RB);
            List<RBModelSeason> rbModel = new();
            foreach (var player in players)
            {
                var stats = (await _statisticsService.GetSeasonData<SeasonDataRB>(PositionEnum.RB, player.PlayerId, true));
                if (stats.Any())
                {
                    rbModel.Add(_regressionService.RBModelSeason(_statCalculator.CalculateStatProjection(stats)));
                }
            }
            return rbModel;
        }
        private async Task<List<WRModelSeason>> WRProjectionModel()
        {
            var players = await _playersService.GetPlayersByPosition(PositionEnum.WR);
            List<WRModelSeason> wrModel = new();
            foreach (var player in players)
            {
                var stats = (await _statisticsService.GetSeasonData<SeasonDataWR>(PositionEnum.WR, player.PlayerId, true));
                if (stats.Any())
                {
                    wrModel.Add(_regressionService.WRModelSeason(_statCalculator.CalculateStatProjection(stats)));
                }
            }
            return wrModel;
        }
        private async Task<List<TEModelSeason>> TEProjectionModel()
        {
            var players = await _playersService.GetPlayersByPosition(PositionEnum.TE);
            List<TEModelSeason> teModel = new();
            foreach (var player in players)
            {
                var stats = (await _statisticsService.GetSeasonData<SeasonDataTE>(PositionEnum.TE, player.PlayerId, true));
                if (stats.Any())
                {
                    teModel.Add(_regressionService.TEModelSeason(_statCalculator.CalculateStatProjection(stats)));
                }
            }
            return teModel;
        }
        private async Task<List<SeasonProjection>> RookieSeasonProjections(PositionEnum position)
        {
            List<SeasonProjection> rookieProjections = new();
            var historicalRookies = await _playersService.GetHistoricalRookies(_season.CurrentSeason, position.ToString());
            if (historicalRookies.Any())
            {
                var coeff = await _regressionService.PerformRegression(historicalRookies);
                var currentRookies = await _playersService.GetCurrentRookies(_season.CurrentSeason, position.ToString());
                var model = _matrixCalculator.RegressorMatrix(currentRookies);
                var predictions = PerformProjection(model, coeff).ToList();

                for (int i = 0; i < predictions.Count; i++)
                {
                    var rookie = currentRookies.ElementAt(i);
                    rookieProjections.Add(new SeasonProjection
                    {
                        PlayerId = rookie.PlayerId,
                        Name = (await _playersService.GetPlayer(rookie.PlayerId)).Name,
                        Season = _season.CurrentSeason,
                        Position = rookie.Position,
                        ProjectedPoints = predictions[i]
                    });
                }
            }
            return rookieProjections;
        }
    }
}
