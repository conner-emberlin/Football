using Football.Models;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Interfaces
{
    public interface IPredictionService {

        public Vector<double> PerformPrediction(Matrix<double> model, Vector<double> coeff);
        public Task<Vector<double>> PerformPredictedRegression<T>(List<T> regressionModel, Matrix<double> regressorMatrix, string position);
        public Task<IEnumerable<ProjectionModel>> CalculateProjections<T>(List<T> regressionModel, string position);
        public Task<IEnumerable<ProjectionModel>> GetFinalProjections(string position);
        public Task<List<FlexProjectionModel>> FlexRankings();
        public Task<int> InsertFantasyProjections(string position);
    }
}
