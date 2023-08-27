using Football.Interfaces;
using Football.Models;
using Serilog;
using System.Runtime;

namespace Football.Services
{
    public class AdjustmentCalculator : IAdjustmentCalculator
    {
        private readonly IAdjustmentRepository _adjustmentRepository;
        private readonly IPlayerService _playerService;
        private readonly ILogger _logger;

        private readonly int _currentSeason = 2023;
        private readonly int _rbNewTeam = 85;
        public AdjustmentCalculator(IAdjustmentRepository adjustmentRepository, IPlayerService playerService, ILogger logger)
        {
            _adjustmentRepository = adjustmentRepository;
            _playerService = playerService;
            _logger = logger;
        }

        public async Task<IEnumerable<ProjectionModel>> QBAdjustments(IEnumerable<ProjectionModel> qbProjection)
        {
            qbProjection = await SuspensionAdjustment(qbProjection);
            return qbProjection;
        }
        public async Task<IEnumerable<ProjectionModel>> PCAdjustments(IEnumerable<ProjectionModel> pcProjection, IEnumerable<ProjectionModel> qbProjection)
        {
            pcProjection = await SuspensionAdjustment(pcProjection);
            pcProjection = await QBChangeAdjustment(pcProjection, qbProjection);
            pcProjection = await WRTeamChangeAdjustment(pcProjection, qbProjection);
            return pcProjection;
        }

        public async Task<IEnumerable<ProjectionModel>> RBAdjustments(IEnumerable<ProjectionModel> rbProjection)
        {
            rbProjection = await SuspensionAdjustment(rbProjection);
            rbProjection = await RBTimeshareAdjustment(rbProjection);
            return rbProjection;
        }

        private async Task<IEnumerable<ProjectionModel>> SuspensionAdjustment(IEnumerable<ProjectionModel> projection)
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
        
        private async Task<IEnumerable<ProjectionModel>> QBChangeAdjustment(IEnumerable<ProjectionModel> wrProjections, IEnumerable<ProjectionModel> qbProjections)
        {
            foreach(var proj in wrProjections)
            {
                var team = await _playerService.GetPlayerTeam(proj.PlayerId);
                var qbRecords = await _adjustmentRepository.GetTeamChange(_currentSeason, team);
                foreach (var qbRecord in qbRecords)
                {
                    if (qbRecord != null && await _playerService.GetPlayerPosition(qbRecord.PlayerId) == "QB")
                    {
                        _logger.Information("New QB found for player " + proj.PlayerId);
                        ProjectionModel oldQBProj = new();
                        foreach (var qbProj in qbProjections)
                        {
                            if (await _playerService.GetPlayerTeam(proj.PlayerId) == team)
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
            }
            return wrProjections;
        }

        private async Task<IEnumerable<ProjectionModel>> WRTeamChangeAdjustment(IEnumerable<ProjectionModel> wrProjections, IEnumerable<ProjectionModel> qbProjections)
        {
            foreach (var wrProjection in wrProjections)
            {
                var change = await _adjustmentRepository.GetTeamChange(_currentSeason, wrProjection.PlayerId);
                if (change != null)
                {
                    _logger.Information("Team change found for playerid " + wrProjection.PlayerId);
                    ProjectionModel previousQBProjection = new();
                    ProjectionModel currentQBProjection = new();
                    foreach (var qbProj in qbProjections)
                    {
                        if (qbProj.Team == change.PreviousTeam)
                        {
                            previousQBProjection = qbProj;
                        }
                        if (qbProj.Team == change.NewTeam)
                        {
                            currentQBProjection = qbProj;
                        }
                    }
                    previousQBProjection ??= qbProjections.ElementAt(qbProjections.Count() - 1);
                    currentQBProjection ??= qbProjections.ElementAt(qbProjections.Count() - 1);

                    var ratio = currentQBProjection.ProjectedPoints / previousQBProjection.ProjectedPoints;
                    wrProjection.ProjectedPoints = (wrProjection.ProjectedPoints * (ratio + 1)) / 2;
                }
            }
            return wrProjections;
        }
        
        private async Task<IEnumerable<ProjectionModel>> RBTimeshareAdjustment(IEnumerable<ProjectionModel> rbProjection)
        {
            var replacementLevel = rbProjection.ElementAt(rbProjection.Count() - 1);
            foreach (var rbProj in rbProjection)
            {
                var team = await _playerService.GetPlayerTeam(rbProj.PlayerId);
                var changes = await _adjustmentRepository.GetTeamChange(_currentSeason, team);
                foreach (var change in changes)
                {
                    if (change != null && await _playerService.GetPlayerPosition(change.PlayerId) == "RB")
                    {
                        ProjectionModel newGuyInTown = new();
                        newGuyInTown = rbProjection.Where(r => r.PlayerId == change.PlayerId).ToList().FirstOrDefault();
                        newGuyInTown ??= replacementLevel;

                        var improvementRatio = (replacementLevel.ProjectedPoints / (newGuyInTown.ProjectedPoints - _rbNewTeam));
                        var newcomerRatio = (replacementLevel.ProjectedPoints / rbProj.ProjectedPoints);
                        rbProj.ProjectedPoints = (rbProj.ProjectedPoints * (improvementRatio + 1)) / 2;
                        rbProjection.Where(r => r.PlayerId == newGuyInTown.PlayerId).ToList().FirstOrDefault().ProjectedPoints = ((newcomerRatio+1) * rbProjection.Where(r => r.PlayerId == newGuyInTown.PlayerId).ToList().FirstOrDefault().ProjectedPoints)/2;

                    }
                }
            }
            return rbProjection;
        }
        
    }
}
