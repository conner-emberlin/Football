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
        public List<double> ModelErrorPerSeason(int playerId, string position)
        {
            List<double> errors = new();          
            List<Vector<double>> regressions = new();
            List<double> actualPoints = new();
            var seasons = _fantasyService.GetActiveSeasons(playerId);
            switch (position)
            {
                case "QB":
                    List<RegressionModelQB> models = new();                    
                    foreach (var season in seasons)
                    {
                        models.Add(_regressionModelService.PopulateRegressionModelQB(playerId, season));                                                                    
                        regressions.Add(_performRegressionService.PerformRegression(season, position));
                        actualPoints.Add(_fantasyService.GetFantasyResults(playerId, season).TotalPoints);
                    }
                    var modelVectors = models.Select(m => _matrixService.TransformQbModel(m));
                    for (int i = 0; i < seasons.Count - 1; i++)
                    {
                            errors.Add(modelVectors.ElementAt(i) * regressions.ElementAt(i) - actualPoints.ElementAt(i));
                    }
                    return errors;                                             
                    break;
                case "RB":
                    List<RegressionModelRB> modelsR = new();
                    foreach (var season in seasons)
                    {
                        modelsR.Add(_regressionModelService.PopulateRegressionModelRb(playerId, season));
                        regressions.Add(_performRegressionService.PerformRegression(season, position));
                        actualPoints.Add(_fantasyService.GetFantasyResults(playerId, season).TotalPoints);
                    }
                    var modelVectorsR = modelsR.Select(m => _matrixService.TransformRbModel(m));
                    for (int i = 0; i < seasons.Count - 1; i++)
                    {
                        errors.Add(modelVectorsR.ElementAt(i) * regressions.ElementAt(i) - actualPoints.ElementAt(i));
                    }
                    return errors;
                    break;
                case "WR/TE":
                    List<RegressionModelPassCatchers> modelsP = new();
                    foreach (var season in seasons)
                    {
                        modelsP.Add(_regressionModelService.PopulateRegressionModelPassCatchers(playerId, season));
                        regressions.Add(_performRegressionService.PerformRegression(season, position));
                        actualPoints.Add(_fantasyService.GetFantasyResults(playerId, season).TotalPoints);
                    }
                    var modelVectorsP = modelsP.Select(m => _matrixService.TransformPassCatchersModel(m));
                    for (int i = 0; i < seasons.Count - 1; i++)
                    {
                        errors.Add(modelVectorsP.ElementAt(i) * regressions.ElementAt(i) - actualPoints.ElementAt(i));
                    }
                    return errors;
                    break;
                default: return null;
            }          
        }

        public PassingStatistic CalculateAveragePassingStats(int playerId)
        {
            var seasons = _fantasyService.GetActivePassingSeasons(playerId);          
            var passingSeasonsStats = seasons.Select(s => _regressionModelService.GetPassingStatistic(playerId, s)).ToList();
            var averageGames = passingSeasonsStats.Select(x => x.Games).DefaultIfEmpty(0).Average();
            return new PassingStatistic
            {
                Name = passingSeasonsStats[0].Name,
                Team = passingSeasonsStats[seasons.Count-1].Team,
                Age = passingSeasonsStats[seasons.Count - 1].Age,
                Games = averageGames,
                Completions = ProjectStatToFullSeason(averageGames, passingSeasonsStats.Select(x => x.Completions).DefaultIfEmpty(0).Average()),
                Attempts = ProjectStatToFullSeason(averageGames, passingSeasonsStats.Select(x => x.Attempts).DefaultIfEmpty(0).Average()),
                Yards = ProjectStatToFullSeason(averageGames, passingSeasonsStats.Select(x => x.Yards).DefaultIfEmpty(0).Average()),
                Touchdowns = ProjectStatToFullSeason(averageGames,passingSeasonsStats.Select(x => x.Touchdowns).DefaultIfEmpty(0).Average()),
                Interceptions = ProjectStatToFullSeason(averageGames,passingSeasonsStats.Select(x => x.Interceptions).DefaultIfEmpty(0).Average()),
                FirstDowns = ProjectStatToFullSeason(averageGames,passingSeasonsStats.Select(x => x.FirstDowns).DefaultIfEmpty(0).Average()),
                Long = ProjectStatToFullSeason(averageGames,passingSeasonsStats.Select(x => x.Long).DefaultIfEmpty(0).Average()),
                Sacks = ProjectStatToFullSeason(averageGames,passingSeasonsStats.Select(x => x.Sacks).DefaultIfEmpty(0).Average()),
                SackYards = ProjectStatToFullSeason(averageGames,passingSeasonsStats.Select(x => x.SackYards).DefaultIfEmpty(0).Average())
            };
        }

        public RushingStatistic CalculateAverageRushingStats(int playerId)
        {
            var seasons = _fantasyService.GetActiveRushingSeasons(playerId);
            var rushingSeasonsStats = seasons.Select(s => _regressionModelService.GetRushingStatistic(playerId, s)).ToList();
            var averageGames = rushingSeasonsStats.Select(x => x.Games).DefaultIfEmpty(0).Average();
            return new RushingStatistic
            {
                Name = rushingSeasonsStats[0].Name,
                Team = rushingSeasonsStats[seasons.Count - 1].Team,
                Age = rushingSeasonsStats.Select(x => x.Games).DefaultIfEmpty(0).Average(),
                Games = averageGames,
                RushAttempts = ProjectStatToFullSeason(averageGames,rushingSeasonsStats.Select(x => x.RushAttempts).DefaultIfEmpty(0).Average()),
                Yards = ProjectStatToFullSeason(averageGames, rushingSeasonsStats.Select(x => x.Yards).DefaultIfEmpty(0).Average()),
                Touchdowns = ProjectStatToFullSeason(averageGames,rushingSeasonsStats.Select(x => x.Touchdowns).DefaultIfEmpty(0).Average()),
                FirstDowns = ProjectStatToFullSeason(averageGames,rushingSeasonsStats.Select(x => x.FirstDowns).DefaultIfEmpty(0).Average()),
                Long = ProjectStatToFullSeason(averageGames,rushingSeasonsStats.Select(x => x.Long).DefaultIfEmpty(0).Average()),
                Fumbles = ProjectStatToFullSeason(averageGames,rushingSeasonsStats.Select(x => x.Fumbles).DefaultIfEmpty(0).Average())
            };
        }

        public ReceivingStatistic CalculateAverageReceivingStats(int playerId)
        {
            var seasons = _fantasyService.GetActiveReceivingSeasons(playerId);
            var receivingSeasonStats = seasons.Select(seasons => _regressionModelService.GetReceivingStatistic(playerId, seasons)).ToList();
            var averageGames = receivingSeasonStats.Select(x => x.Games).DefaultIfEmpty(0).Average();
            return  new ReceivingStatistic
            {
                Name = seasons.Count > 0 ? receivingSeasonStats[0].Name : "",
                Team = seasons.Count > 0 ? receivingSeasonStats[seasons.Count - 1].Team : "",
                Age = seasons.Count > 0 ? receivingSeasonStats[seasons.Count - 1].Age : 0,
                Games = averageGames,
                Targets = ProjectStatToFullSeason(averageGames, receivingSeasonStats.Select(x => x.Targets).DefaultIfEmpty(0).Average()),
                Receptions = ProjectStatToFullSeason(averageGames, receivingSeasonStats.Select(x => x.Receptions).DefaultIfEmpty(0).Average()),
                Yards = ProjectStatToFullSeason(averageGames, receivingSeasonStats.Select(x => x.Yards).DefaultIfEmpty(0).Average()),
                Touchdowns = ProjectStatToFullSeason(averageGames, receivingSeasonStats.Select(x => x.Touchdowns).DefaultIfEmpty(0).Average()),
                FirstDowns = ProjectStatToFullSeason(averageGames, receivingSeasonStats.Select(x => x.FirstDowns).DefaultIfEmpty(0).Average()),
                Long = ProjectStatToFullSeason(averageGames, receivingSeasonStats.Select(x => x.Long).DefaultIfEmpty(0).Average()),
                RpG = ProjectStatToFullSeason(averageGames, receivingSeasonStats.Select(x => x.RpG).DefaultIfEmpty(0).Average()),
                Fumbles = ProjectStatToFullSeason(averageGames, receivingSeasonStats.Select(x => x.Fumbles).DefaultIfEmpty(0).Average())
            };
        }

        public double ProjectStatToFullSeason(double averageGames, double averageStat)
        {   //tech debt
            return averageStat;
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

        public FantasyPoints CalculateProjectedAverageFantasyPoints(int playerId)
        {
            var activeSeasons = _fantasyService.GetActiveSeasons(playerId);
            var fantasyBySeason = activeSeasons.Select(s => _fantasyService.GetFantasyPoints(playerId, s)).ToList();
            var averageFp = fantasyBySeason.Select(x => x.TotalPoints).DefaultIfEmpty(0).Average();
            var averageGames = _fantasyService.GetAverageTotalGames(playerId);
            return new FantasyPoints
            {
                PlayerId = playerId,
                TotalPoints = ProjectStatToFullSeason(averageGames, averageFp)
            };
        }

        public List<FantasyPoints> AverageProjectedFantasyByPosition(string position)
        {
            var players = _fantasyService.GetPlayersByPosition(position);
            return players.Select(x => CalculateProjectedAverageFantasyPoints(x)).ToList();
        }

        public List<RegressionModelQB> AverageProjectedModelQB()
        {
            var players = _fantasyService.GetPlayersByPosition("QB");
            return players.Select(x => PopulateProjectedAverageModelQB(CalculateAveragePassingStats(x), CalculateAverageRushingStats(x), x)).ToList();           
        }

        public List<RegressionModelRB> AverageProjectedModelRB()
        {
            var players = _fantasyService.GetPlayersByPosition("RB");
            return players.Select(x => PopulateProjectedAverageModelRB(CalculateAverageRushingStats(x), CalculateAverageReceivingStats(x), x)).ToList();

        }

        public List<RegressionModelPassCatchers> AverageProjectedModelPassCatchers()
        {
            var players = _fantasyService.GetPlayersByPosition("WR/TE");
            return players.Select(x => PopulateProjectedAverageModelPassCatchers(CalculateAverageReceivingStats(x), x)).ToList();
        }

        public Vector<double> PerformPredictedRegression(string position)
        {
            switch (position)
            {
                case "QB":
                    var modelQB = AverageProjectedModelQB();
                    var modelQBFpts = AverageProjectedFantasyByPosition("QB");
                    var regressorMatrix = _matrixService.PopulateQbRegressorMatrix(modelQB);
                    var dependentVector = _matrixService.PopulateDependentVector(modelQBFpts);
                    return _performRegressionService.CholeskyDecomposition(regressorMatrix, dependentVector);
                    break;
                case "RB":
                    var modelRB = AverageProjectedModelRB();
                    var modelRBFpts = AverageProjectedFantasyByPosition("RB");
                    var regressorMatrixRB = _matrixService.PopulateRbRegressorMatrix(modelRB);
                    var dependentVectorRB = _matrixService.PopulateDependentVector(modelRBFpts);
                    return _performRegressionService.CholeskyDecomposition(regressorMatrixRB, dependentVectorRB);
                case "WR/TE":
                    var modelWR = AverageProjectedModelPassCatchers();
                    var modelWRFpts = AverageProjectedFantasyByPosition("WR/TE");
                    var regressorMatrixWR = _matrixService.PopulatePassCatchersRegressorMatrix(modelWR);
                    var dependentVectorWR = _matrixService.PopulateDependentVector(modelWRFpts);
                    return _performRegressionService.CholeskyDecomposition(regressorMatrixWR, dependentVectorWR);
                default: return null;
            }
        }

        public Vector<double> PerformPrediction(Matrix<double> model, Vector<double> coeff)
        {
            return model * coeff;
        }

        public IEnumerable<ProjectionModel> GetProjections(string position)
        {
            List<ProjectionModel> projection = new();
            switch (position)
            {
                case "QB":
                    var qbs = AverageProjectedModelQB();
                    var model = _matrixService.PopulateQbRegressorMatrix(qbs);
                    var results = PerformPrediction(model, PerformPredictedRegression("QB")).ToList();
                    for (int i = 0; i < results.Count; i++)
                    {
                        var qb = qbs.ElementAt(i);
                        if (_fantasyService.IsPlayerActive(qb.PlayerId))
                        {
                            projection.Add(new ProjectionModel
                            {
                                PlayerId = qb.PlayerId,
                                Name = _fantasyService.GetPlayerName(qb.PlayerId),
                                Team = _fantasyService.GetPlayerTeam(qb.PlayerId),
                                Position = _fantasyService.GetPlayerPosition(qb.PlayerId),
                                ProjectedPoints = results[i]
                            });
                        }
                    }
                    return projection.OrderByDescending(p => p.ProjectedPoints).Take(24);
                    break;
                case "RB":
                    var rbs = AverageProjectedModelRB();
                    var modelRB = _matrixService.PopulateRbRegressorMatrix(rbs);
                    var resultsRB = PerformPrediction(modelRB, PerformPredictedRegression("RB")).ToList();
                    for (int i = 0; i < resultsRB.Count; i++)
                    {
                        var rb = rbs.ElementAt(i);
                        if (_fantasyService.IsPlayerActive(rb.PlayerId))
                        {
                            projection.Add(new ProjectionModel
                            {
                                PlayerId = rb.PlayerId,
                                Name = _fantasyService.GetPlayerName(rb.PlayerId),
                                Team = _fantasyService.GetPlayerTeam(rb.PlayerId),
                                Position = _fantasyService.GetPlayerPosition(rb.PlayerId),
                                ProjectedPoints = resultsRB[i]
                            });
                        }
                    }
                    return projection.OrderByDescending(p => p.ProjectedPoints).Take(36);
                    break;
                case "WR/TE":
                    var pcs = AverageProjectedModelPassCatchers();
                    var modelPC = _matrixService.PopulatePassCatchersRegressorMatrix(pcs);
                    var resultsPC = PerformPrediction(modelPC, PerformPredictedRegression("WR/TE")).ToList();
                    for (int i = 0; i < resultsPC.Count; i++)
                    {
                        var pc = pcs.ElementAt(i);
                        if (_fantasyService.IsPlayerActive(pc.PlayerId))
                        {
                            projection.Add(new ProjectionModel
                            {
                                PlayerId = pc.PlayerId,
                                Name = _fantasyService.GetPlayerName(pc.PlayerId),
                                Team = _fantasyService.GetPlayerTeam(pc.PlayerId),
                                Position = _fantasyService.GetPlayerPosition(pc.PlayerId),
                                ProjectedPoints = Math.Round((double)resultsPC[i],2)
                            });
                        }
                    }
                    return projection.OrderByDescending(p => p.ProjectedPoints).Take(50);
                    break;
                default: return null; ;

            }
        }
    }
}
