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
    IStatisticsService statisticsService, IOptionsMonitor<Season> season, IOptionsMonitor<WeeklyTunings> tunings, IPlayersService playersService, IAdjustmentService adjustmentService, IProjectionRepository projectionRepository, ISettingsService settingsService, IMapper mapper) : IProjectionService<WeekProjection>
    {
        private readonly Season _season = season.CurrentValue;
        private readonly WeeklyTunings _tunings = tunings.CurrentValue;
        public async Task<bool> DeleteProjection(WeekProjection projection) 
        { 
            var recordDeleted = await projectionRepository.DeleteWeeklyProjection(projection.PlayerId, projection.Week, projection.Season);
            if (recordDeleted) 
                cache.Remove(projection.Position + Cache.WeeklyProjections.ToString());
            return recordDeleted;
        } 
        public async Task<IEnumerable<WeekProjection>?> GetPlayerProjections(int playerId) => await projectionRepository.GetWeeklyProjection(playerId);
        public async Task<int> PostProjections(List<WeekProjection> projections, List<string> filters) 
        {
            if (projections.Count == 0) return 0;

            var config = new WeeklyProjectionConfiguration
            {
                Season = projections.First().Season,
                Week = projections.First().Week,
                Position = projections.First().Position,
                DateCreated = DateTime.Now,
                Filter = string.Join(", ", [.. filters])
            };

            if (await projectionRepository.PostWeeklyProjectionConfiguration(config))
            {
                return await projectionRepository.PostWeeklyProjections(projections);
            }

            return 0;
        }

        public bool GetProjectionsFromSQL(Position position, int week, out IEnumerable<WeekProjection> projections)
        {
            projections = projectionRepository.GetWeeklyProjectionsFromSQL(position, week);
            return projections.Any();
        }
        public async Task<IEnumerable<WeekProjection>> GetProjections(Position position, List<string>? filter = null)
        {
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            var tunings = await settingsService.GetWeeklyTunings(_season.CurrentSeason, currentWeek);
            var seasonGames = await playersService.GetCurrentSeasonGames();

            if (currentWeek == 1) return await GetWeekOneProjections(position, tunings, seasonGames);

            var projections = position switch
            {
                Position.QB => await CalculateProjections(await QBProjectionModel(), position, currentWeek, tunings, seasonGames, filter),
                Position.RB => await CalculateProjections(await RBProjectionModel(), position, currentWeek, tunings, seasonGames, filter),
                Position.WR => await CalculateProjections(await WRProjectionModel(), position, currentWeek, tunings, seasonGames, filter),
                Position.TE => await CalculateProjections(await TEProjectionModel(), position, currentWeek, tunings, seasonGames, filter),
                Position.DST => await CalculateProjections(await DSTProjectionModel(), position, currentWeek, tunings, seasonGames, filter),
                Position.K => await CalculateProjections(await KProjectionModel(), position, currentWeek, tunings, seasonGames, filter),
                _ => throw new NotImplementedException()
            };

            if (projections.Any())
            {
                projections = await adjustmentService.AdjustmentEngine(projections.ToList(), tunings);
                var formattedProjections = projections.OrderByDescending(p => p.ProjectedPoints);
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

        public IEnumerable<string> GetModelVariablesByPosition(Position position)
        {
            return position switch
            {
                Position.QB => settingsService.GetPropertyNamesFromModel<QBModelWeek>(),
                Position.RB => settingsService.GetPropertyNamesFromModel<RBModelWeek>(),
                Position.WR => settingsService.GetPropertyNamesFromModel<WRModelWeek>(),
                Position.TE => settingsService.GetPropertyNamesFromModel<TEModelWeek>(),
                Position.DST => settingsService.GetPropertyNamesFromModel<DSTModelWeek>(),
                Position.K => settingsService.GetPropertyNamesFromModel<KModelWeek>(),
                _ => Enumerable.Empty<string>()
            };
        }
        public async Task<bool> PostProjectionConfiguration(Position position, string filter)
        {
            var week = await playersService.GetCurrentWeek(_season.CurrentSeason);
            var config = new WeeklyProjectionConfiguration
            {
                Season = _season.CurrentSeason,
                Week = week,
                Position = position.ToString(),
                DateCreated = DateTime.Now,
                Filter = filter
            };
            return await projectionRepository.PostWeeklyProjectionConfiguration(config);
        }
        public async Task<string?> GetCurrentProjectionConfigurationFilter(Position position) => await projectionRepository.GetCurrentWeekProjectionFilter(position.ToString(), await playersService.GetCurrentWeek(_season.CurrentSeason), _season.CurrentSeason);
        private async Task<IEnumerable<WeekProjection>> CalculateProjections<T>(List<T> model, Position position, int currentWeek, WeeklyTunings tunings, int seasonGames, List<string>? filter = null)
        {
            List<WeekProjection> projections = [];
            
            var coefficients = await CalculateCoefficients(model, position, filter);
            var players = await playersService.GetPlayersByPosition(position, activeOnly: true);
            var seasonProjections = await playersService.GetSeasonProjections(players.Select(p => p.PlayerId), _season.CurrentSeason);

            foreach (var player in players)
            {
                var weightedVector = await GetWeightedAverageVector(player, currentWeek, tunings, filter);
                var projectedPoints = weightedVector * coefficients;

                if (projectedPoints > 0)
                {
                    var seasonProjection = seasonProjections.TryGetValue(player.PlayerId, out var proj) ? proj : settingsService.GetAverageProjectionByPosition(position, await settingsService.GetSeasonTunings(_season.CurrentSeason));
                    var projection = seasonProjection > 0 ? WeightedWeeklyProjection(seasonProjection / seasonGames, projectedPoints, currentWeek - 1, tunings) : projectedPoints;
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

        private async Task<Vector<double>> CalculateCoefficients<T>(List<T> model, Position position, List<string> filter = null)
        {
            var regressorMatrix = matrixCalculator.RegressorMatrix(model, filter);
            var fantasyModel = await FantasyProjectionModel(position);
            var dependentVector = matrixCalculator.DependentVector(fantasyModel, Model.FantasyPoints);
            return MultipleRegression.NormalEquations(regressorMatrix, dependentVector);
        }

        private async Task<double> GetAverageProjectionError(int playerId)
        {
            var weeklyProjections = await GetPlayerProjections(playerId);
            var weeklyFantasy = await fantasyService.GetWeeklyFantasy(playerId);
            if (weeklyProjections != null && weeklyFantasy.Count > 0 && weeklyProjections.Count() > _tunings.ErrorAdjustmentWeek - 1)
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
        private async Task<List<DSTModelWeek>> DSTProjectionModel() => mapper.Map<List<DSTModelWeek>>(await statisticsService.GetAllWeeklyDataByPosition<AllWeeklyDataDST>(Position.DST));
        private async Task<Vector<double>> GetWeightedAverageVector(Player player, int currentWeek, WeeklyTunings tunings, List<string>? filter = null)
        {
            _ = Enum.TryParse(player.Position, out Position position);

            switch (position)
            {
                case Position.QB:
                    var modelQB = mapper.Map<QBModelWeek>(statCalculator.WeightedWeeklyAverage(await statisticsService.GetWeeklyData<WeeklyDataQB>(Position.QB, player.PlayerId), currentWeek, tunings));
                    modelQB.SnapsPerGame = (statCalculator.WeightedWeeklyAverage(await statisticsService.GetSnapCounts(player.PlayerId), currentWeek, tunings)).Snaps;
                    return matrixCalculator.TransformModel(modelQB, filter);
                case Position.RB:
                    var modelRB = mapper.Map<RBModelWeek>(statCalculator.WeightedWeeklyAverage(await statisticsService.GetWeeklyData<WeeklyDataRB>(Position.RB, player.PlayerId), currentWeek, tunings));
                    modelRB.SnapsPerGame = (statCalculator.WeightedWeeklyAverage(await statisticsService.GetSnapCounts(player.PlayerId), currentWeek, tunings)).Snaps;
                    return matrixCalculator.TransformModel(modelRB, filter);
                case Position.WR:
                    var modelWR = mapper.Map<WRModelWeek>(statCalculator.WeightedWeeklyAverage(await statisticsService.GetWeeklyData<WeeklyDataWR>(Position.WR, player.PlayerId), currentWeek, tunings));
                    modelWR.SnapsPerGame = (statCalculator.WeightedWeeklyAverage(await statisticsService.GetSnapCounts(player.PlayerId), currentWeek, tunings)).Snaps;
                    return matrixCalculator.TransformModel(modelWR, filter);
                case Position.TE:
                    var modelTE = mapper.Map<TEModelWeek>(statCalculator.WeightedWeeklyAverage(await statisticsService.GetWeeklyData<WeeklyDataTE>(Position.TE, player.PlayerId), currentWeek, tunings));
                    modelTE.SnapsPerGame = (statCalculator.WeightedWeeklyAverage(await statisticsService.GetSnapCounts(player.PlayerId), currentWeek, tunings)).Snaps;
                    return matrixCalculator.TransformModel(modelTE, filter);
                case Position.K:
                    var modelK = mapper.Map<KModelWeek>(statCalculator.WeightedWeeklyAverage(await statisticsService.GetWeeklyData<WeeklyDataK>(Position.K, player.PlayerId), currentWeek, tunings));
                    return matrixCalculator.TransformModel(modelK, filter);
                case Position.DST:
                    var modelDST = mapper.Map<DSTModelWeek>(statCalculator.WeightedWeeklyAverage(await statisticsService.GetWeeklyData<WeeklyDataDST>(Position.DST, player.PlayerId), currentWeek, tunings));
                    return matrixCalculator.TransformModel(modelDST, filter);
                default: return Vector<double>.Build.Dense(0);
            }
        }

        private double WeightedWeeklyProjection(double seasonProjection, double weeklyProjection, int week, WeeklyTunings tunings) 
        {
            if (seasonProjection > 0)
                return (tunings.ProjectionWeight / week) * seasonProjection + (1 - (tunings.ProjectionWeight / week)) * weeklyProjection;

            return weeklyProjection;
        } 

        private async Task<List<WeekProjection>> GetWeekOneProjections(Position position, WeeklyTunings tunings, int seasonGames)
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
                        Position = position.ToString(),
                        Season = _season.CurrentSeason,
                        Week = 1,
                        ProjectedPoints = seasonProjection / seasonGames
                    });
                }
            }
            return await adjustmentService.AdjustmentEngine(weekOneProjections, tunings);
        }
    }    
}
