using Football.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics.Contracts;
using System.Reflection;
using Football.Interfaces;


namespace Football.Services
{
    public class PredictionService : IPredictionService
    {
        private readonly int currentSeason = 2023;
        private readonly List<int> pastSeasons = new List<int>{ 2018, 2019, 2020, 2021, 2022 };

        public readonly IRegressionModelService _regressionModelService;
        public readonly IPerformRegressionService _performRegressionService;
        public readonly IFantasyService _fantasyService;
        public readonly IMatrixService _matrixService;

        public PredictionService(IRegressionModelService regressionModelService, IPerformRegressionService performRegressionService, IFantasyService fantasyService, IMatrixService matrixService)
        {
            _regressionModelService = regressionModelService;
            _performRegressionService = performRegressionService;
            _matrixService = matrixService;
            _fantasyService = fantasyService;
        }
        public async Task<List<double>> ModelErrorPerSeason(int playerId, string position)
        {
            List<double> errors = new();          
            List<Vector<double>> regressions = new();
            List<double> actualPoints = new();
            var seasons = await _fantasyService.GetActiveSeasons(playerId);
            switch (position)
            {
                case "QB":
                    List<RegressionModelQB> models = new();                    
                    foreach (var season in seasons)
                    {
                        models.Add(await _regressionModelService.PopulateRegressionModelQB(playerId, season));                                                                    
                        regressions.Add(await _performRegressionService.PerformRegression(season, position));
                        var results = await _fantasyService.GetFantasyResults(playerId, season);
                        actualPoints.Add(results.TotalPoints);
                    }
                    var modelVectors = models.Select(m => _matrixService.TransformQbModel(m));
                    for (int i = 0; i < seasons.Count - 1; i++)
                    {
                            errors.Add(modelVectors.ElementAt(i) * regressions.ElementAt(i) - actualPoints.ElementAt(i));
                    }                                           
                    break;
                case "RB":
                    List<RegressionModelRB> modelsR = new();
                    foreach (var season in seasons)
                    {
                        modelsR.Add(await _regressionModelService.PopulateRegressionModelRb(playerId, season));
                        regressions.Add(await _performRegressionService.PerformRegression(season, position));
                        var results = await _fantasyService.GetFantasyResults(playerId, season);
                        actualPoints.Add(results.TotalPoints);
                    }
                    var modelVectorsR = modelsR.Select(m => _matrixService.TransformRbModel(m));
                    for (int i = 0; i < seasons.Count - 1; i++)
                    {
                        errors.Add(modelVectorsR.ElementAt(i) * regressions.ElementAt(i) - actualPoints.ElementAt(i));
                    }
                    break;
                case "WR/TE":
                    List<RegressionModelPassCatchers> modelsP = new();
                    foreach (var season in seasons)
                    {
                        modelsP.Add(await _regressionModelService.PopulateRegressionModelPassCatchers(playerId, season));
                        regressions.Add(await _performRegressionService.PerformRegression(season, position));
                        var results = await _fantasyService.GetFantasyResults(playerId, season);
                        actualPoints.Add(results.TotalPoints);
                    }
                    var modelVectorsP = modelsP.Select(m => _matrixService.TransformPassCatchersModel(m));
                    for (int i = 0; i < seasons.Count - 1; i++)
                    {
                        errors.Add(modelVectorsP.ElementAt(i) * regressions.ElementAt(i) - actualPoints.ElementAt(i));
                    }
                    break;
                default: 
                    errors.Add(0);
                    break;
            }
            return errors;
        }

        public async Task<PassingStatistic> CalculateAveragePassingStats(int playerId)
        {
            var seasons = await _fantasyService.GetActivePassingSeasons(playerId);
            List<PassingStatistic> passingSeasonStats = new();
            foreach(var s in seasons)
            {
                var stat = await _regressionModelService.GetPassingStatistic(playerId, s);
                if (stat != null)
                {
                    passingSeasonStats.Add(stat);
                }
            }
            var averageGames = passingSeasonStats.Select(x => x.Games).DefaultIfEmpty(0).Average();
            return new PassingStatistic
            {
                Name = passingSeasonStats[0].Name,
                Team = passingSeasonStats[seasons.Count-1].Team,
                Age = passingSeasonStats[seasons.Count - 1].Age,
                Games = averageGames,
                Completions = passingSeasonStats.Select(x => x.Completions).DefaultIfEmpty(0).Average(),
                Attempts = passingSeasonStats.Select(x => x.Attempts).DefaultIfEmpty(0).Average(),
                Yards = passingSeasonStats.Select(x => x.Yards).DefaultIfEmpty(0).Average(),
                Touchdowns = passingSeasonStats.Select(x => x.Touchdowns).DefaultIfEmpty(0).Average(),
                Interceptions = passingSeasonStats.Select(x => x.Interceptions).DefaultIfEmpty(0).Average(),
                FirstDowns = passingSeasonStats.Select(x => x.FirstDowns).DefaultIfEmpty(0).Average(),
                Long = passingSeasonStats.Select(x => x.Long).DefaultIfEmpty(0).Average(),
                Sacks = passingSeasonStats.Select(x => x.Sacks).DefaultIfEmpty(0).Average(),
                SackYards = passingSeasonStats.Select(x => x.SackYards).DefaultIfEmpty(0).Average()
            };
        }

        public async Task<RushingStatistic> CalculateAverageRushingStats(int playerId)
        {
            var seasons = await _fantasyService.GetActiveRushingSeasons(playerId);
            List<RushingStatistic> rushingSeasonsStats = new();
            foreach (var s in seasons)
            {
                var stat = await _regressionModelService.GetRushingStatistic(playerId, s);
                if (stat != null)
                {
                    rushingSeasonsStats.Add(stat);
                }
            }
            var averageGames = rushingSeasonsStats.Select(x => x.Games).DefaultIfEmpty(0).Average();
            return new RushingStatistic
            {
                Name = rushingSeasonsStats[0].Name,
                Team = rushingSeasonsStats[seasons.Count - 1].Team,
                Age = rushingSeasonsStats.Select(x => x.Games).DefaultIfEmpty(0).Average(),
                Games = averageGames,
                RushAttempts = rushingSeasonsStats.Select(x => x.RushAttempts).DefaultIfEmpty(0).Average(),
                Yards = rushingSeasonsStats.Select(x => x.Yards).DefaultIfEmpty(0).Average(),
                Touchdowns = rushingSeasonsStats.Select(x => x.Touchdowns).DefaultIfEmpty(0).Average(),
                FirstDowns = rushingSeasonsStats.Select(x => x.FirstDowns).DefaultIfEmpty(0).Average(),
                Long = rushingSeasonsStats.Select(x => x.Long).DefaultIfEmpty(0).Average(),
                Fumbles = rushingSeasonsStats.Select(x => x.Fumbles).DefaultIfEmpty(0).Average()
            };
        }

        public async Task<ReceivingStatistic> CalculateAverageReceivingStats(int playerId)
        {
            var seasons = await _fantasyService.GetActiveReceivingSeasons(playerId);
            List<ReceivingStatistic>  receivingSeasonStats = new();
            foreach (var s in seasons)
            {
                var stat = await _regressionModelService.GetReceivingStatistic(playerId, s);
                if (stat != null)
                {
                    receivingSeasonStats.Add(stat);
                }
            }
            var averageGames = receivingSeasonStats.Select(x => x.Games).DefaultIfEmpty(0).Average();
            return  new ReceivingStatistic
            {
                Name = seasons.Count > 0 ? receivingSeasonStats[0].Name : "",
                Team = seasons.Count > 0 ? receivingSeasonStats[seasons.Count - 1].Team : "",
                Age = seasons.Count > 0 ? receivingSeasonStats[seasons.Count - 1].Age : 0,
                Games = averageGames,
                Targets = receivingSeasonStats.Select(x => x.Targets).DefaultIfEmpty(0).Average(),
                Receptions = receivingSeasonStats.Select(x => x.Receptions).DefaultIfEmpty(0).Average(),
                Yards = receivingSeasonStats.Select(x => x.Yards).DefaultIfEmpty(0).Average(),
                Touchdowns = receivingSeasonStats.Select(x => x.Touchdowns).DefaultIfEmpty(0).Average(),
                FirstDowns = receivingSeasonStats.Select(x => x.FirstDowns).DefaultIfEmpty(0).Average(),
                Long = receivingSeasonStats.Select(x => x.Long).DefaultIfEmpty(0).Average(),
                RpG = receivingSeasonStats.Select(x => x.RpG).DefaultIfEmpty(0).Average(),
                Fumbles = receivingSeasonStats.Select(x => x.Fumbles).DefaultIfEmpty(0).Average()
            };
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

        public async Task<FantasyPoints> CalculateProjectedAverageFantasyPoints(int playerId)
        {
            var activeSeasons = await _fantasyService.GetActiveSeasons(playerId);
            List<FantasyPoints> fantasyBySeason = new();
            foreach(var s in activeSeasons)
            {
                var season = await _fantasyService.GetFantasyPoints(playerId, s);
                fantasyBySeason.Add(season);
            }
            var averageFp = fantasyBySeason.Select(x => x.TotalPoints).DefaultIfEmpty(0).Average();
            var averageGames = await _fantasyService.GetAverageTotalGames(playerId);
            return new FantasyPoints
            {
                PlayerId = playerId,
                TotalPoints = averageFp
            };
        }

        public async Task<List<FantasyPoints>> AverageProjectedFantasyByPosition(string position)
        {
            var players = await _fantasyService.GetPlayersByPosition(position);
            List<FantasyPoints> projectedAverage = new();
            foreach(var p in players)
            {
                var projected = await CalculateProjectedAverageFantasyPoints(p);
                projectedAverage.Add(projected);
            }
            return projectedAverage;
        }

        public async Task<List<RegressionModelQB>> AverageProjectedModelQB()
        {
            var players = await _fantasyService.GetPlayersByPosition("QB");
            List<RegressionModelQB> regressionModel = new();
            foreach(var p in players)
            {
                var projectedAveragePassing = await CalculateAveragePassingStats(p);
                var projectedAverageRushing = await CalculateAverageRushingStats(p);
                var model = PopulateProjectedAverageModelQB(projectedAveragePassing, projectedAverageRushing, p);
                regressionModel.Add(model);
            }
            return regressionModel;          
        }

        public async Task<List<RegressionModelRB>> AverageProjectedModelRB()
        {
            var players = await _fantasyService.GetPlayersByPosition("RB");
            List<RegressionModelRB> regressionModel = new();
            foreach(var p in players)
            {
                var projectedAverageRushing = await CalculateAverageRushingStats(p);
                var projectedAverageReceiving = await CalculateAverageReceivingStats(p);
                var model = PopulateProjectedAverageModelRB(projectedAverageRushing,projectedAverageReceiving, p);
                regressionModel.Add(model);
            }
            return regressionModel;
        }

        public async Task<List<RegressionModelPassCatchers>> AverageProjectedModelPassCatchers()
        {
            var players = await _fantasyService.GetPlayersByPosition("WR/TE");
            List<RegressionModelPassCatchers> regressionModel = new();
            foreach(var p in players)
            {
                var projectedAverageReceiving = await CalculateAverageReceivingStats(p);
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
                    var qbs = await AverageProjectedModelQB();
                    var model = _matrixService.PopulateQbRegressorMatrix(qbs);
                    var results = PerformPrediction(model, await PerformPredictedRegression("QB")).ToList();
                    for (int i = 0; i < results.Count; i++)
                    {
                        var qb = qbs.ElementAt(i);
                        if (await _fantasyService.IsPlayerActive(qb.PlayerId))
                        {
                            projection.Add(new ProjectionModel
                            {
                                PlayerId = qb.PlayerId,
                                Name = await _fantasyService.GetPlayerName(qb.PlayerId),
                                Team = await _fantasyService.GetPlayerTeam(qb.PlayerId),
                                Position = await _fantasyService.GetPlayerPosition(qb.PlayerId),
                                ProjectedPoints = results[i]
                            });
                        }
                    }
                    return projection.OrderByDescending(p => p.ProjectedPoints).Take(24);
                case "RB":
                    var rbs = await AverageProjectedModelRB();
                    var modelRB = _matrixService.PopulateRbRegressorMatrix(rbs);
                    var resultsRB = PerformPrediction(modelRB, await PerformPredictedRegression("RB")).ToList();
                    for (int i = 0; i < resultsRB.Count; i++)
                    {
                        var rb = rbs.ElementAt(i);
                        if (await _fantasyService.IsPlayerActive(rb.PlayerId))
                        {
                            projection.Add(new ProjectionModel
                            {
                                PlayerId = rb.PlayerId,
                                Name = await _fantasyService.GetPlayerName(rb.PlayerId),
                                Team = await _fantasyService.GetPlayerTeam(rb.PlayerId),
                                Position =await  _fantasyService.GetPlayerPosition(rb.PlayerId),
                                ProjectedPoints = resultsRB[i]
                            });
                        }
                    }
                    return projection.OrderByDescending(p => p.ProjectedPoints).Take(36);
                case "WR/TE":
                    var pcs = await AverageProjectedModelPassCatchers();
                    var modelPC = _matrixService.PopulatePassCatchersRegressorMatrix(pcs);
                    var resultsPC = PerformPrediction(modelPC, await PerformPredictedRegression("WR/TE")).ToList();
                    for (int i = 0; i < resultsPC.Count; i++)
                    {
                        var pc = pcs.ElementAt(i);
                        if (await _fantasyService.IsPlayerActive(pc.PlayerId))
                        {
                            projection.Add(new ProjectionModel
                            {
                                PlayerId = pc.PlayerId,
                                Name = await _fantasyService.GetPlayerName(pc.PlayerId),
                                Team = await _fantasyService.GetPlayerTeam(pc.PlayerId),
                                Position = await _fantasyService.GetPlayerPosition(pc.PlayerId),
                                ProjectedPoints = Math.Round((double)resultsPC[i],2)
                            });
                        }
                    }
                    return projection.OrderByDescending(p => p.ProjectedPoints).Take(50);
                default: return null; ;

            }
        }
    }
}
