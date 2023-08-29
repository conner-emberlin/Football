using Football.Interfaces;
using Football.Models;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearRegression;
using Serilog;

namespace Football.Services
{
    public  class PerformRegressionService : IPerformRegressionService
    {
        private readonly IRegressionModelService _regressionModelService;
        private readonly IFantasyService _fantasyService;
        private readonly IMatrixService _matrixService;
        private readonly IPlayerService _playerService;
        private readonly ILogger _logger;
        public PerformRegressionService(IRegressionModelService regressionModelService, IFantasyService fantasyService,IPlayerService playerService, IMatrixService matrixService, ILogger logger) 
        { 
            _regressionModelService = regressionModelService;
            _fantasyService = fantasyService;
            _matrixService = matrixService;
            _playerService = playerService;
            _logger = logger;
        }
        public Vector<double> CholeskyDecomposition(Matrix<double> regressors, Vector<double> dependents)
        {
            var vec = MultipleRegression.NormalEquations(regressors, dependents);
            return vec;
        }

        public async Task<Vector<double>> PerformRegression(int season, string position)
        {
            var fantasyPoints = await _regressionModelService.PopulateFantasyResults(season, position);
            switch(position.ToUpper())
            {
                case "QB":
                    List<RegressionModelQB> qbs = new();
                    foreach (var fp in fantasyPoints)
                    {
                        var qb = await _playerService.GetPlayer(fp.PlayerId);
                        qbs.Add(_regressionModelService.RegressionModelQB(qb, season));
                    }
                    return CholeskyDecomposition(_matrixService.PopulateQbRegressorMatrix(qbs), _matrixService.PopulateDependentVector(fantasyPoints));
                case "RB":
                    List<RegressionModelRB> rbs = new();
                    foreach(var fp in fantasyPoints)
                    {
                        var rb = await _playerService.GetPlayer(fp.PlayerId);
                        rbs.Add(_regressionModelService.RegressionModelRB(rb, season));
                    }
                    return CholeskyDecomposition(_matrixService.PopulateRbRegressorMatrix(rbs), _matrixService.PopulateDependentVector(fantasyPoints));
                case "WR/TE":
                    List<RegressionModelPassCatchers> passCatchers = new();
                    foreach (var fp in fantasyPoints)
                    {
                        var pc = await _playerService.GetPlayer(fp.PlayerId);
                        passCatchers.Add(_regressionModelService.RegressionModelPC(pc, season));
                    }
                    return CholeskyDecomposition(_matrixService.PopulatePassCatchersRegressorMatrix(passCatchers), _matrixService.PopulateDependentVector(fantasyPoints));
                default: throw new NotImplementedException();
            }
        }
        public async Task<Vector<double>> PerformRegression(List<Rookie> rookies)
        {
            var vec = CholeskyDecomposition(_matrixService.PopulateRookieRegressorMatrix(rookies), _matrixService.PopulateDependentVector(await _fantasyService.GetRookieFantasyResults(rookies)));
            return vec;

        }
            
        public double CalculateMSE(Vector<double> actual, Vector<double> coefficients, Matrix<double> model)
        {
            return MathNet.Numerics.Distance.MSE(actual, model * coefficients);
        }

        public Vector<double> CalculateError(Vector<double> actual, Vector<double> coefficients, Matrix<double> model)
        {
            return actual - model * coefficients;
        }

        public async Task<List<double>> ModelErrorPerSeason(int playerId, string position)
        {
            List<double> errors = new();
            List<Vector<double>> regressions = new();
            List<double> actualPoints = new();
            var seasons = await _playerService.GetActiveSeasons(playerId);
            switch (position)
            {
                case "QB":
                    List<RegressionModelQB> models = new();
                    foreach (var season in seasons)
                    {
                        models.Add(_regressionModelService.RegressionModelQB(await _playerService.GetPlayer(playerId), season));
                        regressions.Add(await PerformRegression(season, position));
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
                        modelsR.Add(_regressionModelService.RegressionModelRB(await _playerService.GetPlayer(playerId), season));
                        regressions.Add(await PerformRegression(season, position));
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
                        modelsP.Add(_regressionModelService.RegressionModelPC(await _playerService.GetPlayer(playerId), season));
                        regressions.Add(await PerformRegression(season, position));
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
                    _logger.Error("Invalid position. Check data");
                    break;
            }
            return errors;
        }
    }
}
