using Football.Enums;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Football.Models;
using Football.Players.Interfaces;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Serilog;

namespace Football.Projections.Services
{
    public class ProjectionService : IProjectionService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;       
        private readonly ProjectionLimits _projections;
        private readonly Starters _starters;
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

        public ProjectionService(IRegressionService regressionService, IFantasyDataService fantasyService, IOptionsMonitor<ProjectionLimits> projections, 
            IMemoryCache cache, ILogger logger, IMatrixCalculator matrixCalculator, IStatProjectionCalculator statCalculator,
            IStatisticsService statisticsService, IOptionsMonitor<Starters> starters, IOptionsMonitor<Season> season, 
            IPlayersService playersService, IAdjustmentService adjustmentService, IProjectionRepository projectionRepository, IOptionsMonitor<WeeklyTunings> weeklyTunings)
        {
            _regressionService = regressionService;
            _projections = projections.CurrentValue;
            _starters = starters.CurrentValue;
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
        }
        public async Task<SeasonProjection?> GetSeasonProjection(int playerId) => await _projectionRepository.GetSeasonProjection(playerId);
        public async Task<int> PostSeasonProjections(List<SeasonProjection> projections)
        {
            var count = 0;
            foreach(var proj in projections)
            {
                count += await _projectionRepository.PostSeasonProjections(proj);
            }
            return count;
        }
        public async Task<int> PostWeeklyProjections(List<WeekProjection> projections)
        {
            try
            {
                var count = 0;
                foreach (var proj in projections)
                {
                    count += await _projectionRepository.PostWeeklyProjections(proj);
                }
                return count;
            }
            catch(Exception ex)
            {
                _logger.Error(ex.ToString(), ex.StackTrace);
                throw;
            }
        }
        public async Task<List<SeasonProjection>> RookieSeasonProjections(PositionEnum position)
        {
            List<SeasonProjection> rookieProjections = new();
            var historicalRookies = await _playersService.GetHistoricalRookies(_season.CurrentSeason, position.ToString());
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
            return rookieProjections;
        }
        public async Task<List<SeasonFlex>> SeasonFlexRankings()
        {
            List<SeasonFlex> flexRankings = new();
            var qbProjections = (await GetSeasonProjections(PositionEnum.QB)).ToList();
            var rbProjections = (await GetSeasonProjections(PositionEnum.RB)).ToList();
            var wrProjections = (await GetSeasonProjections(PositionEnum.WR)).ToList();
            var teProjections = (await GetSeasonProjections(PositionEnum.TE)).ToList();
            try
            {
                var rankings = qbProjections.Concat(rbProjections).Concat(wrProjections).Concat(teProjections).ToList();
                foreach (var rank in rankings)
                {
                    if (Enum.TryParse(rank.Position, out PositionEnum position))
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
                _logger.Error(ex.ToString() + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }
        public async Task<IEnumerable<SeasonProjection>> GetSeasonProjections(PositionEnum position)
        {
            if (RetrieveFromCache(position.ToString()).Any())
            {
                return RetrieveFromCache(position.ToString());
            }
            else if (position == PositionEnum.QB)
            {
                var projections = (await CalculateSeasonProjections(await QBProjectionModel(), position)).ToList();
                projections = await _adjustmentService.AdjustmentEngine(projections);
                var formattedProjections = projections.OrderByDescending(p => p.ProjectedPoints).Take(_projections.QBProjections);
                _cache.Set("QBProjections", formattedProjections);
                return formattedProjections;
            }
            else if (position == PositionEnum.RB)
            {
                var rookieProjections = await RookieSeasonProjections(position);
                var projections = (await CalculateSeasonProjections(await RBProjectionModel(), position)).ToList();
                foreach(var r in rookieProjections)
                {
                    projections.Add(r);
                }
                projections = await _adjustmentService.AdjustmentEngine(projections);
                var formattedProjections = projections.OrderByDescending(p => p.ProjectedPoints).Take(_projections.RBProjections);
                _cache.Set("RBProjections", formattedProjections);
                return formattedProjections;
            }
            else if (position == PositionEnum.WR)
            {
                var projections = (await CalculateSeasonProjections(await WRProjectionModel(), position)).ToList();
                projections = await _adjustmentService.AdjustmentEngine(projections);
                var formattedProjections = projections.OrderByDescending(p => p.ProjectedPoints).Take(_projections.WRProjections);
                _cache.Set("WRProjections", formattedProjections);
                return formattedProjections;
            }
            else if (position == PositionEnum.TE)
            {
                var projections = (await CalculateSeasonProjections(await TEProjectionModel(), position)).ToList();
                projections = await _adjustmentService.AdjustmentEngine(projections);
                var formattedProjections = projections.OrderByDescending(p => p.ProjectedPoints).Take(_projections.TEProjections);
                _cache.Set("TEProjections", formattedProjections);
                return formattedProjections;
            }
            else
            {
                _logger.Error("Unable to retrieve projections");
                return Enumerable.Empty<SeasonProjection>();
            }
        }
        public async Task<IEnumerable<WeekProjection>> GetWeeklyProjections(PositionEnum position)
        {
            var currentWeek = await _playersService.GetCurrentWeek(_season.CurrentSeason);
            if (RetrieveWeeklyFromCache(position.ToString()).Any())
            {
                return RetrieveWeeklyFromCache(position.ToString());
            }
            else if (GetWeeklyProjectionsFromSQL(position, currentWeek, out var projectionsSQL))
            {
                return projectionsSQL;
            }
            else if (position == PositionEnum.QB)
            {
                var projections = await CalculateWeeklyProjections(await QBWeeklyProjectionModel());
                projections = await _adjustmentService.AdjustmentEngine(projections.ToList());
                var formattedProjections = projections.OrderByDescending(p => p.ProjectedPoints).Take(_projections.QBProjections);
                _cache.Set("QBWeeklyProjections", formattedProjections);
                return formattedProjections;
            }
            else if (position == PositionEnum.RB)
            {
                var projections = await CalculateWeeklyProjections(await RBWeeklyProjectionModel());
                projections = await _adjustmentService.AdjustmentEngine(projections.ToList());
                var formattedProjections = projections.OrderByDescending(p => p.ProjectedPoints).Take(_projections.RBProjections);
                _cache.Set("RBWeeklyProjections", formattedProjections);
                return formattedProjections;
            }
            else if (position == PositionEnum.WR)
            {
                var projections = await CalculateWeeklyProjections(await WRWeeklyProjectionModel());
                projections = await _adjustmentService.AdjustmentEngine(projections.ToList());
                var formattedProjections = projections.OrderByDescending(p => p.ProjectedPoints).Take(_projections.WRProjections);
                _cache.Set("WRWeeklyProjections", formattedProjections);
                return formattedProjections;
            }
            else if (position == PositionEnum.TE)
            {
                var projections = await CalculateWeeklyProjections(await TEWeeklyProjectionModel());
                projections = await _adjustmentService.AdjustmentEngine(projections.ToList());
                var formattedProjections = projections.OrderByDescending(p => p.ProjectedPoints).Take(_projections.TEProjections);
                _cache.Set("TEWeeklyProjections", formattedProjections);
                return formattedProjections;
            }
            else
            {
                _logger.Error("Unable to retrieve projections");
                return Enumerable.Empty<WeekProjection>(); 
            }
        }
        public bool GetWeeklyProjectionsFromSQL(PositionEnum position, int week, out IEnumerable<WeekProjection> projections) 
        {
            projections = _projectionRepository.GetWeeklyProjectionsFromSQL(position, week);
            return projections.Any();
        }
        public async Task<IEnumerable<SeasonProjection>> CalculateSeasonProjections<T>(List<T> model, PositionEnum position)
        {
            List<SeasonProjection> projections = new();
            var regressorMatrix = _matrixCalculator.RegressorMatrix(model);
            var results = PerformProjection(regressorMatrix, await PerformRegression(regressorMatrix, position));
            for (int i = 0; i < results.Count; i++)
            {
                var playerId = (int)typeof(T).GetProperties()[0].GetValue(model[i]);
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
            return projections;
        }
        public async Task<IEnumerable<WeekProjection>> CalculateWeeklyProjections(List<QBModelWeek> model)
        {
            List<WeekProjection> projections = new();
            var regressorMatrix = _matrixCalculator.RegressorMatrix(model);
            var results = PerformProjection(regressorMatrix, await PerformWeeklyRegression(regressorMatrix, PositionEnum.QB));
            for (int i = 0; i < results.Count; i++)
            {
                var playerId = model[i].PlayerId;
                var player = await _playersService.GetPlayer(playerId);
                if(player.Active == 1)
                {
                    projections.Add(new WeekProjection
                    {
                        PlayerId = playerId,
                        Season = _season.CurrentSeason,
                        Week = model.First().Week,
                        Name = player.Name,
                        Position = player.Position,
                        ProjectedPoints = WeightedWeeklyProjection(model[i].ProjectedPoints / _season.Games, results[i], model.First().Week - 1)
                    }); ;
                }
            }
            return projections;
        }
        public async Task<IEnumerable<WeekProjection>> CalculateWeeklyProjections(List<RBModelWeek> model)
        {
            List<WeekProjection> projections = new();
            var regressorMatrix = _matrixCalculator.RegressorMatrix(model);
            var results = PerformProjection(regressorMatrix, await PerformWeeklyRegression(regressorMatrix, PositionEnum.RB));
            for (int i = 0; i < results.Count; i++)
            {
                var playerId = model[i].PlayerId;
                var player = await _playersService.GetPlayer(playerId);
                if(player.Active == 1)
                {
                    projections.Add(new WeekProjection
                    {
                        PlayerId = playerId,
                        Season = _season.CurrentSeason,
                        Week = model.First().Week,
                        Name = player.Name,
                        Position = player.Position,
                        ProjectedPoints = WeightedWeeklyProjection(model[i].ProjectedPoints / _season.Games, results[i], model.First().Week - 1)
                    });
                }
            }
            return projections;
        }
        public async Task<IEnumerable<WeekProjection>> CalculateWeeklyProjections(List<WRModelWeek> model)
        {
            List<WeekProjection> projections = new();
            var regressorMatrix = _matrixCalculator.RegressorMatrix(model);
            var results = PerformProjection(regressorMatrix, await PerformWeeklyRegression(regressorMatrix, PositionEnum.WR));
            for (int i = 0; i < results.Count; i++)
            {
                var playerId = model[i].PlayerId;
                var player = await _playersService.GetPlayer(playerId);
                if (player.Active == 1)
                {
                    projections.Add(new WeekProjection
                    {
                        PlayerId = playerId,
                        Season = _season.CurrentSeason,
                        Week = model.First().Week,
                        Name = player.Name,
                        Position = player.Position,
                        ProjectedPoints = WeightedWeeklyProjection(model[i].ProjectedPoints / _season.Games, results[i], model.First().Week - 1)
                    });
                }
            }
            return projections;
        }
        public async Task<IEnumerable<WeekProjection>> CalculateWeeklyProjections(List<TEModelWeek> model)
        {
            List<WeekProjection> projections = new();
            var regressorMatrix = _matrixCalculator.RegressorMatrix(model);
            var results = PerformProjection(regressorMatrix, await PerformWeeklyRegression(regressorMatrix, PositionEnum.TE));
            for (int i = 0; i < results.Count; i++)
            {
                var playerId = model[i].PlayerId;
                var player = await _playersService.GetPlayer(playerId);
                if (player.Active == 1)
                {
                    projections.Add(new WeekProjection
                    {
                        PlayerId = playerId,
                        Season = _season.CurrentSeason,
                        Week = model.First().Week,
                        Name = player.Name,
                        Position = player.Position,
                        ProjectedPoints = WeightedWeeklyProjection(model[i].ProjectedPoints / _season.Games, results[i], model.First().Week - 1)
                    });
                }
            }
            return projections;
        }
        public async Task<Vector<double>> PerformRegression( Matrix<double> regressorMatrix, PositionEnum position)
        {
            try
            {
                var dependentVector = _matrixCalculator.PopulateDependentVector(await SeasonFantasyProjectionModel(position));
                return _regressionService.CholeskyDecomposition(regressorMatrix, dependentVector);
            }
            catch(Exception ex)
            {
                _logger.Error(ex.ToString(), ex.StackTrace);
                throw;
            }
        }
        public async Task<Vector<double>> PerformWeeklyRegression(Matrix<double> regressorMatrix, PositionEnum position)
        {
            try
            {
                var dependentVector = _matrixCalculator.PopulateDependentVector(await WeeklyFantasyProjectionModel(position));
                return _regressionService.CholeskyDecomposition(regressorMatrix, dependentVector);
            }
            catch(Exception ex)
            {
                _logger.Error(ex.ToString(), ex.StackTrace);
                throw;
            }
        }
        public Vector<double> PerformProjection(Matrix<double> model, Vector<double> coeff) => model * coeff;

        #region Projection Models
        private async Task<List<SeasonFantasy>> SeasonFantasyProjectionModel(PositionEnum position)
        {
            var players = await _playersService.GetPlayersByPosition(position);
            List<SeasonFantasy> seasonFantasy = new();
            foreach(var player in players) 
            {
                var seasonFantasyResults = await _fantasyService.GetSeasonFantasy(player.PlayerId);
                if (seasonFantasyResults.Any())
                {
                    seasonFantasy.Add(_statCalculator.CalculateStatProjection(seasonFantasyResults));
                }                   
            }
            return seasonFantasy;
        }
        private async Task<List<WeeklyFantasy>> WeeklyFantasyProjectionModel(PositionEnum position)
        {
            var players = await _playersService.GetPlayersByPosition(position);
            List<WeeklyFantasy> weeklyFantasy = new();
            foreach (var player in players)
            {
                var weeklyResults = await _fantasyService.GetWeeklyFantasy(player.PlayerId);
                if (weeklyResults.Any())
                {
                    weeklyFantasy.Add(_statCalculator.CalculateWeeklyAverage(weeklyResults));
                }
            }
            return weeklyFantasy;
        }

        private async Task<List<QBModelSeason>> QBProjectionModel()
        {
            var players = await _playersService.GetPlayersByPosition(PositionEnum.QB);
            List<QBModelSeason> qbModel = new();
            foreach(var player in players)
            {
                var stats = (await _statisticsService.GetSeasonDataQB(player.PlayerId));
                if (stats.Any())
                {
                    qbModel.Add(_regressionService.QBModelSeason(_statCalculator.CalculateStatProjection(stats)));
                }
            }
            return qbModel;
        }
        private async Task<List<QBModelWeek>> QBWeeklyProjectionModel()
        {
            var players = await _playersService.GetPlayersByPosition(PositionEnum.QB);
            List<QBModelWeek> qbModel = new();
            foreach(var player in players)
            {
                var stats = await _statisticsService.GetWeeklyDataQB(player.PlayerId);
                if (stats.Any())
                {
                    qbModel.Add(await _regressionService.QBModelWeek(_statCalculator.CalculateWeeklyAverage(stats)));
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
                var stats = (await _statisticsService.GetSeasonDataRB(player.PlayerId));
                if (stats.Any())
                {
                    rbModel.Add(_regressionService.RBModelSeason(_statCalculator.CalculateStatProjection(stats)));
                }
            }
            return rbModel;
        }

        private async Task<List<RBModelWeek>> RBWeeklyProjectionModel()
        {
            var players = await _playersService.GetPlayersByPosition(PositionEnum.RB);
            List<RBModelWeek> rbModel = new();
            foreach (var player in players)
            {
                var stats = await _statisticsService.GetWeeklyDataRB(player.PlayerId);
                if (stats.Any())
                {
                    rbModel.Add(await _regressionService.RBModelWeek(_statCalculator.CalculateWeeklyAverage(stats)));
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
                var stats = (await _statisticsService.GetSeasonDataWR(player.PlayerId));
                if (stats.Any())
                {
                    wrModel.Add(_regressionService.WRModelSeason(_statCalculator.CalculateStatProjection(stats)));
                }
            }
            return wrModel;
        }
        private async Task<List<WRModelWeek>> WRWeeklyProjectionModel()
        {
            var players = await _playersService.GetPlayersByPosition(PositionEnum.WR);
            List<WRModelWeek> wrModel = new();
            foreach (var player in players)
            {
                var stats = await _statisticsService.GetWeeklyDataWR(player.PlayerId);
                if (stats.Any())
                {
                    wrModel.Add(await _regressionService.WRModelWeek(_statCalculator.CalculateWeeklyAverage(stats)));
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
                var stats = (await _statisticsService.GetSeasonDataTE(player.PlayerId));
                if (stats.Any())
                {
                    teModel.Add(_regressionService.TEModelSeason(_statCalculator.CalculateStatProjection(stats)));
                }
            }
            return teModel;
        }
        private async Task<List<TEModelWeek>> TEWeeklyProjectionModel()
        {
            var players = await _playersService.GetPlayersByPosition(PositionEnum.TE);
            List<TEModelWeek> teModel = new();
            foreach (var player in players)
            {
                var stats = await _statisticsService.GetWeeklyDataTE(player.PlayerId);
                if (stats.Any())
                {
                    teModel.Add(await _regressionService.TEModelWeek(_statCalculator.CalculateWeeklyAverage(stats)));
                }
            }
            return teModel;
        }
        #endregion
        private IEnumerable<SeasonProjection> RetrieveFromCache(string position) =>  
            _cache.TryGetValue(position + "Projections", out IEnumerable<SeasonProjection> cachedProj) ? cachedProj
                  : Enumerable.Empty<SeasonProjection>();
        private IEnumerable<WeekProjection> RetrieveWeeklyFromCache(string position) =>
            _cache.TryGetValue(position + "WeeklyProjections", out IEnumerable<WeekProjection> cachedProj) ? cachedProj
            : Enumerable.Empty<WeekProjection>();
        
        private async Task<double> GetReplacementPoints(PositionEnum position)
        {
            return position switch
            {
                PositionEnum.QB => (await GetSeasonProjections(position)).ElementAt(_starters.QBStarters - 1).ProjectedPoints,
                PositionEnum.RB => (await GetSeasonProjections(position)).ElementAt(_starters.RBStarters - 1).ProjectedPoints,
                PositionEnum.WR => (await GetSeasonProjections(position)).ElementAt(_starters.WRStarters - 1).ProjectedPoints,
                PositionEnum.TE => (await GetSeasonProjections(position)).ElementAt(_starters.TEStarters - 1).ProjectedPoints,
                _ => 0,
            };
        }
        private double WeightedWeeklyProjection(double seasonProjection, double weeklyProjection, int week) => seasonProjection > 0 ?
            (_weeklyTunings.ProjectionWeight / week) * seasonProjection + (1 - (_weeklyTunings.ProjectionWeight / week)) * weeklyProjection
            : weeklyProjection;     
    }
}
