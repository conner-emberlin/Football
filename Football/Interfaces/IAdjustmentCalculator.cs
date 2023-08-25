using Football.Models;

namespace Football.Interfaces
{
    public interface IAdjustmentCalculator
    {
        public Task<IEnumerable<ProjectionModel>> SuspensionAdjustment(IEnumerable<ProjectionModel> projectins);
    }
}
