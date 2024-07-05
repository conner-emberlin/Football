using Football.Enums;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Projections.Interfaces
{
    public interface IProjectionService<T>
    {
        public Task<bool> DeleteProjection(T projection);
        public bool GetProjectionsFromSQL(Position position, int season, out IEnumerable<T> projections);
        public bool GetProjectionsFromCache(Position position, out IEnumerable<T> projections);
        public Task<IEnumerable<T>?> GetPlayerProjections(int playerId);
        public Task<IEnumerable<T>> GetProjections(Position position);
        public Task<int> PostProjections(List<T> projections);
        public Task<Vector<double>> GetCoefficients(Position position);
    }
}
