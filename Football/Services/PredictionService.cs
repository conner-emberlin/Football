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
        public double SeasonFantasyPointsPrediction(int playerId, string position)
        {
            RegressionModelService regressionModelService = new();
            PerformRegressionService performRegressionService = new();
            FantasyService fantasyService = new();
            MatrixService matrixService = new();

            var seasons = fantasyService.GetActiveSeasons(playerId);
            switch (position)
            {
                case "QB":
                    List<RegressionModelQB> models = new();
                    List<Vector<double>> regressions = new();
                    List<double> actualPoints = new();
                    List<double> errors = new();
                    int normalization = typeof(RegressionModelQB).GetProperties().Length - 1;
                    foreach (var season in seasons)
                    {
                        models.Add(regressionModelService.PopulateRegressionModelQB(playerId, season));                                                                    
                        regressions.Add(performRegressionService.PerformRegression(season, position));
                        actualPoints.Add(fantasyService.GetFantasyResults(playerId, season).TotalPoints);
                    }
                    var modelVectors = models.Select(m => matrixService.TransformQbModel(m));
                    for (int i = 0; i < seasons.Count - 1; i++)
                    {
                        if (i == 0)
                        {
                            //calculate initial error for the first year
                            var error = Math.Round((double)(Math.Abs(modelVectors.ElementAt(i) * regressions.ElementAt(i) - actualPoints.ElementAt(i)) / normalization), 2);
                            errors.Add(error);
                        }
                        else
                        {
                            //see what the error is when you consider the previous years
                            var error = Math.Round((double)(Math.Abs(((errors.ElementAt(i-1))/normalization)* modelVectors.ElementAt(i) * regressions.ElementAt(i) - actualPoints.ElementAt(i)) / normalization), 2);
                            errors.Add(error);
                        }
                    }
                            //last stop, return the prediction for the next season
                     return Math.Round((double)(modelVectors.ElementAt(seasons.Count - 1) * (regressions.ElementAt(seasons.Count - 1) + errors.ElementAt(seasons.Count - 1))), 2);
                            
                    
                    break;
                case "RB": return 0;
                    break;
                case "WR/TE": return 0;
                    break;
                default: return 0;
            }
            

        }
    }
}
