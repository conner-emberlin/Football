using Football.Fantasy.Analysis.Interfaces;
using Football.Models;
using Football.Players.Interfaces;
using Football.Projections.Models;
using Football.Projections.Interfaces;
using Serilog;
using Microsoft.Extensions.Options;
using Football.Enums;
using Football.Players.Services;


namespace Football.Projections.Services
{
    public class AdjustmentService(IPlayersService playerService, IMatchupAnalysisService mathupAnalysisService, ILogger logger, IOptionsMonitor<Season> season,
        IOptionsMonitor<Tunings> tunings, IOptionsMonitor<WeeklyTunings> weeklyTunings) : IAdjustmentService
    {
        private readonly Season _season = season.CurrentValue;
        private readonly Tunings _tunings = tunings.CurrentValue;
        private readonly WeeklyTunings _weeklyTunings = weeklyTunings.CurrentValue;

        public async Task<List<SeasonProjection>> AdjustmentEngine(List<SeasonProjection> seasonProjections)
        {
            logger.Information("Calculating adjustments");
            seasonProjections = await InjuryAdjustment(seasonProjections);
            seasonProjections = await SuspensionAdjustment(seasonProjections);

            var position = seasonProjections.First().Position;
            if (position == Position.WR.ToString())
                seasonProjections = await QuarterbackChangeAdjustment(seasonProjections);
            return seasonProjections;
        }
        public async Task<List<WeekProjection>> AdjustmentEngine(List<WeekProjection> weekProjections)
        {
            logger.Information("Calculating adjustments");
            weekProjections = await MatchupAdjustment(weekProjections);
            weekProjections = await InjuryAdustment(weekProjections);
            return weekProjections;
        }

        private async Task<List<SeasonProjection>> InjuryAdjustment(List<SeasonProjection> seasonProjections)
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
                if (injuredPlayerProjections.Any(ip => ip.WeekProjection.PlayerId == wp.PlayerId))
                    wp.ProjectedPoints = 0;
            }
            return weeklyProjections;
        }

        private async Task<List<SeasonProjection>> SuspensionAdjustment(List<SeasonProjection> seasonProjections)
        {
            var playerSuspensions = (await playerService.GetPlayerSuspensions(_season.CurrentSeason)).ToDictionary(s => s.PlayerId, s => s.Length);
            foreach (var s in seasonProjections)
            {
                if (playerSuspensions.TryGetValue(s.PlayerId, out var length))
                    s.ProjectedPoints -= (s.ProjectedPoints / _season.Games) * length;
            }
            return seasonProjections;
        }
        private async Task<List<SeasonProjection>> QuarterbackChangeAdjustment(List<SeasonProjection> seasonProjections)
        {
            var qbChanges = await playerService.GetQuarterbackChanges(_season.CurrentSeason);
            foreach(var s in seasonProjections)
            {
                if (qbChanges.Any(q => q.PlayerId == s.PlayerId))
                {                   
                    var changeRecord = qbChanges.First(q => q.PlayerId == s.PlayerId);
                    logger.Information("QB Change found for player {p}: {n}. The New QB is {q}.", s.PlayerId, s.Name, changeRecord.CurrentQB);
                    var previousEPA = await playerService.GetEPA(changeRecord.PreviousQB, _season.CurrentSeason - 1);
                    var currentEPA = await playerService.GetEPA(changeRecord.CurrentQB, _season.CurrentSeason - 1);
                    s.ProjectedPoints *= EPARatio(previousEPA, currentEPA);
                }
            }
            return seasonProjections;
        }

        private async Task<List<WeekProjection>> MatchupAdjustment(List<WeekProjection> weekProjections)
        {
            _= Enum.TryParse(weekProjections.First().Position, out Position position);
            var matchupRanks = await mathupAnalysisService.PositionalMatchupRankings(position);
            if (matchupRanks.Count > 0)
            {
                var avgMatchup = matchupRanks.ElementAt((int)Math.Floor((double)(matchupRanks.Count / 2)));
                foreach (var w in weekProjections)
                {
                    var team = await playerService.GetPlayerTeam(_season.CurrentSeason, w.PlayerId);
                    if (team != null)
                    {
                        var teamId = await playerService.GetTeamId(team.Team);
                        var matchup = (await playerService.GetTeamGames(teamId)).FirstOrDefault(m => m.Week == w.Week);
                        if (matchup != null && matchup.OpposingTeam != "BYE")
                        {
                            var opponentRank = matchupRanks.FirstOrDefault(mr => mr.Team.TeamId == matchup.OpposingTeamId);
                            if (opponentRank != null)
                            {
                                var ratio = opponentRank.AvgPointsAllowed / avgMatchup.AvgPointsAllowed;
                                var tamperedRatio = ratio > 1 ? Math.Min(ratio, _weeklyTunings.TamperedMax) : Math.Max(ratio, _weeklyTunings.TamperedMin);
                                w.ProjectedPoints = w.Position != Position.DST.ToString() ? w.ProjectedPoints * (tamperedRatio + 1) / 2 : tamperedRatio * w.ProjectedPoints;
                            }
                        }
                        else
                            w.ProjectedPoints = 0;
                    }
                    else
                        w.ProjectedPoints = 0;
                }
            }
            return weekProjections;
        }
        private double EPARatio(double previousEPA, double currentEPA) => previousEPA == 0 ? 1
                   : previousEPA > currentEPA ? Math.Max(_tunings.NewQBFloor, currentEPA / previousEPA)
                   : Math.Min(_tunings.NewQBCeiling, currentEPA / previousEPA);               
    }
}
