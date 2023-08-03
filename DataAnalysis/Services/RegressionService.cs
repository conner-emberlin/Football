using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearRegression;
using MathNet.Numerics.LinearAlgebra;

namespace DataAnalysis.Services
{
    public class RegressionService
    {
        public Vector<double> CholeskyDecomposition(Matrix<double> regressors, Vector<double> dependents)
        {           
            var vec = MultipleRegression.NormalEquations(regressors, dependents);
            return vec;
        }
    }
}
