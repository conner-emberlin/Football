using Football.Enums;
using Football.Models;
using Football.Fantasy.Interfaces;
using Football.Players.Interfaces;
using Football.Projections.Models;
using Football.Projections.Interfaces;
using Microsoft.Extensions.Options;
using Football.Players.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Football.Projections.Services
{
    public class AdjustmentService(IPlayersService playerService, IMatchupAnalysisService mathupAnalysisService, IStatisticsService statisticsService, IFantasyAnalysisService fantasyAnalysisService,
        IFantasyDataService fantasyDataService, IOptionsMonitor<Season> season, IOptionsMonitor<Tunings> tunings, IOptionsMonitor<WeeklyTunings> weeklyTunings, IMemoryCache cache) : IAdjustmentService
    {
        private readonly Season _season = season.CurrentValue;
        private readonly Tunings _tunings = tunings.CurrentValue;
        private readonly WeeklyTunings _weeklyTunings = weeklyTunings.CurrentValue;

        public async Task<IEnumerable<SeasonProjection>> AdjustmentEngine(IEnumerable<SeasonProjection> seasonProjections)
        {
            _ = Enum.TryParse(seasonProjections.First().Position, out Position position);
            var allTeamChanges = await playerService.GetAllTeamChanges(_season.CurrentSeason);
           
            seasonProjections = await InjuryAdjustment(seasonProjections);
            seasonProjections = await SuspensionAdjustment(seasonProjections);

            return position switch
            {
                Position.QB => await QBSeasonProjectionAdjustments(seasonProjections, allTeamChanges),
                Position.RB => await RBSeasonProjectionAdjustments(seasonProjections, allTeamChanges),
                Position.WR => await WRSeasonProjectionAdjustments(seasonProjections, allTeamChanges),
                Position.TE => await TESeasonProjectionAdjustment(seasonProjections, allTeamChanges),
                _ => seasonProjections
            };
        }
        public async Task<List<WeekProjection>> AdjustmentEngine(List<WeekProjection> weekProjections)
        {
            weekProjections = await MatchupAdjustment(weekProjections);
            weekProjections = await InjuryAdustment(weekProjections);
            return weekProjections;
        }

        private async Task<IEnumerable<SeasonProjection>> InjuryAdjustment(IEnumerable<SeasonProjection> seasonProjections)
        {
            var playerInjuries = (await playerService.GetPlayerInjuries(_season.CurrentSeason)).ToDictionary(i => i.PlayerId, i => i.Games);
            foreach(var s in seasonProjections)
            {
                if (playerInjuries.TryGetValue(s.PlayerId, out var games))
                    s.ProjectedPoints -= (s.ProjectedPoints / _season.Games) * games;
            }
            return seasonProjections;
        }
        private async Task<IEnumerable<SeasonProjection>> SuspensionAdjustment(IEnumerable<SeasonProjection> seasonProjections)
        {
            var playerSuspensions = (await playerService.GetPlayerSuspensions(_season.CurrentSeason)).ToDictionary(s => s.PlayerId, s => s.Length);
            foreach (var s in seasonProjections)
            {
                if (playerSuspensions.TryGetValue(s.PlayerId, out var length)) s.ProjectedPoints -= (s.ProjectedPoints / _season.Games) * length;
            }
            return seasonProjections;
        }
        private async Task<IEnumerable<SeasonProjection>> QuarterbackChangeAdjustment(IEnumerable<SeasonProjection> seasonProjections, Dictionary<int, QuarterbackChange> qbChanges)
        {          
            foreach(var s in seasonProjections)
            {
                if (qbChanges.TryGetValue(s.PlayerId, out var changeRecord) && !await IsRushingQB(changeRecord.CurrentQBId))
                {
                    var qbProjections = await playerService.GetSeasonProjections([changeRecord.PreviousQBId, changeRecord.CurrentQBId], _season.CurrentSeason);
                    var previousQbProjection = qbProjections.TryGetValue(changeRecord.PreviousQBId, out var projectPrev) ? projectPrev : _tunings.AverageQBProjection;
                    var currentQbProjection = qbProjections.TryGetValue(changeRecord.CurrentQBId, out var currentPrev) ? currentPrev : _tunings.AverageQBProjection;
                    s.ProjectedPoints *= QBProjectionRatio(previousQbProjection, currentQbProjection, changeRecord.CurrentQBIsRookie);
                }
            }
            return seasonProjections;
        }

        private async Task<IEnumerable<SeasonProjection>> PreviousSeasonBackupQuarterbackAdjustment(IEnumerable<SeasonProjection> seasonProjections, Dictionary<int, QuarterbackChange> qbChanges)
        {
            var previousSeason = _season.CurrentSeason - 1;
            var playerTeamsDictionary = (await playerService.GetPlayerTeams(previousSeason, seasonProjections.Select(p => p.PlayerId))).ToDictionary(t => t.PlayerId);
            var currentStartedMissedGames = (await statisticsService.GetCurrentStartersThatMissedGamesLastSeason(_season.CurrentSeason, previousSeason, 15, _tunings.AverageQBProjection)).ToDictionary(c => c.PreviousSeasonTeamId);

            foreach (var sp in seasonProjections)
            {
                if (playerTeamsDictionary.TryGetValue(sp.PlayerId, out var playerTeam) && currentStartedMissedGames.TryGetValue(playerTeam.TeamId, out var starter) && !qbChanges.ContainsKey(sp.PlayerId))
                {
                    var previousSeasonBackup = (await statisticsService.GetSeasonDataByTeamIdAndPosition<SeasonDataQB>(starter.PreviousSeasonTeamId, Position.QB, previousSeason)).Where(s => s.PlayerId != starter.PlayerId).OrderByDescending(s => s.Attempts).FirstOrDefault();
                    if (previousSeasonBackup != null)
                    {
                        var backupFantasy = (await fantasyDataService.GetSeasonFantasy(previousSeasonBackup.PlayerId)).First(f => f.Season == previousSeason);
                        var backupFantasyPoints = (backupFantasy.FantasyPoints / backupFantasy.Games) * (_season.Games);
                        if (backupFantasyPoints < starter.CurrentSeasonProjection)
                        {
                            var projectionRatio = (starter.CurrentSeasonProjection / backupFantasyPoints) / (_season.Games - starter.PreviousSeasonGames);
                            sp.ProjectedPoints *= Math.Min(1 + projectionRatio, _tunings.BackupQBAdjustmentMax);
                        }
                    }
                }
               
            }
            return seasonProjections;

        }
        private async Task<IEnumerable<SeasonProjection>> SharedReceivingDutiesAdjustment(IEnumerable<SeasonProjection> seasonProjections, IEnumerable<TeamChange> wrTeamChanges)
        {
            var seasonProjectionDictionary = seasonProjections.ToDictionary(s => s.PlayerId, s => s.ProjectedPoints);
            
            var playerTeams = (await playerService.GetPlayerTeams(_season.CurrentSeason, seasonProjections.Select(p => p.PlayerId))).ToDictionary(p => p.PlayerId);
            foreach (var sp in seasonProjections)
            {
                if (playerTeams.TryGetValue(sp.PlayerId, out var team))
                {
                    var wrsThatMovedToProjectedPlayersTeam = wrTeamChanges.Where(w => w.CurrentTeamId == team.TeamId).Select(p => p.PlayerId);

                    if (wrsThatMovedToProjectedPlayersTeam.Contains(sp.PlayerId))
                    {
                        var otherReceivers = await playerService.GetPlayersByTeamIdAndPosition(team.TeamId, Position.WR, _season.CurrentSeason, true);
                        if (otherReceivers.Any(r => r.PlayerId != sp.PlayerId && seasonProjectionDictionary.TryGetValue(r.PlayerId, out var proj) && proj > _tunings.NewWRMinPoints))
                                sp.ProjectedPoints *= _tunings.NewWRAdjustmentFactor;
                    }
                    else if (wrsThatMovedToProjectedPlayersTeam.Any(w => seasonProjectionDictionary.TryGetValue(w, out var proj) && proj > _tunings.NewWRMinPoints))
                    {
                        sp.ProjectedPoints *= _tunings.ExistingWRAdjustmentFactor;
                    }                        
                }                
            }
            return seasonProjections;
        }

        private async Task<IEnumerable<SeasonProjection>> VeteranQBOnNewTeamAdjustment(IEnumerable<SeasonProjection> seasonProjections, IEnumerable<TeamChange> teamChanges)
        {
            var qbChangesDictionary = teamChanges.ToDictionary(t => t.PlayerId);
            foreach (var sp in seasonProjections)
            {
                if (qbChangesDictionary.TryGetValue(sp.PlayerId, out var teamChage))
                {
                    var yearsExperience = await statisticsService.GetYearsExperience(sp.PlayerId, Position.QB);
                    if (yearsExperience > _tunings.VetQBNewTeamYears) sp.ProjectedPoints *= _tunings.VetQBNewTeamFactor;
                }
            }
            return seasonProjections;
        }

        private async Task<IEnumerable<SeasonProjection>> SharedBackfieldAdjustment(IEnumerable<SeasonProjection> seasonProjections, IEnumerable<TeamChange> rbTeamChanges)
        {
            var seasonProjectionDictionary = seasonProjections.ToDictionary(s => s.PlayerId, s => s.ProjectedPoints);
            var playerTeams = (await playerService.GetPlayerTeams(_season.CurrentSeason, seasonProjections.Select(p => p.PlayerId))).ToDictionary(p => p.PlayerId);
            foreach (var sp in seasonProjections)
            {
                if (playerTeams.TryGetValue(sp.PlayerId, out var team))
                {
                    var rbsThatMovedToProjectedPlayersTeam = rbTeamChanges.Where(w => w.CurrentTeamId == team.TeamId).Select(p => p.PlayerId);

                    if (rbsThatMovedToProjectedPlayersTeam.Contains(sp.PlayerId))
                    {
                        var otherReceivers = await playerService.GetPlayersByTeamIdAndPosition(team.TeamId, Position.WR, _season.CurrentSeason, true);
                        if (otherReceivers.Any(r => r.PlayerId != sp.PlayerId && seasonProjectionDictionary.TryGetValue(r.PlayerId, out var proj) && proj > _tunings.NewWRMinPoints))
                            sp.ProjectedPoints *= _tunings.NewRBAdjustmentFactor;
                    }
                    else if (rbsThatMovedToProjectedPlayersTeam.Any(w => seasonProjectionDictionary.TryGetValue(w, out var proj) && proj > _tunings.NewWRMinPoints))
                    {
                        sp.ProjectedPoints *= _tunings.ExistingRBAdjustmentFactor;
                    }
                }

            }
            return seasonProjections;
        }

        private async Task<IEnumerable<SeasonProjection>> DownwardTrendingAdjustment(IEnumerable<SeasonProjection> seasonProjections)
        {
            foreach (var sp in seasonProjections)
            {
                var seasonFantasy = (await fantasyDataService.GetSeasonFantasy(sp.PlayerId)).OrderByDescending(f => f.Season);
                if (seasonFantasy.Count() > 4)
                {
                    var downwardTrend = seasonFantasy.ElementAt(0).FantasyPoints < seasonFantasy.ElementAt(1).FantasyPoints 
                                     && seasonFantasy.ElementAt(1).FantasyPoints < seasonFantasy.ElementAt(2).FantasyPoints;
                    
                    if (downwardTrend)
                    {
                        var diff = (seasonFantasy.ElementAt(2).FantasyPoints - seasonFantasy.ElementAt(0).FantasyPoints) / 2;
                        sp.ProjectedPoints -= diff;
                    }
                }
            }
            return seasonProjections;
        }

        private async Task<IEnumerable<SeasonProjection>> EliteRookieWRTopTargetAdjustment(IEnumerable<SeasonProjection> seasonProjections)
        {
            var seasonProjectionDictionary = seasonProjections.ToDictionary(p => p.PlayerId, p => p.ProjectedPoints);
            var currentEliteRookieWRsWithProjections = (await playerService.GetCurrentRookies(_season.CurrentSeason, Position.WR.ToString()))
                                                       .Where(r => r.DraftPick <= _tunings.EliteWRDraftPositionMax).ToDictionary(c => c.PlayerId);
            var playerTeams = await playerService.GetPlayerTeams(_season.CurrentSeason, seasonProjections.Select(s => s.PlayerId));
            
            foreach (var sp in seasonProjections)
            {
                if (currentEliteRookieWRsWithProjections.TryGetValue(sp.PlayerId, out var rookieRecord))
                {
                    var otherReceiversOnTeam = playerTeams.Where(p => p.TeamId == rookieRecord.TeamId && p.PlayerId != sp.PlayerId);
                    if (!otherReceiversOnTeam.Any(o => seasonProjectionDictionary.TryGetValue(o.PlayerId, out var proj) && proj > 240))
                        sp.ProjectedPoints *= _tunings.EliteWRRookieTopReceiverFactor;
                }
            }
            return seasonProjections;

        }
        private async Task<List<WeekProjection>> InjuryAdustment(List<WeekProjection> weeklyProjections)
        {
            var activeInjuries = await playerService.GetActiveInSeasonInjuries(_season.CurrentSeason);
            var injuredPlayerProjections = activeInjuries.Join(weeklyProjections,
                                                                ai => ai.PlayerId,
                                                                wp => wp.PlayerId,
                                                                (ai, wp) => new { InSeasonInjury = ai, WeekProjection = wp });
            foreach (var wp in weeklyProjections)
            {
                if (injuredPlayerProjections.Any(ip => ip.WeekProjection.PlayerId == wp.PlayerId)) wp.ProjectedPoints = 0;
            }
            return weeklyProjections;
        }

        private async Task<List<WeekProjection>> MatchupAdjustment(List<WeekProjection> weekProjections)
        {
            if (weekProjections.Count == 0) return weekProjections;

            _= Enum.TryParse(weekProjections.First().Position, out Position position);
            var matchupSeason = weekProjections.First().Week == 1 ? _season.CurrentSeason - 1 : _season.CurrentSeason;
            var matchupWeek = weekProjections.First().Week == 1 ? _season.Weeks + 1 : weekProjections.First().Week;

            var matchupRanks = await mathupAnalysisService.GetPositionalMatchupRankingsFromSQL(position, matchupSeason, matchupWeek);
            if (matchupRanks.Count > 0)
            {
                var avgMatchup = matchupRanks.ElementAt((int)Math.Floor((double)(matchupRanks.Count / 2)));
                var teamDictionary = (await playerService.GetPlayerTeams(_season.CurrentSeason, weekProjections.Select(w => w.PlayerId))).ToDictionary(p => p.PlayerId);
                var scheduleDictionary = (await playerService.GetWeeklySchedule(_season.CurrentSeason, weekProjections.First().Week)).ToDictionary(s => s.TeamId);

                foreach (var w in weekProjections)
                {
                    if (teamDictionary.TryGetValue(w.PlayerId, out var team))
                    {
                        var matchup = scheduleDictionary[team.TeamId];
                        if (matchup.OpposingTeamId == 0) w.ProjectedPoints = 0;
                        else 
                        {
                            var opponentRank = matchupRanks.FirstOrDefault(mr => mr.TeamId == matchup.OpposingTeamId);
                            if (opponentRank != null)
                            {
                                var ratio = opponentRank.AvgPointsAllowed / avgMatchup.AvgPointsAllowed;
                                var tamperedRatio = ratio > 1 ? Math.Min(ratio, _weeklyTunings.TamperedMax) : Math.Max(ratio, _weeklyTunings.TamperedMin);
                                w.ProjectedPoints = w.Position != Position.DST.ToString() ? w.ProjectedPoints * (tamperedRatio + 1) / 2 : tamperedRatio * w.ProjectedPoints;
                            }
                        }
                    }
                    else w.ProjectedPoints = 0;
                }
            }
            return weekProjections;
        }

        private double QBProjectionRatio(double previousProjection, double currentProjection, bool qbIsRookie)
        {
            if (previousProjection == currentProjection) return 1;

            if (qbIsRookie && currentProjection / previousProjection > 1) return 1;

            return previousProjection > currentProjection ? Math.Max(_tunings.NewQBFloor, currentProjection / previousProjection)
                                            : Math.Min(_tunings.NewQBCeiling, currentProjection / previousProjection);
        }

        private async Task<IEnumerable<QuarterbackChange>> GetQuarterbackChanges(IEnumerable<TeamChange> allTeamChanges)
        {
            if (cache.TryGetValue(Cache.QuarterbackChanges, out IEnumerable<QuarterbackChange>? changes) && changes != null) return changes;

           List<QuarterbackChange> quarterbackChanges = [];
            var qbTeamChanges = allTeamChanges.Where(t => t.Position == Position.QB);
            var wrTeamChanges = allTeamChanges.Where(p => p.Position == Position.WR);
            var teTeamChanges = allTeamChanges.Where(p => p.Position == Position.TE);
            var rbTeamChanges = await (GetTeamChangesForReceivingRBs(allTeamChanges.Where(p => p.Position == Position.RB))).ToListAsync();
            
            var currentRookies = await playerService.GetCurrentRookies(_season.CurrentSeason, Position.QB.ToString());
            foreach (var change in qbTeamChanges)
            {
                quarterbackChanges.AddRange(await GetChangeRecordsForCurrentTeam(change, currentRookies));
                quarterbackChanges.AddRange(await GetChangeRecordsForPreviousTeam(change, currentRookies));
            }
            var rookieQbs = currentRookies.Where(c => !quarterbackChanges.Any(q => q.CurrentQBId == c.PlayerId));
            quarterbackChanges.AddRange(await GetChangeRecordsForRookieQBs(rookieQbs));
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
                var newQBs = await playerService.GetPlayersByTeamIdAndPosition(pc.Value.CurrentTeamId, Position.QB, _season.CurrentSeason, true);
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
        private async Task<IEnumerable<QuarterbackChange>> GetChangeRecordsForCurrentTeam(TeamChange change, List<Rookie> currentRookies)
        {
            var QBsOnNewTeam = (await playerService.GetPlayersByTeamIdAndPosition(change.CurrentTeamId, Position.QB, _season.CurrentSeason, true)).Select(p => p.PlayerId);
            var qbProjection = (await playerService.GetSeasonProjections(QBsOnNewTeam, _season.CurrentSeason)).OrderByDescending(q => q.Value).First();
            if (qbProjection.Key == change.PlayerId)
            {
                var newPassCatchers = await GetPassCatchers(change.CurrentTeamId);
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

        private async Task<IEnumerable<QuarterbackChange>> GetChangeRecordsForPreviousTeam(TeamChange change, List<Rookie> currentRookies)
        {
            var previousPassCatchers = await GetPassCatchers(change.PreviousTeamId);
            if (previousPassCatchers.Any())
            {
                var previousTeamQBs = (await playerService.GetPlayersByTeamIdAndPosition(change.PreviousTeamId, Position.QB, _season.CurrentSeason)).Select(p => p.PlayerId);
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

        private async Task<IEnumerable<QuarterbackChange>> GetChangeRecordsForRookieQBs(IEnumerable<Rookie>? rookieQbs)
        {
            if (rookieQbs == null) return Enumerable.Empty<QuarterbackChange>();

            List<QuarterbackChange> changeRecords = [];

            foreach (var rq in rookieQbs)
            {
                var passCatchersForRookie = await GetPassCatchers(rq.TeamId);
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

        private async Task<bool> IsRushingQB(int playerId)
        {
            var fantasyPercentage = await fantasyAnalysisService.GetQBSeasonFantasyPercentageByPlayerId(_season.CurrentSeason - 1, playerId);
            return fantasyPercentage.RushTDShare + fantasyPercentage.RushYDShare > _tunings.RushingQBThreshold;
        }

        private async Task<bool> IsReceivingRB(int playerId)
        {
            var fantasyPercentage = await fantasyAnalysisService.GetRBSeasonFantasyPercentageByPlayerId(_season.CurrentSeason - 1, playerId);
            return fantasyPercentage.RecShare + fantasyPercentage.RecYDShare + fantasyPercentage.RecTDShare > _tunings.ReceivingRBThreshold;
        }

        private IAsyncEnumerable<TeamChange> GetTeamChangesForReceivingRBs(IEnumerable<TeamChange> allRBTeamChanges) => allRBTeamChanges.ToAsyncEnumerable().WhereAwait(async a => await IsReceivingRB(a.PlayerId));
        private IAsyncEnumerable<PlayerTeam> GetReceivingRBsByTeamId(IEnumerable<PlayerTeam> rbTeams) => rbTeams.ToAsyncEnumerable().WhereAwait(async a => await IsReceivingRB(a.PlayerId));

        private async Task<IEnumerable<PlayerTeam>> GetPassCatchers(int teamId)
        {
            var allRbs = await playerService.GetPlayersByTeamIdAndPosition(teamId, Position.RB, _season.CurrentSeason);
            return (await playerService.GetPlayersByTeamIdAndPosition(teamId, Position.WR, _season.CurrentSeason))
                                            .Union(await playerService.GetPlayersByTeamIdAndPosition(teamId, Position.TE, _season.CurrentSeason))
                                            .Union(await GetReceivingRBsByTeamId(allRbs).ToListAsync());
        }

        private async Task<IEnumerable<SeasonProjection>> QBSeasonProjectionAdjustments(IEnumerable<SeasonProjection> seasonProjections, IEnumerable<TeamChange> allTeamChanges)
        {
            seasonProjections = await VeteranQBOnNewTeamAdjustment(seasonProjections, allTeamChanges.Where(t => t.Position == Position.QB));
            return seasonProjections;
        }

        private async Task<IEnumerable<SeasonProjection>> RBSeasonProjectionAdjustments(IEnumerable<SeasonProjection> seasonProjections, IEnumerable<TeamChange> allTeamChanges)
        {
            var qbChanges = (await GetQuarterbackChanges(allTeamChanges)).ToDictionary(q => q.PlayerId);

            seasonProjections = await DownwardTrendingAdjustment(seasonProjections);
            seasonProjections = await SharedBackfieldAdjustment(seasonProjections, allTeamChanges.Where(t => t.Position == Position.RB));
            seasonProjections = await QuarterbackChangeAdjustment(seasonProjections, qbChanges);
            return seasonProjections;
        }

        private async Task<IEnumerable<SeasonProjection>> WRSeasonProjectionAdjustments(IEnumerable<SeasonProjection> seasonProjections, IEnumerable<TeamChange> allTeamChanges)
        {
            var qbChanges = (await GetQuarterbackChanges(allTeamChanges)).ToDictionary(q => q.PlayerId);
            seasonProjections = await EliteRookieWRTopTargetAdjustment(seasonProjections);
            seasonProjections = await PreviousSeasonBackupQuarterbackAdjustment(seasonProjections, qbChanges);
            seasonProjections = await SharedReceivingDutiesAdjustment(seasonProjections, allTeamChanges.Where(t => t.Position == Position.WR));
            seasonProjections = await QuarterbackChangeAdjustment(seasonProjections, qbChanges);
                                 
            return seasonProjections;
        }

        private async Task<IEnumerable<SeasonProjection>> TESeasonProjectionAdjustment(IEnumerable<SeasonProjection> seasonProjections, IEnumerable<TeamChange> allTeamChanges)
        {

            var qbChanges = (await GetQuarterbackChanges(allTeamChanges)).ToDictionary(q => q.PlayerId);
            seasonProjections = await PreviousSeasonBackupQuarterbackAdjustment(seasonProjections, qbChanges);

            return seasonProjections;
        }
    }
}
