using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Football.Models;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Interfaces
{
    public interface IPerformRegressionService
    {
        public Vector<double> CholeskyDecomposition(Matrix<double> regressors, Vector<double> dependents);
        public Vector<double> PerformRegression(int season, string position);
        public double CalculateMSE(Vector<double> actual, Vector<double> coefficients, Matrix<double> model);
        public Vector<double> CalculatError(Vector<double> actual, Vector<double> coefficients, Matrix<double> model);
    }
}
