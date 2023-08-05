
using Football.Models;
using Football.Repository;
using Football.Services;
using MathNet.Numerics.LinearAlgebra;

namespace DataUpload
{

    public partial class Program
    {
        

        public static async Task<int> Main()
        {
            /* UPLOAD FILE
            DataUploadService dus = new();
            var filepath = @"C:\\NFLData\\PassingStats2022.csv";

            var stats = dus.PassingStatFileUpload(filepath);
            var count = dus.PassingStatInsert(stats, 2022);
            
            var stats = dus.ReceivingStatFileUpload(filepath);
            var count = dus.ReceivingStatInsert(stats, 2018);
            

            var stats = dus.RushingStatFileUpload(filepath);
            var count = dus.RushingStatInsert(stats, 2022);
           
            System.Console.WriteLine(count + " records uploaded");
            */

            //CALCULATE FANTASY POINTS
            /*
           FantasyService fcs = new();
           List<int> playerIds = fcs.GetPlayersByPosition("WR/TE");

           int season = 2018;
           int count = 0;


           foreach(var playerId in  playerIds)
           {

               var fantasyPoints = fcs.GetFantasyPoints(playerId, season);
               if (fantasyPoints.TotalPoints > 0)
               {
                   count += fcs.InsertFantasyPoints(fantasyPoints);
               }
           }

           System.Console.WriteLine(count + " records added");
            */
            

            //PERFORM REGRESSION (use api)
            /*
            PerformRegressionService performRegressionService = new();
            int season = 2022;
            string position = "RB";

            var result = performRegressionService.PerformRegression(season, position);

            for(int i = 0; i< result.Count; i++)
            {
                System.Console.WriteLine(result[i]);
            }
            */

            //Calculate MSE
            /*
            PerformRegressionService performRegressionService = new();
            MatrixService matrixService = new();
            RegressionModelService regressionModelService = new();

            int season = 2020;
            string position = "WR/TE";
            var fantasyResults = regressionModelService.PopulateFantasyResults(season, position);
            //run regression to get coefficients
            var coefficients = performRegressionService.PerformRegression(season, position);
            //get the actual results
            var actual = matrixService.PopulateDependentVector(fantasyResults);
            //get the model
            var model = matrixService.PopulatePassCatchersRegressorMatrix(fantasyResults.Select(fr => regressionModelService.PopulateRegressionModelPassCatchers(fr.PlayerId, fr.Season)).ToList());
            System.Console.WriteLine(performRegressionService.CalculateMSE(actual, coefficients, model));
            System.Console.WriteLine(performRegressionService.CalculatError(actual, coefficients, model));
            System.Console.WriteLine(coefficients);
            */

            /*CHECK MODEL ERROR BY PLAYER
            PredictionService pred = new();
            FantasyService fs = new();
            var players = fs.GetPlayersByPosition("QB");
            foreach (var player in players)
            {
                var test = pred.ModelErrorPerSeason(player, "QB");
                foreach (var item in test)
                {
                    System.Console.WriteLine(item);
                }
            }
            */
            /*
            PredictionService ps = new();
            MatrixService ms = new();
            FantasyService fs = new();
            var qbs = ps.AverageProjectedModelRB();
            var model = ms.PopulateRbRegressorMatrix(qbs);
            var results = ps.PerformPrediction(model, ps.PerformPredictedRegression("RB"));
            for (int i = 0; i < results.Count; i++)
            {
                var qb = qbs.ElementAt(i);
                if (fs.IsPlayerActive(qb.PlayerId)){
                    var id = qb.PlayerId;
                    var name = fs.GetPlayerName(id);
                    System.Console.WriteLine("Name: " + name + " Projection " + results[i]);
                }
            }
           */
            /*
            FantasyService service = new();
            service.RefreshFantasyByPosition("RB");
            */
            return 1;
        }
    }
}
