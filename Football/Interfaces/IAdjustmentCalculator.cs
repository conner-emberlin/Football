using Football.Models;

namespace Football.Interfaces
{
    public interface IAdjustmentCalculator
    {
        public Task<IEnumerable<ProjectionModel>> QBAdjustments(IEnumerable<ProjectionModel> qbProjections);
        public Task<IEnumerable<ProjectionModel>> PCAdjustments(IEnumerable<ProjectionModel> pcProjections, IEnumerable<ProjectionModel> qbProjections);
        public Task<IEnumerable<ProjectionModel>> RBAdjustments(IEnumerable<ProjectionModel> rbProjections);

    }
}
