﻿using Football.Enums;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Projections.Interfaces
{
    public interface IProjectionService<T>
    {
        public Task<bool> DeleteProjection(T projection);
        public bool GetProjectionsFromSQL(Position position, int season, out IEnumerable<T> projections);
        public Task<IEnumerable<T>?> GetPlayerProjections(int playerId);
        public Task<IEnumerable<T>> GetProjections(Position position, List<string>? filter = null);
        public Task<int> PostProjections(List<T> projections, List<string> filters);
        public Task<Vector<double>> GetCoefficients(Position position);
        public IEnumerable<string> GetModelVariablesByPosition(Position position);
        public Task<bool> PostProjectionConfiguration(Position position, string filter);
        public Task<string?> GetCurrentProjectionConfigurationFilter(Position position);
    }

}
