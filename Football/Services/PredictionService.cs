using Football.Models;
using MathNet.Numerics.LinearAlgebra;
using System.Data;
using Football.Interfaces;
using Serilog;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

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
        private readonly IConfiguration _configuration;
        public PredictionService(IPerformRegressionService performRegressionService, IRegressionModelService regressionModelService, 
            IFantasyService fantasyService, IPlayerService playerService, 
            IMatrixService matrixService, IWeightedAverageCalculator weightedAverageCalculator, 
            IAdjustmentCalculator adjustmentCalculator, ILogger logger, IMemoryCache cache,
            IConfiguration configuration)
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
            _configuration = configuration;
        }

        public async Task<IEnumerable<ProjectionModel>> GetFinalProjections(string position)
        {
            if(RetrieveFromCache(position).Any())
            {
                return RetrieveFromCache(position);
            }
            else if(position == "QB")
            {
                var projections = (await CalculateProjections(await AverageProjectedModelQB(), position)).ToList();
                var adjustedProjections = await _adjustmentCalculator.QBAdjustments(projections);
                var formattedProjections = adjustedProjections.OrderByDescending(p => p.ProjectedPoints).Take(QBProjections);
                _cache.Set("QBProjections", formattedProjections);
                return formattedProjections;
            }
            else if (position == "RB")
            {
                var rookieProjections = await PerformPredictedRookieRegression(position); 
                var projections = (await CalculateProjections(await AverageProjectedModelRB(), position)).ToList();
                foreach(var proj in rookieProjections)
                {
                    projections.Add(proj);
                }
                var adjustedProjections = await _adjustmentCalculator.RBAdjustments(projections);
                var formattedProjections = adjustedProjections.OrderByDescending(p => p.ProjectedPoints).Take(RBProjections);
                _cache.Set("RBProjections", formattedProjections);
                return formattedProjections;
            }
            else if (position == "WR")
            {
                var tightEnds = await _playerService.GetTightEnds();
                var qbProjections = await GetFinalProjections("QB");
                var projections = (await CalculateProjections(await AverageProjectedModelPassCatchers(), "WR/TE")).ToList();
                var adjustedProjections = await _adjustmentCalculator.PCAdjustments(projections, qbProjections);
                var formattedProjections = adjustedProjections.Where(p => !tightEnds.Contains(p.PlayerId)).OrderByDescending(p => p.ProjectedPoints).Take(WRProjections);
                foreach(var projection in formattedProjections)
                {
                    projection.Position = "WR";
                }
                _cache.Set("WRProjections", formattedProjections);
                return formattedProjections;
            }
            else if (position == "TE")
            {
                var tightEnds = await _playerService.GetTightEnds();
                var qbProjections = await GetFinalProjections("QB");
                var projections = (await CalculateProjections(await AverageProjectedModelPassCatchers(), "WR/TE")).ToList();
                var adjustedProjections = await _adjustmentCalculator.PCAdjustments(projections, qbProjections);
                var formattedProjections = adjustedProjections.Where(p => tightEnds.Contains(p.PlayerId)).OrderByDescending(p => p.ProjectedPoints).Take(TEProjections);
                foreach(var projection in formattedProjections)
                {
                    projection.Position = "TE";
                }
                _cache.Set("TEProjections", formattedProjections);
                return formattedProjections;
            }
            else
            {                
                return Enumerable.Empty<ProjectionModel>();
            }
        }
        public async Task<IEnumerable<ProjectionModel>> CalculateProjections<T>(List<T> model, string position)
        {
            List<ProjectionModel> projections = new();
            var regressorMatrix = _matrixService.PopulateRegressorMatrix(model);
            var results = PerformPrediction(regressorMatrix, await PerformPredictedRegression(model, regressorMatrix, position));
            for (int i = 0; i < results.Count; i++)
            {
                var playerId = (int)typeof(T).GetProperties()[0].GetValue(model[i]);
                if(await _playerService.IsPlayerActive(playerId))
                {
                    projections.Add(new ProjectionModel
                    {
                        PlayerId = playerId,
                        Name = await _playerService.GetPlayerName(playerId),
                        Team = await _playerService.GetPlayerTeam(playerId),
                        Position = await _playerService.GetPlayerPosition(playerId),
                        ProjectedPoints = results[i]
                    });
                }               
            }
            return projections;
        }
        public async Task<Vector<double>> PerformPredictedRegression<T>(List<T> regressionModel, Matrix<double> regressorMatrix, string position)
        {
            var dependentVector = _matrixService.PopulateDependentVector(await AverageProjectedFantasyByPosition(position));
            return _performRegressionService.CholeskyDecomposition(regressorMatrix, dependentVector);
        }
        public Vector<double> PerformPrediction(Matrix<double> model, Vector<double> coeff)
        {
            return model * coeff;
        }
        public async Task<List<FlexProjectionModel>> FlexRankings()
        {
            List<FlexProjectionModel> flexRankings = new();
            var qbProjections = (await GetFinalProjections("QB")).ToList();
            var rbProjections = (await GetFinalProjections("RB")).ToList();
            var wrProjections = (await GetFinalProjections("WR")).ToList();
            var teProjections = (await GetFinalProjections("TE")).ToList();
            try
            {
                var rankings = qbProjections.Concat(rbProjections).Concat(wrProjections).Concat(teProjections).ToList();
                foreach(var rank in rankings)
                {
                    flexRankings.Add(new FlexProjectionModel
                    {
                        PlayerId = rank.PlayerId, 
                        Name = rank.Name, 
                        Team = rank.Team, 
                        Position = rank.Position, 
                        ProjectedPoints = rank.ProjectedPoints,
                        VORP = rank.ProjectedPoints - await GetReplacementPoints(rank.Position)
                    });
                }
                return flexRankings.OrderByDescending(f => f.VORP).ToList();
            }
            catch(Exception ex)
            {
                _logger.Error(ex.ToString() + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }
        public async Task<int> InsertFantasyProjections(string position)
        {
            int rank = 1;
            int count = 0;
            var projections = await GetFinalProjections(position);
            foreach (var proj in projections)
            {
                count += await _fantasyService.InsertFantasyProjections(rank, proj);
                rank++;
            }
            return count;
        }

        private async Task<List<ProjectionModel>> PerformPredictedRookieRegression(string position)
        {
            List<ProjectionModel> rookieProjections = new();
            var historicalRookies = await _playerService.GetHistoricalRookies(CurrentSeason, position);
            var coeff = await _performRegressionService.PerformRegression(historicalRookies);
            var currentRookies = await _playerService.GetCurrentRookies(CurrentSeason, position);
            var model = _matrixService.PopulateRegressorMatrix(currentRookies);
            var predictions = PerformPrediction(model, coeff).ToList();

            for (int i = 0; i < predictions.Count; i++)
            {
                var rookie = currentRookies.ElementAt(i);
                rookieProjections.Add(new ProjectionModel
                {
                    PlayerId = rookie.PlayerId,
                    Name = await _playerService.GetPlayerName(rookie.PlayerId),
                    Team = rookie.TeamDrafted,
                    Position = rookie.Position,
                    ProjectedPoints = predictions[i]
                });
            }
            return rookieProjections;
        }
        private async Task<List<FantasyPoints>> AverageProjectedFantasyByPosition(string position)
        {
            var players = await _playerService.GetPlayersByPosition(position);
            List<FantasyPoints> projectedAverage = new();
            foreach (var p in players)
            {
                var player = await _playerService.GetPlayer(p);
                if (player.FantasyPoints.Count > 0)
                {
                    projectedAverage.Add(_weightedAverageCalculator.WeightedAverage(player));
                }
            }
            return projectedAverage;
        }

        private async Task<List<RegressionModelQB>> AverageProjectedModelQB()
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

        private async Task<List<RegressionModelRB>> AverageProjectedModelRB()
        {
            var players = await _playerService.GetPlayersByPosition("RB");
            List<RegressionModelRB> regressionModel = new();
            foreach (var p in players)
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

        private async Task<List<RegressionModelPassCatchers>> AverageProjectedModelPassCatchers()
        {
            var players = await _playerService.GetPlayersByPosition("WR/TE");
            List<RegressionModelPassCatchers> regressionModel = new();
            foreach (var p in players)
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
        private IEnumerable<ProjectionModel> RetrieveFromCache(string position)
        {
            var key = position + "Projections";
            if(_cache.TryGetValue(key, out IEnumerable<ProjectionModel> cachedProjections))
            {
                return cachedProjections;
            }
            else
            {
                return Enumerable.Empty<ProjectionModel>();
            }
        }

        private async Task<double> GetReplacementPoints(string position)
        {
            return position switch
            {
                "QB" => (await GetFinalProjections(position)).ElementAt(QBStarters - 1).ProjectedPoints,
                "RB" => (await GetFinalProjections(position)).ElementAt(RBStarters - 1).ProjectedPoints,
                "WR" => (await GetFinalProjections(position)).ElementAt(WRStarters - 1).ProjectedPoints,
                "TE" => (await GetFinalProjections(position)).ElementAt(TEStarters - 1).ProjectedPoints,
                _ => 0,
            };
        }

        public int QBProjections => int.Parse(_configuration["Projections:QBProjections"]);
        public int RBProjections => int.Parse(_configuration["Projections:RBProjections"]);
        public int WRProjections => int.Parse(_configuration["Projections:WRProjections"]);
        public int TEProjections => int.Parse(_configuration["Projections:TEProjections"]);
        public int QBStarters => int.Parse(_configuration["Starters:QBStarters"]);
        public int RBStarters => int.Parse(_configuration["Starters:RBStarters"]);
        public int WRStarters => int.Parse(_configuration["Starters:WRStarters"]);
        public int TEStarters => int.Parse(_configuration["Starters:TEStarters"]);
        public int CurrentSeason => int.Parse(_configuration["CurrentSeason"]);

    }
}
