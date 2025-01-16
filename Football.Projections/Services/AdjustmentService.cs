using Football.Enums;
using Football.Models;
using Football.Fantasy.Interfaces;
using Football.Players.Interfaces;
using Football.Projections.Models;
using Football.Projections.Interfaces;
using Microsoft.Extensions.Options;
using Football.Players.Models;
using Microsoft.Extensions.Caching.Memory;
using AutoMapper;

namespace Football.Projections.Services
{
    public class AdjustmentService(IPlayersService playerService, ITeamsService teamsService, IStatisticsService statisticsService, IFantasyAnalysisService fantasyAnalysisService,
        IFantasyDataService fantasyDataService, IProjectionRepository projectionRepository, IOptionsMonitor<Season> season, IMemoryCache cache, IMapper mapper) : IAdjustmentService
    {
        private readonly Season _season = season.CurrentValue;

        public async Task<IEnumerable<SeasonProjection>> AdjustmentEngine(IEnumerable<SeasonProjection> seasonProjections, Tunings tunings, int seasonGames, SeasonAdjustments adjustments)
        {
            if (!seasonProjections.Any()) return seasonProjections;
            _ = Enum.TryParse(seasonProjections.First().Position, out Position position);
            var allTeamChanges = await playerService.GetAllTeamChanges(_season.CurrentSeason);
            return position switch
            {
                Position.QB => await QBSeasonProjectionAdjustments(seasonProjections, allTeamChanges, tunings, seasonGames, adjustments),
                Position.RB => await RBSeasonProjectionAdjustments(seasonProjections, allTeamChanges, tunings, seasonGames, adjustments),
                Position.WR => await WRSeasonProjectionAdjustments(seasonProjections, allTeamChanges, tunings, seasonGames, adjustments),
                Position.TE => await TESeasonProjectionAdjustment(seasonProjections, allTeamChanges, tunings, seasonGames, adjustments),
                _ => seasonProjections
            };
        }
        public async Task<IEnumerable<AdjustmentDescription>> GetAdjustmentDescriptions() => await projectionRepository.GetAdjustmentDescriptions();

        private async Task<Dictionary<int, double>> InjuryAdjustment(IEnumerable<SeasonProjection> seasonProjections, int seasonGames, List<(int, string)> adjustmentTracker)
        {
            Dictionary<int, double> adjustments = [];
            var playerInjuries = (await playerService.GetPlayerInjuries(_season.CurrentSeason)).ToDictionary(i => i.PlayerId, i => i.Games);
            foreach(var s in seasonProjections)
            {
                if (playerInjuries.TryGetValue(s.PlayerId, out var games))
                {
                    var diff = - (s.ProjectedPoints / seasonGames) * games;
                    adjustments.Add(s.PlayerId, diff);
                    adjustmentTracker.Add((s.PlayerId, Adjustment.Injury.ToString()));
                }
                    
            }
            return adjustments;
        }
        private async Task<Dictionary<int, double>> SuspensionAdjustment(IEnumerable<SeasonProjection> seasonProjections, int seasonGames, List<(int, string)> adjustmentTracker)
        {
            Dictionary<int, double> adjustments = [];
            var playerSuspensions = (await playerService.GetPlayerSuspensions(_season.CurrentSeason)).ToDictionary(s => s.PlayerId, s => s.Length);
            foreach (var s in seasonProjections)
            {
                if (playerSuspensions.TryGetValue(s.PlayerId, out var length)) 
                {
                    var diff = -(s.ProjectedPoints / seasonGames) * length;
                    adjustments.Add(s.PlayerId, diff);
                    adjustmentTracker.Add((s.PlayerId, Adjustment.Suspension.ToString()));

                } 
            }
            return adjustments;
        }

        private async Task<Dictionary<int, double>> BackupQuarterbackAdjustment(IEnumerable<SeasonProjection> seasonProjections, List<(int, string)> adjustmentTracker)
        {
            Dictionary<int, double> adjustments = [];
            var backupQuarterbacks = await playerService.GetBackupQuarterbacks(_season.CurrentSeason);
            foreach (var s in seasonProjections)
            {
                if (backupQuarterbacks.Any(b => b.PlayerId == s.PlayerId))
                {
                    adjustments.Add(s.PlayerId, -s.ProjectedPoints);
                    adjustmentTracker.Add((s.PlayerId, Adjustment.BackupQuarterback.ToString()));
                }
            }
            return adjustments;
        }
        private async Task<Dictionary<int, double>> QuarterbackChangeAdjustment(IEnumerable<SeasonProjection> seasonProjections, Dictionary<int, QuarterbackChange> qbChanges, Tunings tunings, List<(int, string)> adjustmentTracker)
        {
            Dictionary<int, double> adjustments = [];
            foreach (var s in seasonProjections)
            {
                if (qbChanges.TryGetValue(s.PlayerId, out var changeRecord) && !await IsRushingQB(changeRecord.CurrentQBId, tunings) 
                    && !(changeRecord.CurrentQBIsRookie && s.Position == Position.RB.ToString()))
                {
                    var qbProjections = await playerService.GetSeasonProjections([changeRecord.PreviousQBId, changeRecord.CurrentQBId], _season.CurrentSeason);
                    var previousQbProjection = qbProjections.TryGetValue(changeRecord.PreviousQBId, out var projectPrev) ? projectPrev : tunings.AverageQBProjection;
                    var currentQbProjection = qbProjections.TryGetValue(changeRecord.CurrentQBId, out var currentPrev) ? currentPrev : tunings.AverageQBProjection;
                    var ratio =  QBProjectionRatio(s.Position, previousQbProjection, currentQbProjection, changeRecord.CurrentQBIsRookie, tunings);
                    adjustments.Add(s.PlayerId, s.ProjectedPoints * (ratio - 1));
                    adjustmentTracker.Add((s.PlayerId, Adjustment.QuarterbackChange.ToString()));
                }
            }
            return adjustments;
        }

        private async Task<Dictionary<int, double>> PreviousSeasonBackupQuarterbackAdjustment(IEnumerable<SeasonProjection> seasonProjections, Dictionary<int, QuarterbackChange> qbChanges, Tunings tunings, int seasonGames, List<(int, string)> adjustmentTracker)
        {
            Dictionary<int, double> adjustments = [];
            var previousSeason = _season.CurrentSeason - 1;
            var playerTeamsDictionary = (await teamsService.GetPlayerTeams(previousSeason, seasonProjections.Select(p => p.PlayerId))).ToDictionary(t => t.PlayerId);
            var currentStartedMissedGames = (await statisticsService.GetCurrentStartersThatMissedGamesLastSeason(_season.CurrentSeason, previousSeason, 15, tunings.AverageQBProjection)).ToDictionary(c => c.PreviousSeasonTeamId);

            foreach (var sp in seasonProjections)
            {
                if (playerTeamsDictionary.TryGetValue(sp.PlayerId, out var playerTeam) && currentStartedMissedGames.TryGetValue(playerTeam.TeamId, out var starter) && !qbChanges.ContainsKey(sp.PlayerId))
                {
                    var previousSeasonBackup = (await statisticsService.GetSeasonDataByTeamIdAndPosition<SeasonDataQB>(starter.PreviousSeasonTeamId, Position.QB, previousSeason)).Where(s => s.PlayerId != starter.PlayerId).OrderByDescending(s => s.Attempts).FirstOrDefault();
                    if (previousSeasonBackup != null)
                    {
                        var backupFantasy = (await fantasyDataService.GetSeasonFantasy(previousSeasonBackup.PlayerId)).First(f => f.Season == previousSeason);
                        var backupFantasyPoints = (backupFantasy.FantasyPoints / backupFantasy.Games) * (seasonGames);
                        if (backupFantasyPoints < starter.CurrentSeasonProjection)
                        {
                            var projectionRatio = (starter.CurrentSeasonProjection / backupFantasyPoints) / (seasonGames - starter.PreviousSeasonGames);
                            var ratio = Math.Min(1 + projectionRatio, tunings.BackupQBAdjustmentMax);
                            adjustments.Add(sp.PlayerId, sp.ProjectedPoints * (ratio - 1));
                            adjustmentTracker.Add((sp.PlayerId, Adjustment.PreviousSeasonBackupQuarterback.ToString()));
                        }
                    }
                }
               
            }
            return adjustments;

        }
        private async Task<Dictionary<int, double>> SharedReceivingDutiesAdjustment(IEnumerable<SeasonProjection> seasonProjections, IEnumerable<TeamChange> wrTeamChanges, Tunings tunings, List<(int, string)> adjustmentTracker)
        {
            Dictionary<int, double> adjustments = [];
            var seasonProjectionDictionary = seasonProjections.ToDictionary(s => s.PlayerId, s => s.ProjectedPoints);
            
            var playerTeams = (await teamsService.GetPlayerTeams(_season.CurrentSeason, seasonProjections.Select(p => p.PlayerId))).ToDictionary(p => p.PlayerId);
            foreach (var sp in seasonProjections)
            {
                if (playerTeams.TryGetValue(sp.PlayerId, out var team))
                {
                    var wrsThatMovedToProjectedPlayersTeam = wrTeamChanges.Where(w => w.CurrentTeamId == team.TeamId).Select(p => p.PlayerId);

                    if (wrsThatMovedToProjectedPlayersTeam.Contains(sp.PlayerId))
                    {
                        var otherReceivers = await teamsService.GetPlayersByTeamIdAndPosition(team.TeamId, Position.WR, _season.CurrentSeason, true);
                        if (otherReceivers.Any(r => r.PlayerId != sp.PlayerId && seasonProjectionDictionary.TryGetValue(r.PlayerId, out var proj) && proj > tunings.NewWRMinPoints))
                        {
                            adjustments.Add(sp.PlayerId, sp.ProjectedPoints * (tunings.NewWRAdjustmentFactor - 1));
                            adjustmentTracker.Add((sp.PlayerId, Adjustment.SharedReceivingDuties.ToString()));
                        }
                            
                        
                    }
                    else if (wrsThatMovedToProjectedPlayersTeam.Any(w => seasonProjectionDictionary.TryGetValue(w, out var proj) && proj > tunings.NewWRMinPoints))
                    {
                        adjustments.Add(sp.PlayerId, sp.ProjectedPoints * (tunings.ExistingWRAdjustmentFactor - 1));
                        adjustmentTracker.Add((sp.PlayerId, Adjustment.SharedReceivingDuties.ToString()));
                    }                        
                }                
            }
            return adjustments;
        }

        private async Task<Dictionary<int, double>> VeteranQBOnNewTeamAdjustment(IEnumerable<SeasonProjection> seasonProjections, IEnumerable<TeamChange> teamChanges, Tunings tunings, List<(int, string)> adjustmentTracker)
        {
            Dictionary<int, double> adjustments = [];
            var qbChangesDictionary = teamChanges.ToDictionary(t => t.PlayerId);
            foreach (var sp in seasonProjections)
            {
                if (qbChangesDictionary.TryGetValue(sp.PlayerId, out var teamChage))
                {
                    var yearsExperience = await statisticsService.GetYearsExperience(sp.PlayerId, Position.QB);
                    if (yearsExperience > tunings.VetQBNewTeamYears) 
                    {
                        adjustments.Add(sp.PlayerId, sp.ProjectedPoints * (tunings.VetQBNewTeamFactor - 1));
                        adjustmentTracker.Add((sp.PlayerId, Adjustment.VeteranQBonNewTeam.ToString()));
                    }
                }
            }
            return adjustments;
        }

        private async Task<Dictionary<int, double>> SharedBackfieldAdjustment(IEnumerable<SeasonProjection> seasonProjections, IEnumerable<TeamChange> rbTeamChanges, Tunings tunings, List<(int, string)> adjustmentTracker)
        {
            Dictionary<int, double> adjustments = [];
            var seasonProjectionDictionary = seasonProjections.ToDictionary(s => s.PlayerId, s => s.ProjectedPoints);
            var playerTeams = (await teamsService.GetPlayerTeams(_season.CurrentSeason, seasonProjections.Select(p => p.PlayerId))).ToDictionary(p => p.PlayerId);
            foreach (var sp in seasonProjections)
            {
                if (playerTeams.TryGetValue(sp.PlayerId, out var team))
                {
                    var rbsThatMovedToProjectedPlayersTeam = rbTeamChanges.Where(w => w.CurrentTeamId == team.TeamId).Select(p => p.PlayerId);
                    var otherRBsOnTeam = playerTeams.Where(p => p.Value.TeamId == team.TeamId && p.Key != sp.PlayerId);

                    if (rbsThatMovedToProjectedPlayersTeam.Contains(sp.PlayerId))
                    {
                        var otherRBs = await teamsService.GetPlayersByTeamIdAndPosition(team.TeamId, Position.RB, _season.CurrentSeason, true);
                        if (otherRBs.Any(r => r.PlayerId != sp.PlayerId && seasonProjectionDictionary.TryGetValue(r.PlayerId, out var proj) && proj > tunings.NewRBMinPoints))
                        {
                            adjustments.Add(sp.PlayerId, sp.ProjectedPoints * (tunings.NewRBAdjustmentFactor - 1));
                            adjustmentTracker.Add((sp.PlayerId, Adjustment.SharedBackfield.ToString()));
                        }
                            
                    }
                    else if (rbsThatMovedToProjectedPlayersTeam.Any(w => seasonProjectionDictionary.TryGetValue(w, out var proj) && proj > tunings.NewRBMinPoints))
                    {
                        adjustments.Add(sp.PlayerId, sp.ProjectedPoints * (tunings.ExistingRBAdjustmentFactor - 1));
                        adjustmentTracker.Add((sp.PlayerId, Adjustment.SharedBackfield.ToString()));
                    }
                    else
                    {
                        foreach (var otherRB in otherRBsOnTeam)
                        {
                            if (seasonProjectionDictionary.TryGetValue(otherRB.Key, out var otherProjection) && otherProjection > tunings.NewRBMinPoints && !adjustments.ContainsKey(sp.PlayerId))
                            {
                                adjustments.Add(sp.PlayerId, sp.ProjectedPoints * (tunings.ExistingRBAdjustmentFactor - 1));
                                adjustmentTracker.Add((sp.PlayerId, Adjustment.SharedBackfield.ToString()));
                            }
                                
                        }
                    }

                }

            }
            return adjustments;
        }

        private async Task<Dictionary<int, double>> DownwardTrendingAdjustment(IEnumerable<SeasonProjection> seasonProjections, List<(int, string)> adjustmentTracker)
        {
            Dictionary<int, double> adjustments = [];
            foreach (var sp in seasonProjections)
            {
                var seasonFantasy = (await fantasyDataService.GetSeasonFantasy(sp.PlayerId)).OrderByDescending(f => f.Season);
                if (seasonFantasy.Count() > 4)
                {
                    var downwardTrend = seasonFantasy.ElementAt(0).FantasyPoints < seasonFantasy.ElementAt(1).FantasyPoints 
                                     && seasonFantasy.ElementAt(1).FantasyPoints < seasonFantasy.ElementAt(2).FantasyPoints;
                    
                    if (downwardTrend)
                    {
                        var diff = (seasonFantasy.ElementAt(0).FantasyPoints - seasonFantasy.ElementAt(2).FantasyPoints);
                        adjustments.Add(sp.PlayerId, diff);
                        adjustmentTracker.Add((sp.PlayerId, Adjustment.DownwardTrending.ToString()));
                    }
                }
            }
            return adjustments;
        }

        private async Task<Dictionary<int, double>> EliteRookieWRTopTargetAdjustment(IEnumerable<SeasonProjection> seasonProjections, Tunings tunings, List<(int, string)> adjustmentTracker)
        {
            Dictionary<int, double> adjustments = [];
            var seasonProjectionDictionary = seasonProjections.ToDictionary(p => p.PlayerId, p => p.ProjectedPoints);
            var currentEliteRookieWRsWithProjections = (await playerService.GetCurrentRookies(_season.CurrentSeason, Position.WR.ToString()))
                                                       .Where(r => r.DraftPick <= tunings.EliteWRDraftPositionMax).ToDictionary(c => c.PlayerId);
            var playerTeams = await teamsService.GetPlayerTeams(_season.CurrentSeason, seasonProjections.Select(s => s.PlayerId));
            
            foreach (var sp in seasonProjections)
            {
                if (currentEliteRookieWRsWithProjections.TryGetValue(sp.PlayerId, out var rookieRecord))
                {
                    var otherReceiversOnTeam = playerTeams.Where(p => p.TeamId == rookieRecord.TeamId && p.PlayerId != sp.PlayerId);
                    if (!otherReceiversOnTeam.Any(o => seasonProjectionDictionary.TryGetValue(o.PlayerId, out var proj) && proj > tunings.WR1MinPoints))
                    {
                        adjustments.Add(sp.PlayerId, sp.ProjectedPoints * (tunings.EliteWRRookieTopReceiverFactor - 1));
                        adjustmentTracker.Add((sp.PlayerId, Adjustment.EliteRookieWRTopTarget.ToString()));
                    }
                        
                }
            }
            return adjustments;

        }

        private async Task<Dictionary<int, double>> LeadRBPromotionAdjustment(IEnumerable<SeasonProjection> seasonProjections, IEnumerable<TeamChange> rbTeamChanges, Tunings tunings, List<(int, string)> adjustmentTracker)
        {
            Dictionary<int, double> adjustments = [];
            var playerTeamDictionary = (await teamsService.GetPlayerTeams(_season.CurrentSeason, seasonProjections.Select(s => s.PlayerId))).ToDictionary(p => p.PlayerId, t => t.TeamId);
            var seasonProjectionDictionary = seasonProjections.ToDictionary(s => s.PlayerId, s => s.ProjectedPoints);
            foreach (var sp in seasonProjections)
            {
                if (playerTeamDictionary.TryGetValue(sp.PlayerId, out var teamId))
                {
                    var newRBsOnTeam = rbTeamChanges.Where(r => r.CurrentTeamId == teamId).Select(r => r.PlayerId);
                    var previousRBsOnTeam = rbTeamChanges.Where(r => r.PreviousTeamId == teamId).Select(r => r.PlayerId);
                    var newRBsOnTeamProjections = seasonProjectionDictionary.Where(s => newRBsOnTeam.Contains(s.Key));
                    var previousRBsOnTeamProjections = seasonProjectionDictionary.Where(s => previousRBsOnTeam.Contains(s.Key));
                    
                    if (!newRBsOnTeamProjections.Any(n => n.Value > tunings.RB1MinPoints) && previousRBsOnTeamProjections.Any(p => p.Value > tunings.RB1MinPoints))
                    {
                        var seasonData = await statisticsService.GetSeasonData<SeasonDataRB>(Position.RB, sp.PlayerId, true);
                        if (seasonData.Count < 3 && YardsPerCarryCareerAverage(seasonData) > tunings.RBPromotionMinYardsPerCarry)
                        {
                            adjustments.Add(sp.PlayerId, sp.ProjectedPoints * (tunings.RBPromotionFactor - 1));
                            adjustmentTracker.Add((sp.PlayerId, Adjustment.LeadRBPromotion.ToString()));
                        }
                            
                    }
                }
            }
            return adjustments;
        }
        private double QBProjectionRatio(string position, double previousProjection, double currentProjection, bool qbIsRookie, Tunings tunings)
        {
            if (previousProjection == currentProjection) return 1;

            if (qbIsRookie && currentProjection / previousProjection > 1) return 1;

            if (position == Position.RB.ToString())
            {
               return previousProjection > currentProjection ? Math.Max(tunings.NewQBFloor, currentProjection / previousProjection)
                                            : Math.Min(tunings.NewQBCeilingForRB, currentProjection / previousProjection);
            }

            return previousProjection > currentProjection ? Math.Max(tunings.NewQBFloor, currentProjection / previousProjection)
                                            : Math.Min(tunings.NewQBCeiling, currentProjection / previousProjection);
        }

        private async Task<IEnumerable<QuarterbackChange>> GetQuarterbackChanges(IEnumerable<TeamChange> allTeamChanges, Tunings tunings)
        {
            if (cache.TryGetValue(Cache.QuarterbackChanges, out IEnumerable<QuarterbackChange>? changes) && changes != null) return changes;

           List<QuarterbackChange> quarterbackChanges = [];
            var qbTeamChanges = allTeamChanges.Where(t => t.Position == Position.QB);
            var wrTeamChanges = allTeamChanges.Where(p => p.Position == Position.WR);
            var teTeamChanges = allTeamChanges.Where(p => p.Position == Position.TE);
            var rbTeamChanges = await (GetTeamChangesForReceivingRBs(allTeamChanges.Where(p => p.Position == Position.RB), tunings)).ToListAsync();
            
            var currentRookies = await playerService.GetCurrentRookies(_season.CurrentSeason, Position.QB.ToString());
            foreach (var change in qbTeamChanges)
            {
                quarterbackChanges.AddRange(await GetChangeRecordsForCurrentTeam(change, currentRookies, tunings));
                quarterbackChanges.AddRange(await GetChangeRecordsForPreviousTeam(change, currentRookies, tunings));
            }
            var rookieQbs = currentRookies.Where(c => !quarterbackChanges.Any(q => q.CurrentQBId == c.PlayerId));
            quarterbackChanges.AddRange(await GetChangeRecordsForRookieQBs(rookieQbs, tunings));
            var distinctQuarterbackChanges = quarterbackChanges.DistinctBy(qc => qc.PlayerId).ToList();

            var passCatcherTeamChanges = (wrTeamChanges).Union(teTeamChanges).Union(rbTeamChanges).ToDictionary(t => t.PlayerId);
            foreach (var qc in distinctQuarterbackChanges)
            {
                if (passCatcherTeamChanges.TryGetValue(qc.PlayerId, out var teamChange))
                {
                    qc.PreviousQBId = (await statisticsService.GetSeasonDataByTeamIdAndPosition<SeasonDataQB>(teamChange.PreviousTeamId, Position.QB, _season.CurrentSeason - 1))
                                      .OrderByDescending(s => s.Games).First().PlayerId;
                }
            }

            foreach(var pc in passCatcherTeamChanges.Where(pc => !distinctQuarterbackChanges.Any(d => d.PlayerId == pc.Key)))
            {
                var previousQB = (await statisticsService.GetSeasonDataByTeamIdAndPosition<SeasonDataQB>(pc.Value.PreviousTeamId, Position.QB, _season.CurrentSeason - 1))
                                      .OrderByDescending(s => s.Games).First().PlayerId;
                var newQBs = await teamsService.GetPlayersByTeamIdAndPosition(pc.Value.CurrentTeamId, Position.QB, _season.CurrentSeason, true);
                var newQB = (await playerService.GetSeasonProjections(newQBs.Select(n => n.PlayerId), _season.CurrentSeason)).OrderByDescending(q => q.Value).First().Key;

                distinctQuarterbackChanges.Add(new QuarterbackChange
                {
                    PlayerId = pc.Key,
                    Season = _season.CurrentSeason,
                    PreviousQBId = previousQB,
                    CurrentQBId = newQB,
                    CurrentQBIsRookie = currentRookies.Any(c => c.PlayerId == newQB)
                });
            }
            cache.Set(Cache.QuarterbackChanges, distinctQuarterbackChanges);
            return distinctQuarterbackChanges;
        }
        private async Task<IEnumerable<QuarterbackChange>> GetChangeRecordsForCurrentTeam(TeamChange change, List<Rookie> currentRookies, Tunings tunings)
        {
            var QBsOnNewTeam = (await teamsService.GetPlayersByTeamIdAndPosition(change.CurrentTeamId, Position.QB, _season.CurrentSeason, true)).Select(p => p.PlayerId);
            var qbProjection = (await playerService.GetSeasonProjections(QBsOnNewTeam, _season.CurrentSeason)).OrderByDescending(q => q.Value).First();
            if (qbProjection.Key == change.PlayerId)
            {
                var newPassCatchers = await GetPassCatchers(change.CurrentTeamId, tunings);
                if (newPassCatchers.Any())
                {
                    var previousQB = (await statisticsService.GetSeasonDataByTeamIdAndPosition<SeasonDataQB>(newPassCatchers.First().TeamId, Position.QB, _season.CurrentSeason - 1)).OrderByDescending(s => s.Games).First();
                    return newPassCatchers.Select(n => new QuarterbackChange
                    {
                        PlayerId = n.PlayerId,
                        Season = _season.CurrentSeason,
                        PreviousQBId = previousQB.PlayerId,
                        CurrentQBId = change.PlayerId,
                        CurrentQBIsRookie = currentRookies.Any(c => c.PlayerId == change.PlayerId)
                    });
                }
            }
            return Enumerable.Empty<QuarterbackChange>();
        }

        private async Task<IEnumerable<QuarterbackChange>> GetChangeRecordsForPreviousTeam(TeamChange change, List<Rookie> currentRookies, Tunings tunings)
        {
            var previousPassCatchers = await GetPassCatchers(change.PreviousTeamId, tunings);
            if (previousPassCatchers.Any())
            {
                var previousTeamQBs = (await teamsService.GetPlayersByTeamIdAndPosition(change.PreviousTeamId, Position.QB, _season.CurrentSeason)).Select(p => p.PlayerId);
                var previousTeamQBProjections = (await playerService.GetSeasonProjections(previousTeamQBs, _season.CurrentSeason)).OrderByDescending(q => q.Value).FirstOrDefault();

                if (previousTeamQBProjections.Value > 0)
                {
                    return previousPassCatchers.Select(p => new QuarterbackChange
                    {
                        PlayerId = p.PlayerId,
                        Season = _season.CurrentSeason,
                        PreviousQBId = change.PlayerId,
                        CurrentQBId = previousTeamQBProjections.Key,
                        CurrentQBIsRookie = currentRookies.Any(c => c.PlayerId == previousTeamQBProjections.Key)
                    });
                }
            }
            return Enumerable.Empty<QuarterbackChange>();
        }

        private async Task<IEnumerable<QuarterbackChange>> GetChangeRecordsForRookieQBs(IEnumerable<Rookie>? rookieQbs, Tunings tunings)
        {
            if (rookieQbs == null) return Enumerable.Empty<QuarterbackChange>();

            List<QuarterbackChange> changeRecords = [];

            foreach (var rq in rookieQbs)
            {
                var passCatchersForRookie = await GetPassCatchers(rq.TeamId, tunings);
                var previousQB = (await statisticsService.GetSeasonDataByTeamIdAndPosition<SeasonDataQB>(rq.TeamId, Position.QB, _season.CurrentSeason - 1)).OrderByDescending(s => s.Games).First();
                changeRecords.AddRange(passCatchersForRookie.Select(p => new QuarterbackChange
                {
                    PlayerId = p.PlayerId,
                    Season = _season.CurrentSeason,
                    PreviousQBId = previousQB.PlayerId,
                    CurrentQBId = rq.PlayerId,
                    CurrentQBIsRookie = true
                }));
            }
            return changeRecords;
        }

        private async Task<bool> IsRushingQB(int playerId, Tunings tunings)
        {
            var fantasyPercentage = await fantasyAnalysisService.GetQBSeasonFantasyPercentageByPlayerId(_season.CurrentSeason - 1, playerId);
            return fantasyPercentage.RushTDShare + fantasyPercentage.RushYDShare > tunings.RushingQBThreshold;
        }

        private async Task<bool> IsReceivingRB(int playerId, Tunings tunings)
        {
            var fantasyPercentage = await fantasyAnalysisService.GetRBSeasonFantasyPercentageByPlayerId(_season.CurrentSeason - 1, playerId);
            return fantasyPercentage.RecShare + fantasyPercentage.RecYDShare + fantasyPercentage.RecTDShare > tunings.ReceivingRBThreshold;
        }

        private IAsyncEnumerable<TeamChange> GetTeamChangesForReceivingRBs(IEnumerable<TeamChange> allRBTeamChanges, Tunings tunings) => allRBTeamChanges.ToAsyncEnumerable().WhereAwait(async a => await IsReceivingRB(a.PlayerId, tunings));
        private IAsyncEnumerable<PlayerTeam> GetReceivingRBsByTeamId(IEnumerable<PlayerTeam> rbTeams, Tunings tunings) => rbTeams.ToAsyncEnumerable().WhereAwait(async a => await IsReceivingRB(a.PlayerId, tunings));
        private static double YardsPerCarryCareerAverage(List<SeasonDataRB> seasonData) => seasonData.Any(s => s.RushingAtt > 0) ? seasonData.Sum(s => s.RushingYds) / seasonData.Sum(s => s.RushingAtt) : 0;
        private async Task<IEnumerable<PlayerTeam>> GetPassCatchers(int teamId, Tunings tunings)
        {
            var allRbs = await teamsService.GetPlayersByTeamIdAndPosition(teamId, Position.RB, _season.CurrentSeason);
            return (await teamsService.GetPlayersByTeamIdAndPosition(teamId, Position.WR, _season.CurrentSeason))
                                            .Union(await teamsService.GetPlayersByTeamIdAndPosition(teamId, Position.TE, _season.CurrentSeason))
                                            .Union(await GetReceivingRBsByTeamId(allRbs, tunings).ToListAsync());
        }

        private async Task<IEnumerable<SeasonProjection>> QBSeasonProjectionAdjustments(IEnumerable<SeasonProjection> seasonProjections, IEnumerable<TeamChange> allTeamChanges, Tunings tunings, int seasonGames, SeasonAdjustments seasonAdjustments)
        {
            List<Dictionary<int, double>> adjustments = [];
            List<(int, string)> adjustmentTracker = []; 

            if (seasonAdjustments.InjuryAdjustment) adjustments.Add(await InjuryAdjustment(seasonProjections, seasonGames, adjustmentTracker));
            if (seasonAdjustments.BackupQuarterbackAdjustment) adjustments.Add(await BackupQuarterbackAdjustment(seasonProjections, adjustmentTracker));
            if (seasonAdjustments.SuspensionAdjustment) adjustments.Add(await SuspensionAdjustment(seasonProjections, seasonGames, adjustmentTracker));
            if (seasonAdjustments.VeteranQBonNewTeamAdjustment) adjustments.Add(await VeteranQBOnNewTeamAdjustment(seasonProjections, allTeamChanges.Where(t => t.Position == Position.QB), tunings, adjustmentTracker));

            return AccumulateAdjustments(seasonProjections, adjustments, adjustmentTracker);
        }

        private async Task<IEnumerable<SeasonProjection>> RBSeasonProjectionAdjustments(IEnumerable<SeasonProjection> seasonProjections, IEnumerable<TeamChange> allTeamChanges, Tunings tunings, int seasonGames, SeasonAdjustments seasonAdjustments)
        {
            List<Dictionary<int, double>> adjustments = [];
            List<(int, string)> adjustmentTracker = [];

            var qbChanges = (await GetQuarterbackChanges(allTeamChanges, tunings)).ToDictionary(q => q.PlayerId);

            if (seasonAdjustments.InjuryAdjustment) adjustments.Add(await InjuryAdjustment(seasonProjections, seasonGames, adjustmentTracker));
            if (seasonAdjustments.SuspensionAdjustment) adjustments.Add(await SuspensionAdjustment(seasonProjections, seasonGames, adjustmentTracker));
            if (seasonAdjustments.DownwardTrendingAdjustment) adjustments.Add(await DownwardTrendingAdjustment(seasonProjections, adjustmentTracker));
            if (seasonAdjustments.SharedBackfieldAdjustment) adjustments.Add(await SharedBackfieldAdjustment(seasonProjections, allTeamChanges.Where(t => t.Position == Position.RB), tunings, adjustmentTracker));
            if (seasonAdjustments.QuarterbackChangeAdjustment) adjustments.Add(await QuarterbackChangeAdjustment(seasonProjections, qbChanges, tunings, adjustmentTracker));
            if (seasonAdjustments.LeadRBPromotionAdjustment) adjustments.Add(await LeadRBPromotionAdjustment(seasonProjections, allTeamChanges.Where(t => t.Position == Position.RB), tunings, adjustmentTracker));

            return AccumulateAdjustments(seasonProjections, adjustments, adjustmentTracker);
        }

        private async Task<IEnumerable<SeasonProjection>> WRSeasonProjectionAdjustments(IEnumerable<SeasonProjection> seasonProjections, IEnumerable<TeamChange> allTeamChanges, Tunings tunings, int seasonGames, SeasonAdjustments seasonAdjustments)
        {
            List<Dictionary<int, double>> adjustments = [];
            List<(int, string)> adjustmentTracker = [];

            var qbChanges = (await GetQuarterbackChanges(allTeamChanges, tunings)).ToDictionary(q => q.PlayerId);

            if (seasonAdjustments.InjuryAdjustment) adjustments.Add(await InjuryAdjustment(seasonProjections, seasonGames, adjustmentTracker));
            if (seasonAdjustments.SuspensionAdjustment) adjustments.Add(await SuspensionAdjustment(seasonProjections, seasonGames, adjustmentTracker));
            if (seasonAdjustments.EliteRookieWRTopTargetAdjustment) adjustments.Add(await EliteRookieWRTopTargetAdjustment(seasonProjections, tunings, adjustmentTracker));
            if (seasonAdjustments.PreviousSeasonBackupQuarterbackAdjustment) adjustments.Add(await PreviousSeasonBackupQuarterbackAdjustment(seasonProjections, qbChanges, tunings, seasonGames, adjustmentTracker));
            if (seasonAdjustments.SharedReceivingDutiesAdjustment) adjustments.Add(await SharedReceivingDutiesAdjustment(seasonProjections, allTeamChanges.Where(t => t.Position == Position.WR), tunings, adjustmentTracker));
            if (seasonAdjustments.QuarterbackChangeAdjustment) adjustments.Add(await QuarterbackChangeAdjustment(seasonProjections, qbChanges, tunings, adjustmentTracker));

            return AccumulateAdjustments(seasonProjections, adjustments, adjustmentTracker);
        }

        private async Task<IEnumerable<SeasonProjection>> TESeasonProjectionAdjustment(IEnumerable<SeasonProjection> seasonProjections, IEnumerable<TeamChange> allTeamChanges, Tunings tunings, int seasonGames, SeasonAdjustments seasonAdjustments)
        {
            List<Dictionary<int, double>> adjustments = [];
            List<(int, string)> adjustmentTracker = [];

            var qbChanges = (await GetQuarterbackChanges(allTeamChanges, tunings)).ToDictionary(q => q.PlayerId);

            if (seasonAdjustments.InjuryAdjustment) adjustments.Add(await InjuryAdjustment(seasonProjections, seasonGames, adjustmentTracker));
            if (seasonAdjustments.SuspensionAdjustment) adjustments.Add(await SuspensionAdjustment(seasonProjections, seasonGames, adjustmentTracker));
            if (seasonAdjustments.PreviousSeasonBackupQuarterbackAdjustment) adjustments.Add(await PreviousSeasonBackupQuarterbackAdjustment(seasonProjections, qbChanges, tunings, seasonGames, adjustmentTracker));
            if (seasonAdjustments.QuarterbackChangeAdjustment) adjustments.Add(await QuarterbackChangeAdjustment(seasonProjections, qbChanges, tunings, adjustmentTracker));

            return AccumulateAdjustments(seasonProjections, adjustments, adjustmentTracker);
        }

        private static IEnumerable<SeasonProjection> AccumulateAdjustments(IEnumerable<SeasonProjection> seasonProjections, List<Dictionary<int, double>> adjustments, List<(int, string)> adjustmentTracker)
        {
            foreach (var sp in seasonProjections)
            {
                sp.ProjectedPoints += adjustments.Where(a => a.ContainsKey(sp.PlayerId)).Select(d => d[sp.PlayerId]).DefaultIfEmpty(0).Sum();
                var playerAdjustments = adjustmentTracker.Where(a => a.Item1 == sp.PlayerId).Select(a => a.Item2).ToArray();
                sp.Adjustments = string.Join(", ", playerAdjustments);
            }
                
            return seasonProjections;
        } 
    }
}
