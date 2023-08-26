using Football.Models;

namespace Football.Interfaces
{
    public interface IAdjustmentCalculator
    {
        public Task<IEnumerable<ProjectionModel>> SuspensionAdjustment(IEnumerable<ProjectionModel> projections);
        public Task<IEnumerable<ProjectionModel>> QBChangeAdjustment(IEnumerable<ProjectionModel> wrProjections, IEnumerable<ProjectionModel> qbProjections);
    }
}
