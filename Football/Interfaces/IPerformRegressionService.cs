using MathNet.Numerics.LinearAlgebra;
using Football.Models;
namespace Football.Interfaces
{
    public interface IPerformRegressionService
    {
        public Vector<double> CholeskyDecomposition(Matrix<double> regressors, Vector<double> dependents);
        public Task<Vector<double>> PerformRegression(int season, string position);
        public Task<Vector<double>> PerformRegression(List<Rookie> rookies);
        public double CalculateMSE(Vector<double> actual, Vector<double> coefficients, Matrix<double> model);
        public Vector<double> CalculateError(Vector<double> actual, Vector<double> coefficients, Matrix<double> model);
        public Task<List<double>> ModelErrorPerSeason(int playerId, string position);
    }
}
