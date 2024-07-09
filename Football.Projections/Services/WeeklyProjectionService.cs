using Football.Enums;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Football.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using MathNet.Numerics.LinearRegression;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using AutoMapper;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Projections.Services
{
    public class WeeklyProjectionService(IFantasyDataService fantasyService, IMemoryCache cache,IMatrixCalculator matrixCalculator, IStatProjectionCalculator statCalculator,
    IStatisticsService statisticsService, IOptionsMonitor<Season> season, IPlayersService playersService, IAdjustmentService adjustmentService, IProjectionRepository projectionRepository,
    IOptionsMonitor<WeeklyTunings> weeklyTunings, ISettingsService settingsService, IOptionsMonitor<ProjectionLimits> settings, IMapper mapper, IMatchupAnalysisService matchupAnalysisService) : IProjectionService<WeekProjection>
    {
        private readonly Season _season = season.CurrentValue;
        private readonly WeeklyTunings _weeklyTunings = weeklyTunings.CurrentValue;
        private readonly ProjectionLimits _settings = settings.CurrentValue;

        public async Task<bool> DeleteProjection(WeekProjection projection) 
        { 
            var recordDeleted = await projectionRepository.DeleteWeeklyProjection(projection.PlayerId, projection.Week, projection.Season);
            if (recordDeleted) 
                cache.Remove(projection.Position + Cache.WeeklyProjections.ToString());
            return recordDeleted;
        } 
        public async Task<IEnumerable<WeekProjection>?> GetPlayerProjections(int playerId) => await projectionRepository.GetWeeklyProjection(playerId);
        public async Task<int> PostProjections(List<WeekProjection> projections) 
        {
            cache.Remove(projections.First().Position + Cache.WeeklyProjections.ToString());
            return await projectionRepository.PostWeeklyProjections(projections); 
        }

        public bool GetProjectionsFromSQL(Position position, int week, out IEnumerable<WeekProjection> projections)
        {
            projections = projectionRepository.GetWeeklyProjectionsFromSQL(position, week);
            return projections.Any();
        }
        public bool GetProjectionsFromCache(Position position, out IEnumerable<WeekProjection> cachedValues)
        {
            if (cache.TryGetValue(position.ToString() + Cache.WeeklyProjections.ToString(), out cachedValues!))
                return cachedValues.Any();
            return false;
        }
        public async Task<IEnumerable<WeekProjection>> GetProjections(Position position)
        {
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            if (currentWeek == 1) return await GetWeekOneProjections(position);

            var projections = position switch
            {
                Position.QB => await CalculateProjections(await QBProjectionModel(), position, currentWeek),
                Position.RB => await CalculateProjections(await RBProjectionModel(), position, currentWeek),
                Position.WR => await CalculateProjections(await WRProjectionModel(), position, currentWeek),
                Position.TE => await CalculateProjections(await TEProjectionModel(), position, currentWeek),
                Position.DST => await CalculateProjections(await DSTProjectionModel(), position, currentWeek),
                Position.K => await CalculateProjections(await KProjectionModel(), position, currentWeek),
                _ => throw new NotImplementedException()
            };

            if (projections.Any())
            {
                projections = await adjustmentService.AdjustmentEngine(projections.ToList());
                var formattedProjections = projections.OrderByDescending(p => p.ProjectedPoints).Take(settingsService.GetProjectionsCount(position));
                cache.Set(position.ToString() + Cache.WeeklyProjections.ToString(), formattedProjections);
                return formattedProjections;
            }
            return projections;
        }

        public async Task<Vector<double>> GetCoefficients(Position position)
        {
            var coefficients = position switch
            {
                Position.QB => await CalculateCoefficients(await QBProjectionModel(), position),
                Position.RB => await CalculateCoefficients(await RBProjectionModel(), position),
                Position.WR => await CalculateCoefficients(await WRProjectionModel(), position),
                Position.TE => await CalculateCoefficients(await TEProjectionModel(), position),
                _ => throw new NotImplementedException()
            };

            return coefficients;
        }

        private async Task<IEnumerable<WeekProjection>> CalculateProjections<T>(List<T> model, Position position, int currentWeek)
        {
            List<WeekProjection> projections = [];
            var coefficients = await CalculateCoefficients(model, position);
            var players = await playersService.GetPlayersByPosition(position, activeOnly: true);
            var seasonProjections = await playersService.GetSeasonProjections(players.Select(p => p.PlayerId), _season.CurrentSeason);
            var matchupRankings = await matchupAnalysisService.GetPositionalMatchupRankingsFromSQL(position, _season.CurrentSeason, currentWeek);
            var playerTeamsDictionary = (await playersService.GetPlayerTeams(_season.CurrentSeason, players.Select(p => p.PlayerId))).ToDictionary(p => p.PlayerId, p => p.TeamId);
            var currentWeekSchedule = await playersService.GetGames(_season.CurrentSeason, currentWeek);

            foreach (var player in players)
            {
                var weightedVector = await GetWeightedAverageVector(player, currentWeek, matchupRankings, playerTeamsDictionary, currentWeekSchedule);
                var projectedPoints = weightedVector * coefficients;

                if (projectedPoints > 0)
                {
                    var seasonProjection = seasonProjections.TryGetValue(player.PlayerId, out var proj) ? proj : 0;
                    var projection = seasonProjection > 0 ? WeightedWeeklyProjection(seasonProjection / _season.Games, projectedPoints, currentWeek - 1) : projectedPoints;
                    var avgError = await GetAverageProjectionError(player.PlayerId);

                    projections.Add(new WeekProjection
                    {
                        PlayerId = player.PlayerId,
                        Season = _season.CurrentSeason,
                        Week = currentWeek,
                        Name = player.Name,
                        Position = player.Position,
                        ProjectedPoints = (2 * projection - avgError) / 2
                    });
                }
            }

            return projections;
        }

        private async Task<Vector<double>> CalculateCoefficients<T>(List<T> model, Position position)
        {
            var regressorMatrix = matrixCalculator.RegressorMatrix(model);
            var fantasyModel = await FantasyProjectionModel(position);
            var dependentVector = matrixCalculator.DependentVector(fantasyModel, Model.FantasyPoints);
            return MultipleRegression.NormalEquations(regressorMatrix, dependentVector);
        }

        private async Task<double> GetAverageProjectionError(int playerId)
        {
            var weeklyProjections = await GetPlayerProjections(playerId);
            var weeklyFantasy = await fantasyService.GetWeeklyFantasy(playerId);
            if (weeklyProjections != null && weeklyFantasy.Count > 0 && weeklyProjections.Count() > _settings.ErrorAdjustmentWeek - 1)
            {
                var diffs = weeklyProjections.Join(weeklyFantasy, wp => wp.Week, wf => wf.Week, (wp, wf) => new { WeekProjection = wp, WeeklyFantasy = wf })
                                            .Select(r => r.WeekProjection.ProjectedPoints - r.WeeklyFantasy.FantasyPoints);
                return diffs.Any() ? diffs.Average() : 0;                                                          
            }
            return 0;
        }
        private async Task<List<WeeklyFantasy>> FantasyProjectionModel(Position position) => await fantasyService.GetAllWeeklyFantasyByPosition(position);
        private async Task<List<QBModelWeek>> QBProjectionModel() => mapper.Map<List<QBModelWeek>>(await statisticsService.GetAllWeeklyDataByPosition<AllWeeklyDataQB>(Position.QB));
        private async Task<List<RBModelWeek>> RBProjectionModel() => mapper.Map<List<RBModelWeek>>(await statisticsService.GetAllWeeklyDataByPosition<AllWeeklyDataRB>(Position.RB));
        private async Task<List<WRModelWeek>> WRProjectionModel() => mapper.Map<List<WRModelWeek>>(await statisticsService.GetAllWeeklyDataByPosition<AllWeeklyDataWR>(Position.WR));
        private async Task<List<TEModelWeek>> TEProjectionModel() => mapper.Map<List<TEModelWeek>>(await statisticsService.GetAllWeeklyDataByPosition<AllWeeklyDataTE>(Position.TE));
        private async Task<List<KModelWeek>> KProjectionModel() => mapper.Map<List<KModelWeek>>(await statisticsService.GetAllWeeklyDataByPosition<AllWeeklyDataK>(Position.K));
        private async Task<List<DSTModelWeek>> DSTProjectionModel() => mapper.Map<List<DSTModelWeek>>(await statisticsService.GetAllWeeklyDataByPosition<AllWeeklyDataDST>(Position.TE));
        private async Task<Vector<double>> GetWeightedAverageVector(Player player, int currentWeek, List<MatchupRanking> matchupRankings, Dictionary<int, int> playerTeamDictionary, List<Schedule> currentWeekSchedule)
        {
            _ = Enum.TryParse(player.Position, out Position position);

            switch (position)
            {
                case Position.QB:
                    var modelQB = mapper.Map<QBModelWeek>(statCalculator.WeightedWeeklyAverage(await statisticsService.GetWeeklyData<WeeklyDataQB>(Position.QB, player.PlayerId), currentWeek));
                    modelQB.SnapsPerGame = (statCalculator.WeightedWeeklyAverage(await statisticsService.GetSnapCounts(player.PlayerId), currentWeek)).Snaps;
                    modelQB.OppAvgPointsAllowed = GetOpponentAvgPointsAllowedToPosition(player.PlayerId, matchupRankings, playerTeamDictionary, currentWeekSchedule);
                    return matrixCalculator.TransformModel(modelQB);
                case Position.RB:
                    var modelRB = mapper.Map<RBModelWeek>(statCalculator.WeightedWeeklyAverage(await statisticsService.GetWeeklyData<WeeklyDataRB>(Position.RB, player.PlayerId), currentWeek));
                    modelRB.SnapsPerGame = (statCalculator.WeightedWeeklyAverage(await statisticsService.GetSnapCounts(player.PlayerId), currentWeek)).Snaps;
                    modelRB.OppAvgPointsAllowed = GetOpponentAvgPointsAllowedToPosition(player.PlayerId, matchupRankings, playerTeamDictionary, currentWeekSchedule);
                    return matrixCalculator.TransformModel(modelRB);
                case Position.WR:
                    var modelWR = mapper.Map<WRModelWeek>(statCalculator.WeightedWeeklyAverage(await statisticsService.GetWeeklyData<WeeklyDataWR>(Position.WR, player.PlayerId), currentWeek));
                    modelWR.SnapsPerGame = (statCalculator.WeightedWeeklyAverage(await statisticsService.GetSnapCounts(player.PlayerId), currentWeek)).Snaps;
                    modelWR.OppAvgPointsAllowed = GetOpponentAvgPointsAllowedToPosition(player.PlayerId, matchupRankings, playerTeamDictionary, currentWeekSchedule);
                    return matrixCalculator.TransformModel(modelWR);
                case Position.TE:
                    var modelTE = mapper.Map<TEModelWeek>(statCalculator.WeightedWeeklyAverage(await statisticsService.GetWeeklyData<WeeklyDataTE>(Position.TE, player.PlayerId), currentWeek));
                    modelTE.SnapsPerGame = (statCalculator.WeightedWeeklyAverage(await statisticsService.GetSnapCounts(player.PlayerId), currentWeek)).Snaps;
                    modelTE.OppAvgPointsAllowed = GetOpponentAvgPointsAllowedToPosition(player.PlayerId, matchupRankings, playerTeamDictionary, currentWeekSchedule);
                    return matrixCalculator.TransformModel(modelTE);
                case Position.K:
                    var modelK = mapper.Map<KModelWeek>(statCalculator.WeightedWeeklyAverage(await statisticsService.GetWeeklyData<WeeklyDataK>(Position.K, player.PlayerId), currentWeek));
                    return matrixCalculator.TransformModel(modelK);
                case Position.DST:
                    var modelDST = mapper.Map<DSTModelWeek>(statCalculator.WeightedWeeklyAverage(await statisticsService.GetWeeklyData<WeeklyDataDST>(Position.DST, player.PlayerId), currentWeek));
                    return matrixCalculator.TransformModel(modelDST);
                default: return Vector<double>.Build.Dense(0);
            }
        }

        private double WeightedWeeklyProjection(double seasonProjection, double weeklyProjection, int week) 
        {
            if (seasonProjection > 0)
                return (_weeklyTunings.ProjectionWeight / week) * seasonProjection + (1 - (_weeklyTunings.ProjectionWeight / week)) * weeklyProjection;

            return weeklyProjection;
        } 

        private async Task<List<WeekProjection>> GetWeekOneProjections(Position position)
        {
            List<WeekProjection> weekOneProjections = [];
            if (position == Position.K || position == Position.DST) return weekOneProjections;

            var players = await playersService.GetPlayersByPosition(position, true);
            var seasonProjectionDictionary = await playersService.GetSeasonProjections(players.Select(p => p.PlayerId), _season.CurrentSeason);
            foreach ( var player in players)
            {
                if (seasonProjectionDictionary.TryGetValue(player.PlayerId, out var seasonProjection))
                {
                    weekOneProjections.Add(new WeekProjection
                    {
                        PlayerId = player.PlayerId,
                        Name = player.Name,
                        Season = _season.CurrentSeason,
                        Week = 1,
                        ProjectedPoints = seasonProjection / _season.Games
                    });
                }
            }
            return await adjustmentService.AdjustmentEngine(weekOneProjections);
        }

        private double GetOpponentAvgPointsAllowedToPosition(int playerId, List<MatchupRanking> matchupRankings, Dictionary<int, int> playerTeamDictionary, List<Schedule> currentWeekGames)
        {
            if (!playerTeamDictionary.TryGetValue(playerId, out var teamId)) return 0;
            var opposingTeamId = currentWeekGames.First(c => c.TeamId == teamId).OpposingTeamId;
            if (opposingTeamId == 0) return 0;
            return matchupRankings.First(m => m.TeamId == opposingTeamId).AvgPointsAllowed;
        }

    }    
}
