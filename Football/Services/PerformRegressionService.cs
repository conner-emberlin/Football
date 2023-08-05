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
        public Vector<double> CholeskyDecomposition(Matrix<double> regressors, Vector<double> dependents)
        {
            var vec = MultipleRegression.NormalEquations(regressors, dependents);
            return vec;
        }

        public Vector<double> PerformRegression(int season, string position)
        {
            RegressionModelService regressionModelService = new();
            FantasyService fantasyService = new();
            MatrixService matrixService = new();

            var fantasyPoints = regressionModelService.PopulateFantasyResults(season, position);
            switch(position.ToUpper())
            {
                case "QB":
                    List<RegressionModelQB> qbs = new();
                    foreach (var fp in fantasyPoints)
                    {
                        qbs.Add(regressionModelService.PopulateRegressionModelQB(fp.PlayerId, season));
                    }
                    return CholeskyDecomposition(matrixService.PopulateQbRegressorMatrix(qbs), matrixService.PopulateDependentVector(fantasyPoints));
                case "RB":
                    List<RegressionModelRB> rbs = new();
                    foreach(var fp in fantasyPoints)
                    {
                        rbs.Add(regressionModelService.PopulateRegressionModelRb(fp.PlayerId, season));
                    }
                    return CholeskyDecomposition(matrixService.PopulateRbRegressorMatrix(rbs), matrixService.PopulateDependentVector(fantasyPoints));
                case "WR/TE":
                    List<RegressionModelPassCatchers> passCatchers = new();
                    foreach (var fp in fantasyPoints)
                    {
                        passCatchers.Add(regressionModelService.PopulateRegressionModelPassCatchers(fp.PlayerId, season));
                    }
                    return CholeskyDecomposition(matrixService.PopulatePassCatchersRegressorMatrix(passCatchers), matrixService.PopulateDependentVector(fantasyPoints));
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
