using Football.Enums;
using Football.Models;
using Football.Fantasy.Interfaces;
using Football.Players.Interfaces;
using Football.Projections.Models;
using Football.Projections.Interfaces;
using Microsoft.Extensions.Options;


namespace Football.Projections.Services
{
    public class AdjustmentService(IPlayersService playerService, IMatchupAnalysisService mathupAnalysisService, IOptionsMonitor<Season> season,
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
            var qbChanges = (await playerService.GetQuarterbackChanges(_season.CurrentSeason)).ToDictionary(q => q.PlayerId);
            foreach(var s in seasonProjections)
            {
                if (qbChanges.TryGetValue(s.PlayerId, out var changeRecord))
                {
                    var qbProjections = await playerService.GetSeasonProjections([changeRecord.PreviousQB, changeRecord.CurrentQB], _season.CurrentSeason);
                    var previousQbProjection = qbProjections.TryGetValue(changeRecord.PreviousQB, out var projectPrev) ? projectPrev : _tunings.AverageQBProjection;
                    var currentQbProjection = qbProjections.TryGetValue(changeRecord.CurrentQB, out var currentPrev) ? currentPrev : _tunings.AverageQBProjection;
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
    }
}
