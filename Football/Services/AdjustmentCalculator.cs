using Football.Interfaces;
using Football.Models;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Football.Services
{
    public class AdjustmentCalculator : IAdjustmentCalculator
    {
        private readonly IAdjustmentRepository _adjustmentRepository;
        private readonly IPlayerService _playerService;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        public int CurrentSeason => Int32.Parse(_configuration["CurrentSeason"]);
        public int RBFloor => Int32.Parse(_configuration["RBFloor"]);
        public int ReplacementRB => Int32.Parse(_configuration["ReplacementLevels:ReplacementLevelRB"]);
        public int ReplacementQB => Int32.Parse(_configuration["ReplacementLevels:ReplacementLevelQB"]);
        public AdjustmentCalculator(IAdjustmentRepository adjustmentRepository, IPlayerService playerService, ILogger logger, IConfiguration configuration)
        {
            _adjustmentRepository = adjustmentRepository;
            _playerService = playerService;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IEnumerable<ProjectionModel>> QBAdjustments(IEnumerable<ProjectionModel> qbProjection)
        {
            qbProjection = await SuspensionAdjustment(qbProjection);
            return qbProjection;
        }
        public async Task<IEnumerable<ProjectionModel>> PCAdjustments(IEnumerable<ProjectionModel> pcProjection, IEnumerable<ProjectionModel> qbProjection)
        {           
            pcProjection = await QBChangeAdjustment(pcProjection, qbProjection);
            pcProjection = await WRTeamChangeAdjustment(pcProjection, qbProjection);
            pcProjection = await SuspensionAdjustment(pcProjection);
            return pcProjection;
        }

        public async Task<IEnumerable<ProjectionModel>> RBAdjustments(IEnumerable<ProjectionModel> rbProjection)
        {           
            rbProjection = await RBTimeshareAdjustment(rbProjection);
            rbProjection = await SuspensionAdjustment(rbProjection);
            return rbProjection;
        }

        private async Task<IEnumerable<ProjectionModel>> SuspensionAdjustment(IEnumerable<ProjectionModel> projection)
        {
            foreach(var proj in projection)
            {
                var gamesSuspended = await _adjustmentRepository.GetGamesSuspended(proj.PlayerId, CurrentSeason);
                if(gamesSuspended > 0)
                {
                    _logger.Information("Supsension found for player " + proj.PlayerId);
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
                var qbRecords = await _adjustmentRepository.GetTeamChange(CurrentSeason, team);
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
                        oldQBProj ??= qbProjections.ElementAt(ReplacementQB);
                        var newQBProj = qbProjections.Where(p => p.PlayerId == qbRecord.PlayerId).FirstOrDefault();
                        if (newQBProj != null)
                        {
                            var ratio = newQBProj.ProjectedPoints / oldQBProj.ProjectedPoints;
                            _logger.Information(proj.PlayerId + " has a new Quarterback: " + newQBProj.Name);
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
                var change = await _adjustmentRepository.GetTeamChange(CurrentSeason, wrProjection.PlayerId);
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
                    previousQBProjection ??= qbProjections.ElementAt(ReplacementQB);
                    currentQBProjection ??= qbProjections.ElementAt(ReplacementQB);

                    var ratio = currentQBProjection.ProjectedPoints / previousQBProjection.ProjectedPoints;
                    wrProjection.ProjectedPoints = (wrProjection.ProjectedPoints * (ratio + 1)) / 2;
                }
            }
            return wrProjections;
        }
        
        private async Task<IEnumerable<ProjectionModel>> RBTimeshareAdjustment(IEnumerable<ProjectionModel> rbProjection)
        {
            var replacementLevel = rbProjection.ElementAt(ReplacementRB);
            foreach (var rbProj in rbProjection)
            {
                var team = await _playerService.GetPlayerTeam(rbProj.PlayerId);
                var changes = await _adjustmentRepository.GetTeamChange(CurrentSeason, team);
                foreach (var change in changes)
                {
                    if (change != null && await _playerService.GetPlayerPosition(change.PlayerId) == "RB")
                    {
                        ProjectionModel? newGuyInTown = new();
                        newGuyInTown = rbProjection.Where(r => r.PlayerId == change.PlayerId).ToList().FirstOrDefault();
                        newGuyInTown ??= replacementLevel;

                        var improvementRatio = replacementLevel.ProjectedPoints / newGuyInTown.ProjectedPoints;
                        var newcomerRatio = (replacementLevel.ProjectedPoints / rbProj.ProjectedPoints);
                        rbProj.ProjectedPoints = (rbProj.ProjectedPoints * (improvementRatio + 1)) / 2;
                        var oldProj = rbProjection.Where(r => r.PlayerId == newGuyInTown.PlayerId).ToList().FirstOrDefault().ProjectedPoints;
                        rbProjection.Where(r => r.PlayerId == newGuyInTown.PlayerId).ToList().FirstOrDefault().ProjectedPoints = ((newcomerRatio+1) * oldProj)/2;
                        foreach(var r in rbProjection)
                        {
                            if(await _playerService.GetPlayerTeam(r.PlayerId) == change.PreviousTeam && r.PlayerId != change.PlayerId)
                            {
                                r.ProjectedPoints = ((double)1 / (double)2) * (Max(r.ProjectedPoints, RBFloor)) * (improvementRatio + 2);
                            }
                        }                      
                    }
                }
            }
            return rbProjection;
        }
        private double Max(double one, double two)
        {
            return one >= two ? one : two;
        }
    }
}
