using Football.Projections.Models;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Projections.Interfaces
{
    public interface IProjectionService
    {
        public Task<List<SeasonFlex>> SeasonFlexRankings();
        public Task<IEnumerable<SeasonProjection>> GetSeasonProjections(string position);
        public Task<IEnumerable<SeasonProjection>> CalculateSeasonProjections<T>(List<T> model, string position);
        public Task<Vector<double>> PerformRegression<T>(List<T> regressionModel, Matrix<double> regressorMatrix, string position);
        public Vector<double> PerformProjection(Matrix<double> model, Vector<double> coeff);
    }
}
