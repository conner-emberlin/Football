using Football.Interfaces;
using Football.Models;
using System.Runtime;

namespace Football.Services
{
    public class AdjustmentCalculator : IAdjustmentCalculator
    {
        private readonly IAdjustmentRepository _adjustmentRepository;

        private readonly int _currentSeason = 2023;
        public AdjustmentCalculator(IAdjustmentRepository adjustmentRepository)
        {
            _adjustmentRepository = adjustmentRepository;
        }
        public async Task<IEnumerable<ProjectionModel>> SuspensionAdjustment(IEnumerable<ProjectionModel> projection)
        {
            foreach(var proj in projection)
            {
                var gamesSuspended = await _adjustmentRepository.GetGamesSuspended(proj.PlayerId, _currentSeason);
                if(gamesSuspended > 0)
                {
                    proj.ProjectedPoints -= gamesSuspended * (proj.ProjectedPoints / 17);
                }
            }
            return projection;
        }
    }
}
