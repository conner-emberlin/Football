using Football.Enums;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Projections.Interfaces
{
    public interface IProjectionService<T>
    {
        public bool GetProjectionsFromSQL(Position position, int season, out IEnumerable<T> projections);
        public bool GetProjectionsFromCache(Position position, out IEnumerable<T> projections);
        public Task<IEnumerable<T>?> GetPlayerProjections(int playerId);
        public Task<IEnumerable<T>> GetProjections(Position position);
        public Task<IEnumerable<T>> CalculateProjections<T1>(List<T1> model, Position position);
        public Task<int> PostProjections(List<T> projections);
    }
}
