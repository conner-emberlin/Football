using Football.Interfaces;
using Football.Models;
using System.Runtime;

namespace Football.Services
{
    public class AdjustmentCalculator : IAdjustmentCalculator
    {
        private readonly IAdjustmentRepository _adjustmentRepository;
        private readonly IPlayerService _playerService;

        private readonly int _currentSeason = 2023;
        public AdjustmentCalculator(IAdjustmentRepository adjustmentRepository, IPlayerService playerService)
        {
            _adjustmentRepository = adjustmentRepository;
            _playerService = playerService;
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
        
        public async Task<IEnumerable<ProjectionModel>> QBChangeAdjustment(IEnumerable<ProjectionModel> wrProjections, IEnumerable<ProjectionModel> qbProjections)
        {
            foreach(var proj in wrProjections)
            {
                var team = await _playerService.GetPlayerTeam(proj.PlayerId);
                var qbRecord = await _adjustmentRepository.GetTeamChange(_currentSeason, team);
                if(qbRecord != null)
                {
                    ProjectionModel oldQBProj = new();
                    foreach (var qbProj in qbProjections)
                    {
                        if(await _playerService.GetPlayerTeam(proj.PlayerId) == team)
                        {
                            oldQBProj = proj;
                        }
                    }
                    oldQBProj ??= qbProjections.ElementAt(qbProjections.Count() - 1);
                    var newQBProj = qbProjections.Where(p => p.PlayerId == qbRecord.PlayerId).FirstOrDefault();
                    if (newQBProj != null)
                    {
                        var ratio = newQBProj.ProjectedPoints / oldQBProj.ProjectedPoints;
                        proj.ProjectedPoints = (proj.ProjectedPoints * (ratio + 1)) / 2;
                    }
                }
            }
            return wrProjections;
        }
        
    }
}
