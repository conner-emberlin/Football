using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Football.Interfaces;
using Football.Models;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearRegression;

namespace Football.Services
{
    public  class PerformRegressionService : IPerformRegressionService
    {
        public readonly IRegressionModelService _regressionModelService;
        public readonly IFantasyService _fantasyService;
        public readonly IMatrixService _matrixService;
        public PerformRegressionService(IRegressionModelService regressionModelService, IFantasyService fantasyService, IMatrixService matrixService) 
        { 
            _regressionModelService = regressionModelService;
            _fantasyService = fantasyService;
            _matrixService = matrixService;
        }
        public Vector<double> CholeskyDecomposition(Matrix<double> regressors, Vector<double> dependents)
        {
            var vec = MultipleRegression.NormalEquations(regressors, dependents);
            return vec;
        }

        public Vector<double> PerformRegression(int season, string position)
        {
            var fantasyPoints = _regressionModelService.PopulateFantasyResults(season, position);
            switch(position.ToUpper())
            {
                case "QB":
                    List<RegressionModelQB> qbs = new();
                    foreach (var fp in fantasyPoints)
                    {
                        qbs.Add(_regressionModelService.PopulateRegressionModelQB(fp.PlayerId, season));
                    }
                    return CholeskyDecomposition(_matrixService.PopulateQbRegressorMatrix(qbs), _matrixService.PopulateDependentVector(fantasyPoints));
                case "RB":
                    List<RegressionModelRB> rbs = new();
                    foreach(var fp in fantasyPoints)
                    {
                        rbs.Add(_regressionModelService.PopulateRegressionModelRb(fp.PlayerId, season));
                    }
                    return CholeskyDecomposition(_matrixService.PopulateRbRegressorMatrix(rbs), _matrixService.PopulateDependentVector(fantasyPoints));
                case "WR/TE":
                    List<RegressionModelPassCatchers> passCatchers = new();
                    foreach (var fp in fantasyPoints)
                    {
                        passCatchers.Add(_regressionModelService.PopulateRegressionModelPassCatchers(fp.PlayerId, season));
                    }
                    return CholeskyDecomposition(_matrixService.PopulatePassCatchersRegressorMatrix(passCatchers), _matrixService.PopulateDependentVector(fantasyPoints));
                default: throw new NotImplementedException();
            }
        }

        public double CalculateMSE(Vector<double> actual, Vector<double> coefficients, Matrix<double> model)
        {
            return MathNet.Numerics.Distance.MSE(actual, model * coefficients);
        }

        public Vector<double> CalculatError(Vector<double> actual, Vector<double> coefficients, Matrix<double> model)
        {
            return actual - model * coefficients;
        }        
    }
}
