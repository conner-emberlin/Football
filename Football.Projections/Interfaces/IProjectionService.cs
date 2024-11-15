using Football.Enums;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Projections.Interfaces
{
    public interface IProjectionService<T>
    {
        Task<bool> DeleteProjection(T projection);
        bool GetProjectionsFromSQL(Position position, int season, out IEnumerable<T> projections);
        Task<IEnumerable<T>?> GetPlayerProjections(int playerId);
        Task<IEnumerable<T>> GetProjections(Position position, List<string>? filter = null);
        Task<int> PostProjections(List<T> projections, List<string> filters);
        Task<Vector<double>> GetCoefficients(Position position);
        IEnumerable<string> GetModelVariablesByPosition(Position position);
        Task<bool> PostProjectionConfiguration(Position position, string filter);
        Task<string?> GetCurrentProjectionConfigurationFilter(Position position);
    }

}
