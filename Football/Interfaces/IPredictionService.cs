using Football.Models;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Interfaces
{
    public interface IPredictionService {
        public Task<List<FantasyPoints>> AverageProjectedFantasyByPosition(string position);
        public Task<List<RegressionModelQB>> AverageProjectedModelQB();
        public Task<List<RegressionModelRB>> AverageProjectedModelRB();
        public Task<List<RegressionModelPassCatchers>> AverageProjectedModelPassCatchers();
        public Vector<double> PerformPrediction(Matrix<double> model, Vector<double> coeff);
        public Task<Vector<double>> PerformPredictedRegression(string position);
        public Task<List<ProjectionModel>> PerformPredictedRookieRegression(string position);
        public Task<IEnumerable<ProjectionModel>> GetProjections(string position);
        public Task<List<FlexProjectionModel>> FlexRankings();
        public Task<int> InsertFantasyProjections(string position);
    }
}
