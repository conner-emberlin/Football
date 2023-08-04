using Football.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using System.Text;
using System.Threading.Tasks;

namespace Football.Services
{
    public class PredictionService
    {
        public List<double> ModelErrorPerSeason(int playerId, string position)
        {
            RegressionModelService regressionModelService = new();
            PerformRegressionService performRegressionService = new();
            FantasyService fantasyService = new();
            MatrixService matrixService = new();

            List<double> errors = new();          
            List<Vector<double>> regressions = new();
            List<double> actualPoints = new();
            var seasons = fantasyService.GetActiveSeasons(playerId);
            switch (position)
            {
                case "QB":
                    List<RegressionModelQB> models = new();                    
                    foreach (var season in seasons)
                    {
                        models.Add(regressionModelService.PopulateRegressionModelQB(playerId, season));                                                                    
                        regressions.Add(performRegressionService.PerformRegression(season, position));
                        actualPoints.Add(fantasyService.GetFantasyResults(playerId, season).TotalPoints);
                    }
                    var modelVectors = models.Select(m => matrixService.TransformQbModel(m));
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
                        modelsR.Add(regressionModelService.PopulateRegressionModelRb(playerId, season));
                        regressions.Add(performRegressionService.PerformRegression(season, position));
                        actualPoints.Add(fantasyService.GetFantasyResults(playerId, season).TotalPoints);
                    }
                    var modelVectorsR = modelsR.Select(m => matrixService.TransformRbModel(m));
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
                        modelsP.Add(regressionModelService.PopulateRegressionModelPassCatchers(playerId, season));
                        regressions.Add(performRegressionService.PerformRegression(season, position));
                        actualPoints.Add(fantasyService.GetFantasyResults(playerId, season).TotalPoints);
                    }
                    var modelVectorsP = modelsP.Select(m => matrixService.TransformPassCatchersModel(m));
                    for (int i = 0; i < seasons.Count - 1; i++)
                    {
                        errors.Add(modelVectorsP.ElementAt(i) * regressions.ElementAt(i) - actualPoints.ElementAt(i));
                    }
                    return errors;
                    break;
                default: return errors;
            }          
        }

        public PassingStatistic CalculateAveragePassingStats(int playerId)
        {
            FantasyService fantasyService = new();
            RegressionModelService regressionModelService = new();
            var seasons = fantasyService.GetActiveSeasons(playerId);          
            var passingSeasonsStats = seasons.Select(s => regressionModelService.GetPassingStatistic(playerId, s)).ToList();
            return new PassingStatistic
            {
                Name = passingSeasonsStats[0].Name,
                Team = passingSeasonsStats[seasons.Count-1].Team,
                Age = passingSeasonsStats[seasons.Count - 1].Age,
                Games = passingSeasonsStats.Select(x => x.Games).DefaultIfEmpty(0).Average(),
                Completions = passingSeasonsStats.Select(x => x.Completions).DefaultIfEmpty(0).Average(),
                Attempts = passingSeasonsStats.Select(x => x.Attempts).DefaultIfEmpty(0).Average(),
                Yards = passingSeasonsStats.Select(x => x.Yards).DefaultIfEmpty(0).Average(),
                Touchdowns = passingSeasonsStats.Select(x => x.Touchdowns).DefaultIfEmpty(0).Average(),
                Interceptions = passingSeasonsStats.Select(x => x.Interceptions).DefaultIfEmpty(0).Average(),
                FirstDowns = passingSeasonsStats.Select(x => x.FirstDowns).DefaultIfEmpty(0).Average(),
                Long = passingSeasonsStats.Select(x => x.Long).DefaultIfEmpty(0).Average(),
                Sacks = passingSeasonsStats.Select(x => x.Sacks).DefaultIfEmpty(0).Average(),
                SackYards = passingSeasonsStats.Select(x => x.SackYards).DefaultIfEmpty(0).Average()
            };
        }

        public RushingStatistic CalculateAverageRushingStats(int playerId)
        {
            FantasyService fantasyService = new();
            RegressionModelService regressionModelService = new();
            var seasons = fantasyService.GetActiveSeasons(playerId);
            var rushingSeasonsStats = seasons.Select(s => regressionModelService.GetRushingStatistic(playerId, s)).ToList();
            return new RushingStatistic
            {
                Name = rushingSeasonsStats[0].Name,
                Team = rushingSeasonsStats[seasons.Count - 1].Team,
                Age = rushingSeasonsStats.Select(x => x.Games).DefaultIfEmpty(0).Average(),
                Games = rushingSeasonsStats.Select(x => x.Age).DefaultIfEmpty(0).Average(),
                RushAttempts = rushingSeasonsStats.Select(x => x.RushAttempts).DefaultIfEmpty(0).Average(),
                Yards = rushingSeasonsStats.Select(x => x.Yards).DefaultIfEmpty(0).Average(),
                Touchdowns = rushingSeasonsStats.Select(x => x.Touchdowns).DefaultIfEmpty(0).Average(),
                FirstDowns = rushingSeasonsStats.Select(x => x.FirstDowns).DefaultIfEmpty(0).Average(),
                Long = rushingSeasonsStats.Select(x => x.Long).DefaultIfEmpty(0).Average(),
                Fumbles = rushingSeasonsStats.Select(x => x.Fumbles).DefaultIfEmpty(0).Average()
            };
        }

        public ReceivingStatistic CalculateAverageReceivingStats(int playerId)
        {
            FantasyService fantasyService = new();
            RegressionModelService regressionModelService = new();
            var seasons = fantasyService.GetActiveSeasons(playerId);
            var receivingSeasonStats = seasons.Select(seasons => regressionModelService.GetReceivingStatistic(playerId, seasons)).ToList();
            return new ReceivingStatistic
            {
                Name = receivingSeasonStats[0].Name,
                Team = receivingSeasonStats[seasons.Count - 1].Team,
                Age = receivingSeasonStats[seasons.Count - 1].Age,
                Games = receivingSeasonStats.Select(x => x.Games).DefaultIfEmpty(0).Average(),
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
    }
}
