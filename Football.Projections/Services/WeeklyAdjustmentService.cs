using AutoMapper;
using Football.Enums;
using Football.Fantasy.Interfaces;
using Football.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using Microsoft.Extensions.Options;

namespace Football.Projections.Services
{
    public class WeeklyAdjustmentService(IPlayersService playerService, ITeamsService teamsService, IMatchupAnalysisService matchupAnalysisService, IOptionsMonitor<Season> season, IMapper mapper) : IWeeklyAdjustmentService
    {
        private readonly Season _season = season.CurrentValue;
        public async Task<List<WeekProjection>> AdjustmentEngine(List<WeekProjection> weekProjections, WeeklyTunings tunings)
        {
            if (weekProjections.Count == 0) return weekProjections;

            if (weekProjections.First().Week >= tunings.MinWeekMatchupAdjustment) weekProjections = await MatchupAdjustment(weekProjections, tunings);
            weekProjections = await InjuryAdustment(weekProjections);
            return weekProjections;
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

        private async Task<List<WeekProjection>> MatchupAdjustment(List<WeekProjection> weekProjections, WeeklyTunings tunings)
        {
            if (weekProjections.Count == 0) return weekProjections;

            _ = Enum.TryParse(weekProjections.First().Position, out Position position);
            var matchupRanks = await matchupAnalysisService.GetPositionalMatchupRankingsFromSQL(position, weekProjections.First().Season, weekProjections.First().Week);
            if (matchupRanks.Count > 0)
            {
                var avgMatchup = matchupRanks.ElementAt((int)Math.Floor((double)(matchupRanks.Count / 2)));
                var teamDictionary = position != Position.DST ? (await teamsService.GetPlayerTeams(_season.CurrentSeason, weekProjections.Select(w => w.PlayerId))).ToDictionary(p => p.PlayerId)
                                                    : (mapper.Map<List<PlayerTeam>>(await teamsService.GetAllTeams())).ToDictionary(p => p.PlayerId);
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
                                var ratio = avgMatchup.AvgPointsAllowed > 0 ? opponentRank.AvgPointsAllowed / avgMatchup.AvgPointsAllowed : 0;
                                var tamperedRatio = ratio > 1 ? Math.Min(ratio, tunings.TamperedMax) : Math.Max(ratio, tunings.TamperedMin);
                                w.ProjectedPoints = w.Position != Position.DST.ToString() ? w.ProjectedPoints * (tamperedRatio + 1) / 2 : tamperedRatio * w.ProjectedPoints;
                            }
                        }
                    }
                    else w.ProjectedPoints = 0;
                }
            }
            return weekProjections;
        }
    }
}
