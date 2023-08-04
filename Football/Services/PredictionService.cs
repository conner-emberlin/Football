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
    }
}
