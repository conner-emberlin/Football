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
        private readonly int currentSeason = 2023;
        private readonly IPerformRegressionService _performRegressionService;
        private readonly IFantasyService _fantasyService;
        private readonly IMatrixService _matrixService;
        private readonly IPlayerService _playerService;
        private readonly IWeightedAverageCalculator _weightedAverageCalculator;
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;

        public PredictionService(IPerformRegressionService performRegressionService, IFantasyService fantasyService, IPlayerService playerService, IMatrixService matrixService, IWeightedAverageCalculator weightedAverageCalculator, ILogger logger, IMemoryCache cache)
        {
            _performRegressionService = performRegressionService;
            _matrixService = matrixService;
            _fantasyService = fantasyService;
            _playerService = playerService;
            _weightedAverageCalculator = weightedAverageCalculator;
            _logger = logger;
            _cache = cache;
        }
 
        public RegressionModelQB PopulateProjectedAverageModelQB(PassingStatistic passingStat, RushingStatistic rushingStat, int playerId)
        {
            var dataP = passingStat != null;
            var dataR = passingStat != null;
            return new RegressionModelQB
            {
                PlayerId = playerId,
                Season = currentSeason,
                PassingAttemptsPerGame = dataP ? Math.Round((double)(passingStat.Attempts / passingStat.Games), 4) : 0,
                PassingYardsPerGame = dataP ? Math.Round((double)(passingStat.Yards / passingStat.Games), 4) : 0,
                PassingTouchdownsPerGame = dataP ? Math.Round((double)(passingStat.Touchdowns / passingStat.Games), 4) : 0,
                RushingAttemptsPerGame = dataR ? Math.Round((double)(rushingStat.RushAttempts / rushingStat.Games), 4) : 0,
                RushingYardsPerGame = dataR ? Math.Round((double)(rushingStat.Yards / rushingStat.Games), 4) : 0,
                RushingTouchdownsPerGame = dataR ? Math.Round((double)(rushingStat.Touchdowns / rushingStat.Games), 4) : 0,
                SackYardsPerGame = dataP ? Math.Round((double)(passingStat.SackYards / passingStat.Games), 4) : 0
            };
        }

        public RegressionModelRB PopulateProjectedAverageModelRB(RushingStatistic rushingStat, ReceivingStatistic receivingStat, int playerId)
        {
            var dataRush = rushingStat != null;
            var dataRec = receivingStat != null;
            return new RegressionModelRB
            {
                PlayerId = playerId,
                Season = currentSeason,
                Age = dataRush ? rushingStat.Age : 0,
                RushingAttemptsPerGame = dataRush ? Math.Round((double)(rushingStat.RushAttempts / rushingStat.Games), 4) : 0,
                RushingYardsPerGame = dataRush ? Math.Round((double)(rushingStat.Yards / rushingStat.Games), 4) : 0,
                RushingYardsPerAttempt = dataRush ? Math.Round((double)(rushingStat.Yards / rushingStat.RushAttempts), 4) : 0,
                RushingTouchdownsPerGame = dataRush ? Math.Round((double)(rushingStat.Touchdowns / rushingStat.Games), 4) : 0,
                ReceivingTouchdownsPerGame = dataRec ? Math.Round((double)(receivingStat.Touchdowns / receivingStat.Games), 4) : 0,
                ReceivingYardsPerGame = dataRec ? Math.Round((double)(receivingStat.Yards / receivingStat.Games), 4) : 0,
                ReceptionsPerGame = dataRec ? Math.Round((double)(receivingStat.Receptions / receivingStat.Games), 4) : 0
            };
        }

        public RegressionModelPassCatchers PopulateProjectedAverageModelPassCatchers(ReceivingStatistic receivingStat, int playerId)
        {
            var data = receivingStat != null;
            return new RegressionModelPassCatchers
            {
                PlayerId = playerId,
                Season = currentSeason,
                TargetsPerGame = data ? Math.Round((double)(receivingStat.Targets / receivingStat.Games), 4) : 0,
                ReceptionsPerGame = data ? Math.Round((double)(receivingStat.Receptions / receivingStat.Games), 4) : 0,
                YardsPerGame = data ? Math.Round((double)(receivingStat.Yards / receivingStat.Games), 4) : 0,
                YardsPerReception = data ? Math.Round((double)receivingStat.Yards / receivingStat.Receptions, 4) : 0,
                TouchdownsPerGame = data ? Math.Round((double)(receivingStat.Touchdowns / receivingStat.Games), 4) : 0
            };
        }

        public async Task<List<FantasyPoints>> AverageProjectedFantasyByPosition(string position)
        {
            var players = await _playerService.GetPlayersByPosition(position);
            List<FantasyPoints> projectedAverage = new();
            foreach(var p in players)
            {
                var player = await _playerService.GetPlayer(p);
                var projected = _weightedAverageCalculator.FantasyWeightedAverage(player);
                projectedAverage.Add(projected);
            }
            return projectedAverage;
        }

        public async Task<List<RegressionModelQB>> AverageProjectedModelQB()
        {
            var players = await _playerService.GetPlayersByPosition("QB");
            List<RegressionModelQB> regressionModel = new();
            foreach (var p in players)
            {               
                _logger.Information("Calculating weighted averages for playerId: {playerid}\r\n", p);
                var player = await _playerService.GetPlayer(p);
                var projectedAveragePassing = _weightedAverageCalculator.WeightedAverage(player.PassingStats);
                var projectedAverageRushing = _weightedAverageCalculator.WeightedAverage(player.RushingStats);
                var model = PopulateProjectedAverageModelQB(projectedAveragePassing, projectedAverageRushing, p);
                regressionModel.Add(model);
            }
            return regressionModel;          
        }

        public async Task<List<RegressionModelRB>> AverageProjectedModelRB()
        {
            var players = await _playerService.GetPlayersByPosition("RB");
            List<RegressionModelRB> regressionModel = new();
            foreach(var p in players)
            {
                _logger.Information("Calculating weighted averages for playerId: {playerid}\r\n", p);
                var player = await _playerService.GetPlayer(p);
                var projectedAverageRushing =  _weightedAverageCalculator.WeightedAverage(player.RushingStats);
                var projectedAverageReceiving = _weightedAverageCalculator.WeightedAverage(player.ReceivingStats);
                var model = PopulateProjectedAverageModelRB(projectedAverageRushing,projectedAverageReceiving, p);
                regressionModel.Add(model);
            }
            return regressionModel;
        }

        public async Task<List<RegressionModelPassCatchers>> AverageProjectedModelPassCatchers()
        {
            var players = await _playerService.GetPlayersByPosition("WR/TE");
            List<RegressionModelPassCatchers> regressionModel = new();
            foreach(var p in players)
            {
                _logger.Information("Calculating weighted averages for playerId: {playerid}\r\n", p);
                var player = await _playerService.GetPlayer(p);
                var projectedAverageReceiving = _weightedAverageCalculator.WeightedAverage(player.ReceivingStats);
                var model = PopulateProjectedAverageModelPassCatchers(projectedAverageReceiving, p);
                regressionModel.Add(model);
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
                        _logger.Information("Retrieving projections from cache");
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
                        var projections = projection.OrderByDescending(p => p.ProjectedPoints).Take(24);
                        _cache.Set("QbProjections", projections);
                        return projections;
                    }
                case "RB":
                    _logger.Information("Getting RB Projections");
                    if (_cache.TryGetValue("RbProjections", out IEnumerable<ProjectionModel> projectionModelRb))
                    {
                        _logger.Information("Getting Rb projections from cache");
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
                    }
                    var projectionsRb = projection.OrderByDescending(p => p.ProjectedPoints).Take(24);
                    _cache.Set("RbProjections", projectionsRb);
                    return projectionsRb;
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
                            var projectionW = wideReceiversOnly.OrderByDescending(p => p.ProjectedPoints).Take(36);
                            _cache.Set("WrProjections", projectionW);
                            return projectionW;
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
                            var projectionsT = tightEndsOnly.OrderByDescending(p => p.ProjectedPoints).Take(12);
                            _cache.Set("TeProjections", projectionsT);
                            return projectionsT;
                        }

                    }

                default:
                    _logger.Error("Bad position. Unable to get projections");
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
    }
}
