using Football.Enums;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Projections.Interfaces
{
    public interface IProjectionService<T>
    {
        public bool GetProjectionsFromSQL(PositionEnum position, int season, out IEnumerable<T> projections);
        public bool GetProjectionsFromCache(PositionEnum position, out IEnumerable<T> projections);
        public Task<IEnumerable<T>?> GetPlayerProjections(int playerId);
        public Task<IEnumerable<T>> GetProjections(PositionEnum position);
        public Task<IEnumerable<T>> CalculateProjections<T1>(List<T1> model, PositionEnum position);
        public Task<Vector<double>> PerformRegression(Matrix<double> regressorMatrix, PositionEnum position);
        public Vector<double> PerformProjection(Matrix<double> model, Vector<double> coeff);
        public Task<int> PostProjections(List<T> projections);
    }
}
