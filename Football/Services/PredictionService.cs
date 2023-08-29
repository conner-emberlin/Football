using Football.Models;
using MathNet.Numerics.LinearAlgebra;
using System.Data;
using Football.Interfaces;
using Serilog;
using Microsoft.Extensions.Caching.Memory;

namespace Football.Services
{
    public class PredictionService : IPredictionService
    {
        private readonly IPerformRegressionService _performRegressionService;
        private readonly IRegressionModelService _regressionModelService;
        private readonly IFantasyService _fantasyService;
        private readonly IMatrixService _matrixService;
        private readonly IPlayerService _playerService;
        private readonly IWeightedAverageCalculator _weightedAverageCalculator;
        private readonly IAdjustmentCalculator _adjustmentCalculator;
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;

        private readonly int QBProjections = 25;
        private readonly int RBProjections = 50;
        private readonly int WRProjections = 48;
        private readonly int TEProjections = 15;

        private readonly int QBStarters = 24;
        private readonly int RBStarters = 24;
        private readonly int WRStarters = 24;
        private readonly int TEStarters = 12;

        private readonly int _currentSeason = 2023;

        public PredictionService(IPerformRegressionService performRegressionService, IRegressionModelService regressionModelService, IFantasyService fantasyService, IPlayerService playerService, IMatrixService matrixService, IWeightedAverageCalculator weightedAverageCalculator, IAdjustmentCalculator adjustmentCalculator, ILogger logger, IMemoryCache cache)
        {
            _performRegressionService = performRegressionService;
            _matrixService = matrixService;
            _fantasyService = fantasyService;
            _playerService = playerService;
            _weightedAverageCalculator = weightedAverageCalculator;
            _adjustmentCalculator = adjustmentCalculator;
            _regressionModelService = regressionModelService;
            _logger = logger;
            _cache = cache;
        }
 
        public async Task<List<FantasyPoints>> AverageProjectedFantasyByPosition(string position)
        {
            var players = await _playerService.GetPlayersByPosition(position);
            List<FantasyPoints> projectedAverage = new();
            foreach(var p in players)
            {
                var player = await _playerService.GetPlayer(p);
                if (player.FantasyPoints.Count > 0)
                {
                    var projected = _weightedAverageCalculator.WeightedAverage(player);
                    projectedAverage.Add(projected);
                }
            }
            return projectedAverage;
        }

        public async Task<List<RegressionModelQB>> AverageProjectedModelQB()
        {
            var players = await _playerService.GetPlayersByPosition("QB");
            List<RegressionModelQB> regressionModel = new();
            foreach (var p in players)
            {               
                _logger.Information("Calculating weighted averages for playerId: {playerid}", p);
                var player = await _playerService.GetPlayer(p);
                if (player.PassingStats.Count > 0 || player.RushingStats.Count > 0)
                {
                    var projectedAveragePassing = _weightedAverageCalculator.WeightedAverage(player.PassingStats);
                    var projectedAverageRushing = _weightedAverageCalculator.WeightedAverage(player.RushingStats);
                    var model = _regressionModelService.RegressionModelQB(projectedAveragePassing, projectedAverageRushing, p);
                    regressionModel.Add(model);
                }
            }
            return regressionModel;          
        }

        public async Task<List<RegressionModelRB>> AverageProjectedModelRB()
        {
            var players = await _playerService.GetPlayersByPosition("RB");
            List<RegressionModelRB> regressionModel = new();
            foreach(var p in players)
            {
                _logger.Information("Calculating weighted averages for playerId: {playerid}", p);
                var player = await _playerService.GetPlayer(p);
                if (player.RushingStats.Count > 0 || player.ReceivingStats.Count > 0)
                {
                    var projectedAverageRushing = _weightedAverageCalculator.WeightedAverage(player.RushingStats);
                    var projectedAverageReceiving = _weightedAverageCalculator.WeightedAverage(player.ReceivingStats);
                    var model = _regressionModelService.RegressionModelRB(projectedAverageRushing, projectedAverageReceiving, p);
                    regressionModel.Add(model);
                }
            }
            return regressionModel;
        }

        public async Task<List<RegressionModelPassCatchers>> AverageProjectedModelPassCatchers()
        {
            var players = await _playerService.GetPlayersByPosition("WR/TE");
            List<RegressionModelPassCatchers> regressionModel = new();
            foreach(var p in players)
            {
                _logger.Information("Calculating weighted averages for playerId: {playerid}", p);
                var player = await _playerService.GetPlayer(p);
                if (player.ReceivingStats.Count > 0)
                {
                    var projectedAverageReceiving = _weightedAverageCalculator.WeightedAverage(player.ReceivingStats);
                    var model = _regressionModelService.RegressionModelPC(projectedAverageReceiving, p);
                    regressionModel.Add(model);
                }
            }
            return regressionModel;
        }

        public async Task<Vector<double>> PerformPredictedRegression(string position)
        {
            switch (position)
            {
                case "QB":
                    var modelQB = await AverageProjectedModelQB();
                    var modelQBFpts = await AverageProjectedFantasyByPosition("QB");
                    var regressorMatrix = _matrixService.PopulateQbRegressorMatrix(modelQB);
                    var dependentVector = _matrixService.PopulateDependentVector(modelQBFpts);
                    return _performRegressionService.CholeskyDecomposition(regressorMatrix, dependentVector);
                case "RB":
                    var modelRB = await AverageProjectedModelRB();
                    var modelRBFpts = await AverageProjectedFantasyByPosition("RB");
                    var regressorMatrixRB = _matrixService.PopulateRbRegressorMatrix(modelRB);
                    var dependentVectorRB = _matrixService.PopulateDependentVector(modelRBFpts);
                    return _performRegressionService.CholeskyDecomposition(regressorMatrixRB, dependentVectorRB);
                case "WR/TE":
                    var modelWR = await AverageProjectedModelPassCatchers();
                    var modelWRFpts = await AverageProjectedFantasyByPosition("WR/TE");
                    var regressorMatrixWR = _matrixService.PopulatePassCatchersRegressorMatrix(modelWR);
                    var dependentVectorWR = _matrixService.PopulateDependentVector(modelWRFpts);
                    return _performRegressionService.CholeskyDecomposition(regressorMatrixWR, dependentVectorWR);
                default:
                    Vector<double> m = Vector<double>.Build.Random(1);
                    return m;
            }
        }
        public async Task<List<ProjectionModel>> PerformPredictedRookieRegression(string position)
        {
            List<ProjectionModel> rookieProjections = new();
            var historicalRookies = await _playerService.GetHistoricalRookies(_currentSeason, position);
            var coeff = await _performRegressionService.PerformRegression(historicalRookies);
            var currentRookies = await _playerService.GetCurrentRookies(_currentSeason, position);
            var model = _matrixService.PopulateRookieRegressorMatrix(currentRookies);
            var predictions = PerformPrediction(model, coeff).ToList();

            for(int i = 0; i < predictions.Count; i++)
            {
                var rookie = currentRookies.ElementAt(i);
                rookieProjections.Add(new ProjectionModel
                {
                    PlayerId = rookie.PlayerId,
                    Name = await _playerService.GetPlayerName(rookie.PlayerId),
                    Team = rookie.TeamDrafted,
                    Position =rookie.Position,
                    ProjectedPoints = predictions[i]
                });
            }
            return rookieProjections;
        }
        public Vector<double> PerformPrediction(Matrix<double> model, Vector<double> coeff)
        {
            return model * coeff;
        }

        public async Task<IEnumerable<ProjectionModel>> GetProjections(string position)
        {
            List<ProjectionModel> projection = new();
            switch (position)
            {
                case "QB":
                    _logger.Information("Getting QB Projections");
                    if (_cache.TryGetValue("QbProjections", out IEnumerable<ProjectionModel> projectionModelQb))
                    {
                        _logger.Information("Retrieving QB projections from cache");
                        return projectionModelQb;
                    }
                    else
                    {
                        var qbs = await AverageProjectedModelQB();
                        var model = _matrixService.PopulateQbRegressorMatrix(qbs);
                        var results = PerformPrediction(model, await PerformPredictedRegression("QB")).ToList();
                        for (int i = 0; i < results.Count; i++)
                        {
                            var qb = qbs.ElementAt(i);
                            if (await _playerService.IsPlayerActive(qb.PlayerId))
                            {
                                projection.Add(new ProjectionModel
                                {
                                    PlayerId = qb.PlayerId,
                                    Name = await _playerService.GetPlayerName(qb.PlayerId),
                                    Team = await _playerService.GetPlayerTeam(qb.PlayerId),
                                    Position = await _playerService.GetPlayerPosition(qb.PlayerId),
                                    ProjectedPoints = results[i]
                                });
                        }
                        }
                        
                        var projections = projection.OrderByDescending(p => p.ProjectedPoints).Take(QBProjections);
                        _cache.Set("QbProjections", projections);
                        return await _adjustmentCalculator.QBAdjustments(projections);
                    }
                case "RB":
                    _logger.Information("Getting RB Projections");
                    if (_cache.TryGetValue("RbProjections", out IEnumerable<ProjectionModel> projectionModelRb))
                    {
                        _logger.Information("Retrieving RB projections from cache");
                        return projectionModelRb;
                    }
                    else
                    {
                        var rbs = await AverageProjectedModelRB();
                        var modelRB = _matrixService.PopulateRbRegressorMatrix(rbs);
                        var resultsRB = PerformPrediction(modelRB, await PerformPredictedRegression("RB")).ToList();
                        for (int i = 0; i < resultsRB.Count; i++)
                        {
                            var rb = rbs.ElementAt(i);
                            if (await _playerService.IsPlayerActive(rb.PlayerId))
                            {
                                projection.Add(new ProjectionModel
                                {
                                    PlayerId = rb.PlayerId,
                                    Name = await _playerService.GetPlayerName(rb.PlayerId),
                                    Team = await _playerService.GetPlayerTeam(rb.PlayerId),
                                    Position = await _playerService.GetPlayerPosition(rb.PlayerId),
                                    ProjectedPoints = resultsRB[i]
                                });
                            }
                        }
                        var rookieRbProjections = await PerformPredictedRookieRegression("RB");
                        foreach(var proj in rookieRbProjections)
                        {
                            projection.Add(proj);
                        }

                    }
                    var projectionsRb = projection.OrderByDescending(p => p.ProjectedPoints).Take(RBProjections);
                    _cache.Set("RbProjections", projectionsRb);
                    return await _adjustmentCalculator.RBAdjustments(projectionsRb);
                case "WR":
                case "TE":
                    _logger.Information("Getting Pass Catcher Projections");
                    if (position == "WR" && _cache.TryGetValue("WrProjections", out IEnumerable<ProjectionModel> projectionModelWr))
                    {
                        _logger.Information("Retrieving WR projections from Cache");
                        return projectionModelWr;
                    }
                    else if (position == "TE" && _cache.TryGetValue("TeProjections", out IEnumerable<ProjectionModel> projectionModelTe))
                    {
                        _logger.Information("Retrieving TE projections from Cache");
                        return projectionModelTe;
                    }
                    else 
                    {
                        var pcs = await AverageProjectedModelPassCatchers();
                        var modelPC = _matrixService.PopulatePassCatchersRegressorMatrix(pcs);
                        var resultsPC = PerformPrediction(modelPC, await PerformPredictedRegression("WR/TE")).ToList();

                        var tightEnds = await _playerService.GetTightEnds();
                        List<ProjectionModel> tightEndsOnly = new();
                        List<ProjectionModel> wideReceiversOnly = new();
                        for (int i = 0; i < resultsPC.Count; i++)
                        {
                            var pc = pcs.ElementAt(i);
                            if (await _playerService.IsPlayerActive(pc.PlayerId))
                            {
                                projection.Add(new ProjectionModel
                                {
                                    PlayerId = pc.PlayerId,
                                    Name = await _playerService.GetPlayerName(pc.PlayerId),
                                    Team = await _playerService.GetPlayerTeam(pc.PlayerId),
                                    Position = await _playerService.GetPlayerPosition(pc.PlayerId),
                                    ProjectedPoints = Math.Round((double)resultsPC[i], 2)
                                });

                        }
                        }
                        if (position == "WR")
                        {
                            foreach (var proj in projection)
                            {
                                if (!tightEnds.Contains(proj.PlayerId))
                                {
                                    proj.Position = "WR";
                                    wideReceiversOnly.Add(proj);
                                }
                            }
                            var projectionW = wideReceiversOnly.OrderByDescending(p => p.ProjectedPoints).Take(WRProjections);
                            _cache.Set("WrProjections", projectionW);
                            return await _adjustmentCalculator.PCAdjustments(projectionW, await GetProjections("QB"));
                        }
                        else
                        {
                            foreach (var proj in projection)
                            {
                                if (tightEnds.Contains(proj.PlayerId))
                                {
                                    proj.Position = "TE";
                                    tightEndsOnly.Add(proj);
                                }
                            }
                            var projectionsT = tightEndsOnly.OrderByDescending(p => p.ProjectedPoints).Take(TEProjections);
                            _cache.Set("TeProjections", projectionsT);
                            return await _adjustmentCalculator.PCAdjustments(projectionsT, await GetProjections("QB"));
                        }

                    }

                default:
                    _logger.Error("Bad position ({position}). Unable to get projections", position);
                    return null;
            }
        }
        public async Task<int> InsertFantasyProjections(string position)
        {
            int rank = 1;
            int count = 0;
            var projections = await GetProjections(position);
            foreach (var proj in projections)
            {
                count += await _fantasyService.InsertFantasyProjections(rank, proj);
                rank++;
            }
            return count;
        }

        public async Task<List<FlexProjectionModel>> FlexRankings()
        {
            List<FlexProjectionModel> flexRankings = new();
            var qbProjections = await GetProjections("QB");
            var replacementPoints = qbProjections.ElementAt(QBStarters - 1).ProjectedPoints;
            foreach(var proj in qbProjections)
            {
                flexRankings.Add(new FlexProjectionModel
                {
                    PlayerId = proj.PlayerId,
                    Name = proj.Name,
                    Team = proj.Team,
                    Position = proj.Position,
                    ProjectedPoints = proj.ProjectedPoints,
                    VORP = proj.ProjectedPoints - replacementPoints
                });
            }
            var rbProjections = await GetProjections("RB");
            var rbReplacementPoints = rbProjections.ElementAt(RBStarters- 1).ProjectedPoints;
            foreach(var proj in rbProjections)
            {
                flexRankings.Add(new FlexProjectionModel
                {
                    PlayerId = proj.PlayerId,
                    Name = proj.Name,
                    Team = proj.Team,
                    Position = proj.Position,
                    ProjectedPoints = proj.ProjectedPoints,
                    VORP = proj.ProjectedPoints - rbReplacementPoints
                });
            }
            var wrProjections = await GetProjections("WR");
            var wrReplacementPoints = wrProjections.ElementAt(WRStarters - 1).ProjectedPoints;
            foreach (var proj in wrProjections)
            {
                flexRankings.Add(new FlexProjectionModel
                {
                    PlayerId = proj.PlayerId,
                    Name = proj.Name,
                    Team = proj.Team,
                    Position = proj.Position,
                    ProjectedPoints = proj.ProjectedPoints,
                    VORP = proj.ProjectedPoints - wrReplacementPoints
                });
            }
            var teProjections = await GetProjections("TE");
            var teReplacementPoints = teProjections.ElementAt(TEStarters - 1).ProjectedPoints;
            foreach (var proj in teProjections)
            {
                flexRankings.Add(new FlexProjectionModel
                {
                    PlayerId = proj.PlayerId,
                    Name = proj.Name,
                    Team = proj.Team,
                    Position = proj.Position,
                    ProjectedPoints = proj.ProjectedPoints,
                    VORP = proj.ProjectedPoints - teReplacementPoints
                });
            }

            return flexRankings.OrderByDescending(f => f.VORP).ToList();
        }
    }
}
