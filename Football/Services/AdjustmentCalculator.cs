﻿using Football.Interfaces;
using Football.Models;
using Serilog;
using Microsoft.Extensions.Options;

namespace Football.Services
{
    public class AdjustmentCalculator : IAdjustmentCalculator
    {
        private readonly IAdjustmentRepository _adjustmentRepository;
        private readonly IPlayerService _playerService;
        private readonly ILogger _logger;
        private readonly Season _season;
        private readonly Tunings _tunings;
        private readonly ReplacementLevels _replace;


        public AdjustmentCalculator(IAdjustmentRepository adjustmentRepository, IPlayerService playerService, ILogger logger, 
                IOptionsMonitor<Season> season, IOptionsMonitor<Tunings> tunings, IOptionsMonitor<ReplacementLevels> replace)
        {
            _adjustmentRepository = adjustmentRepository;
            _playerService = playerService;
            _logger = logger;
            _season = season.CurrentValue;
            _tunings = tunings.CurrentValue;
            _replace = replace.CurrentValue;
        }

        public async Task<IEnumerable<ProjectionModel>> QBAdjustments(IEnumerable<ProjectionModel> qbProjection)
        {
            qbProjection = await SuspensionAdjustment(qbProjection);
            qbProjection = await InjuryAdjustment(qbProjection);
            return qbProjection;
        }
        public async Task<IEnumerable<ProjectionModel>> PCAdjustments(IEnumerable<ProjectionModel> pcProjection, IEnumerable<ProjectionModel> qbProjection)
        {           
            pcProjection = await QBChangeAdjustment(pcProjection, qbProjection);
            pcProjection = await WRTeamChangeAdjustment(pcProjection, qbProjection);
            pcProjection = await SuspensionAdjustment(pcProjection);
            pcProjection = await InjuryAdjustment(pcProjection);
            return pcProjection;
        }

        public async Task<IEnumerable<ProjectionModel>> RBAdjustments(IEnumerable<ProjectionModel> rbProjection)
        {           
            rbProjection = await RBTimeshareAdjustment(rbProjection);
            rbProjection = await SuspensionAdjustment(rbProjection);
            rbProjection = await InjuryAdjustment(rbProjection);
            return rbProjection;
        }

        private async Task<IEnumerable<ProjectionModel>> SuspensionAdjustment(IEnumerable<ProjectionModel> projection)
        {
            foreach(var proj in projection)
            {
                var gamesSuspended = await _adjustmentRepository.GetGamesSuspended(proj.PlayerId, _season.CurrentSeason);
                if(gamesSuspended > 0)
                {
                    _logger.Information("Supsension found for player " + proj.PlayerId);
                    proj.ProjectedPoints -= gamesSuspended * (proj.ProjectedPoints / 17);
                }
            }
            return projection;
        }

        private async Task<IEnumerable<ProjectionModel>> InjuryAdjustment(IEnumerable<ProjectionModel> projection)
        {
            foreach(var proj in projection)
            {
                var injuryGames = await _adjustmentRepository.GetInjuryConcerns(proj.PlayerId, _season.CurrentSeason);
                if(injuryGames > 0)
                {
                    _logger.Information("Injury found for {playerId} during the {season} Season", proj.PlayerId, _season.CurrentSeason);
                    proj.ProjectedPoints -= injuryGames * (proj.ProjectedPoints / 17);
                }
            }
            return projection;
        }
        
        private async Task<IEnumerable<ProjectionModel>> QBChangeAdjustment(IEnumerable<ProjectionModel> wrProjections, IEnumerable<ProjectionModel> qbProjections)
        {
            foreach(var proj in wrProjections)
            {
                var team = await _playerService.GetPlayerTeam(proj.PlayerId);
                var qbRecords = await _adjustmentRepository.GetTeamChange(_season.CurrentSeason, team);
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
                        oldQBProj ??= qbProjections.ElementAt(_replace.ReplacementLevelQB);
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
                var change = await _adjustmentRepository.GetTeamChange(_season.CurrentSeason, wrProjection.PlayerId);
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
                    previousQBProjection ??= qbProjections.ElementAt(_replace.ReplacementLevelQB);
                    currentQBProjection ??= qbProjections.ElementAt(_replace.ReplacementLevelQB);

                    var ratio = currentQBProjection.ProjectedPoints / previousQBProjection.ProjectedPoints;
                    wrProjection.ProjectedPoints = (wrProjection.ProjectedPoints * (ratio + 1)) / 2;
                }
            }
            return wrProjections;
        }
        
        private async Task<IEnumerable<ProjectionModel>> RBTimeshareAdjustment(IEnumerable<ProjectionModel> rbProjection)
        {
            var replacementLevel = rbProjection.ElementAt(_replace.ReplacementLevelRB);
            foreach (var rbProj in rbProjection)
            {
                var team = await _playerService.GetPlayerTeam(rbProj.PlayerId);
                var changes = await _adjustmentRepository.GetTeamChange(_season.CurrentSeason, team);
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
                                r.ProjectedPoints = (0.5) * (Math.Max(r.ProjectedPoints, _tunings.RBFloor)) * (improvementRatio + _tunings.LeadRBFactor);
                            }
                        }                      
                    }
                }
            }
            return rbProjection;
        }
    }
}
