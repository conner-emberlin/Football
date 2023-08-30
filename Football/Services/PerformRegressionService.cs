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
        public PerformRegressionService(IRegressionModelService regressionModelService,
            IFantasyService fantasyService,IPlayerService playerService, 
            IMatrixService matrixService, ILogger logger) 
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
                    return CholeskyDecomposition(_matrixService.PopulateRegressorMatrix(qbs), _matrixService.PopulateDependentVector(fantasyPoints));
                case "RB":
                    List<RegressionModelRB> rbs = new();
                    foreach(var fp in fantasyPoints)
                    {
                        var rb = await _playerService.GetPlayer(fp.PlayerId);
                        rbs.Add(_regressionModelService.RegressionModelRB(rb, season));
                    }
                    return CholeskyDecomposition(_matrixService.PopulateRegressorMatrix(rbs), _matrixService.PopulateDependentVector(fantasyPoints));
                case "WR/TE":
                    List<RegressionModelPassCatchers> passCatchers = new();
                    foreach (var fp in fantasyPoints)
                    {
                        var pc = await _playerService.GetPlayer(fp.PlayerId);
                        passCatchers.Add(_regressionModelService.RegressionModelPC(pc, season));
                    }
                    return CholeskyDecomposition(_matrixService.PopulateRegressorMatrix(passCatchers), _matrixService.PopulateDependentVector(fantasyPoints));
                default: throw new NotImplementedException();
            }
        }
        public async Task<Vector<double>> PerformRegression(List<Rookie> rookies)
        {
            var vec = CholeskyDecomposition(_matrixService.PopulateRegressorMatrix(rookies), _matrixService.PopulateDependentVector(await _fantasyService.GetRookieFantasyResults(rookies)));
            return vec;

        }
            
        public double CalculateMSE(Vector<double> actual, Vector<double> coefficients, Matrix<double> model)
        {
            return MathNet.Numerics.Distance.MSE(actual, model * coefficients);
        }

    }
}
