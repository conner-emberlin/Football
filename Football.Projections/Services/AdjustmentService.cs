using Football.Enums;
using Football.Models;
using Football.Fantasy.Interfaces;
using Football.Players.Interfaces;
using Football.Projections.Models;
using Football.Projections.Interfaces;
using Microsoft.Extensions.Options;
using Football.Players.Models;
using Football.Players.Services;


namespace Football.Projections.Services
{
    public class AdjustmentService(IPlayersService playerService, IMatchupAnalysisService mathupAnalysisService, IStatisticsService statisticsService, IOptionsMonitor<Season> season,
        IOptionsMonitor<Tunings> tunings, IOptionsMonitor<WeeklyTunings> weeklyTunings) : IAdjustmentService
    {
        private readonly Season _season = season.CurrentValue;
        private readonly Tunings _tunings = tunings.CurrentValue;
        private readonly WeeklyTunings _weeklyTunings = weeklyTunings.CurrentValue;

        public async Task<IEnumerable<SeasonProjection>> AdjustmentEngine(IEnumerable<SeasonProjection> seasonProjections)
        {
            seasonProjections = await InjuryAdjustment(seasonProjections);
            seasonProjections = await SuspensionAdjustment(seasonProjections);

            var position = seasonProjections.First().Position;
            if (position == Position.WR.ToString() || position == Position.TE.ToString())
                seasonProjections = await QuarterbackChangeAdjustment(seasonProjections);
            return seasonProjections;
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

        private async Task<IEnumerable<SeasonProjection>> SuspensionAdjustment(IEnumerable<SeasonProjection> seasonProjections)
        {
            var playerSuspensions = (await playerService.GetPlayerSuspensions(_season.CurrentSeason)).ToDictionary(s => s.PlayerId, s => s.Length);
            foreach (var s in seasonProjections)
            {
                if (playerSuspensions.TryGetValue(s.PlayerId, out var length)) s.ProjectedPoints -= (s.ProjectedPoints / _season.Games) * length;
            }
            return seasonProjections;
        }
        private async Task<IEnumerable<SeasonProjection>> QuarterbackChangeAdjustment(IEnumerable<SeasonProjection> seasonProjections)
        {
            var qbChanges = (await GetQuarterbackChanges()).ToDictionary(q => q.PlayerId);
            foreach(var s in seasonProjections)
            {
                if (qbChanges.TryGetValue(s.PlayerId, out var changeRecord))
                {
                    var qbProjections = await playerService.GetSeasonProjections([changeRecord.PreviousQBId, changeRecord.CurrentQBId], _season.CurrentSeason);
                    var previousQbProjection = qbProjections.TryGetValue(changeRecord.PreviousQBId, out var projectPrev) ? projectPrev : _tunings.AverageQBProjection;
                    var currentQbProjection = qbProjections.TryGetValue(changeRecord.CurrentQBId, out var currentPrev) ? currentPrev : _tunings.AverageQBProjection;
                    s.ProjectedPoints *= QBProjectionRatio(previousQbProjection, currentQbProjection);
                }
            }
            return seasonProjections;
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
        private double QBProjectionRatio(double previousProjection, double currentProjection)
        {
            if (previousProjection == currentProjection) return 1;

            return previousProjection > currentProjection ? Math.Max(_tunings.NewQBFloor, currentProjection / previousProjection)
                                            : Math.Min(_tunings.NewQBCeiling, currentProjection / previousProjection);
        }

        public async Task<IEnumerable<QuarterbackChange>> GetQuarterbackChanges()
        {
            List<QuarterbackChange> quarterbackChanges = [];
            var qbTeamChanges = await playerService.GetTeamChanges(_season.CurrentSeason, Position.QB);
            var currentRookies = await playerService.GetCurrentRookies(_season.CurrentSeason, Position.QB.ToString());

            foreach (var change in qbTeamChanges)
            {
                //change record for pass catchers on qb's new team
                var newPassCatchers = (await playerService.GetPlayersByTeamIdAndPosition(change.CurrentTeamId, Position.WR, _season.CurrentSeason)).Union(await playerService.GetPlayersByTeamIdAndPosition(change.CurrentTeamId, Position.TE, _season.CurrentSeason));
                if (newPassCatchers.Any())
                {
                    var previousQB = (await statisticsService.GetSeasonDataByTeamIdAndPosition<SeasonDataQB>(newPassCatchers.First().TeamId, Position.QB, _season.CurrentSeason - 1)).OrderByDescending(s => s.Games).First();
                    var rookieQB = (currentRookies.FirstOrDefault(r => r.TeamId == newPassCatchers.First().TeamId))?.PlayerId;
                    quarterbackChanges.AddRange(newPassCatchers.Select(n => new QuarterbackChange
                    {
                        PlayerId = n.PlayerId,
                        Name = n.Name,
                        Season = _season.CurrentSeason,
                        PreviousQBId = previousQB.PlayerId,
                        CurrentQBId = rookieQB ?? change.PlayerId
                    }));
                }
                //change record for pass catcheres on qb's previous team
                var previousPassCatchers = (await playerService.GetPlayersByTeamIdAndPosition(change.PreviousTeamId, Position.WR, _season.CurrentSeason)).Union(await playerService.GetPlayersByTeamIdAndPosition(change.PreviousTeamId, Position.TE, _season.CurrentSeason));                                         
                if (previousPassCatchers.Any())
                {
                    var previousTeamQBs = (await playerService.GetPlayersByTeamIdAndPosition(change.PreviousTeamId, Position.QB, _season.CurrentSeason)).Select(p => p.PlayerId);
                    var previousTeamQBProjections = (await playerService.GetSeasonProjections(previousTeamQBs, _season.CurrentSeason)).OrderByDescending(q => q.Value).FirstOrDefault();

                    if (previousTeamQBProjections.Value > 0)
                    {
                        quarterbackChanges.AddRange(previousPassCatchers.Select(p => new QuarterbackChange
                        {
                            PlayerId = p.PlayerId,
                            Name = p.Name,
                            Season = _season.CurrentSeason,
                            PreviousQBId = change.PlayerId,
                            CurrentQBId = previousTeamQBProjections.Key
                        }));
                    }
                }
            }
            //change record for players with new rookie qb whose previous qb did not change teams
            foreach (var rq in currentRookies)
            {
                if (!quarterbackChanges.Any(q => q.CurrentQBId == rq.PlayerId))
                {
                    var passCatchersForRookie = (await playerService.GetPlayersByTeamIdAndPosition(rq.TeamId, Position.WR, _season.CurrentSeason)).Union(await playerService.GetPlayersByTeamIdAndPosition(rq.TeamId, Position.TE, _season.CurrentSeason));
                    var previousQB = (await statisticsService.GetSeasonDataByTeamIdAndPosition<SeasonDataQB>(rq.TeamId, Position.QB, _season.CurrentSeason - 1)).OrderByDescending(s => s.Games).First();
                    quarterbackChanges.AddRange(passCatchersForRookie.Select(p => new QuarterbackChange
                    {
                        PlayerId = p.PlayerId,
                        Name = p.Name,
                        Season = _season.CurrentSeason,
                        PreviousQBId = previousQB.PlayerId,
                        CurrentQBId =  rq.PlayerId
                    }));
                }
            }

            var distinctQuarterbackChanges = quarterbackChanges.DistinctBy(qc => qc.PlayerId);

            //if pass catcher with quarterback change also changed teams, adjust previous team to qb on previous team rather than the previous qb of new team
            var passCatcherTeamChanges = (await playerService.GetTeamChanges(_season.CurrentSeason, Position.WR)).Union(await playerService.GetTeamChanges(_season.CurrentSeason, Position.TE)).ToDictionary(t => t.PlayerId);
            foreach (var qc in distinctQuarterbackChanges)
            {
                if (passCatcherTeamChanges.TryGetValue(qc.PlayerId, out var teamChange))
                {
                    qc.PreviousQBId = (await statisticsService.GetSeasonDataByTeamIdAndPosition<SeasonDataQB>(teamChange.PreviousTeamId, Position.QB, _season.CurrentSeason - 1)).OrderByDescending(s => s.Games).First().PlayerId;
                }
            }
            return distinctQuarterbackChanges;
        }
    }
}
