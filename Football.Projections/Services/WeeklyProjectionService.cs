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
    public class WeeklyProjectionService(IFantasyDataService fantasyService, IMemoryCache cache, IMatrixCalculator matrixCalculator, IOptionsMonitor<Season> season, IOptionsMonitor<WeeklyTunings> tunings, IPlayersService playersService, 
    IWeeklyAdjustmentService adjustmentService, IProjectionRepository projectionRepository, IProjectionModelCalculator projectionModelCalculator, ISettingsService settingsService, IMapper mapper) : IProjectionService<WeekProjection>
    {
        private readonly Season _season = season.CurrentValue;
        private readonly WeeklyTunings _tunings = tunings.CurrentValue;

        public Task<IEnumerable<WeekProjection>?> GetPlayerProjections(int playerId) => projectionRepository.GetWeeklyProjection(playerId);
        public async Task<bool> DeleteProjection(WeekProjection projection) 
        { 
            var recordDeleted = await projectionRepository.DeleteWeeklyProjection(projection.PlayerId, projection.Week, projection.Season);
            if (recordDeleted) 
                cache.Remove(projection.Position + Cache.WeeklyProjections.ToString());
            return recordDeleted;
        } 
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
                Position.QB => await CalculateProjections(await projectionModelCalculator.QBWeeklyProjectionModel(), position, currentWeek, tunings, seasonGames, filter),
                Position.RB => await CalculateProjections(await projectionModelCalculator.RBWeeklyProjectionModel(), position, currentWeek, tunings, seasonGames, filter),
                Position.WR => await CalculateProjections(await projectionModelCalculator.WRWeeklyProjectionModel(), position, currentWeek, tunings, seasonGames, filter),
                Position.TE => await CalculateProjections(await projectionModelCalculator.TEWeeklyProjectionModel(), position, currentWeek, tunings, seasonGames, filter),
                Position.DST => await CalculateProjections(await projectionModelCalculator.DSTWeeklyProjectionModel(), position, currentWeek, tunings, seasonGames, filter),
                Position.K => await CalculateProjections(await projectionModelCalculator.KWeeklyProjectionModel(), position, currentWeek, tunings, seasonGames, filter),
                _ => throw new NotImplementedException()
            };

            if (projections.Any())
            {
                projections = await adjustmentService.AdjustmentEngine(projections.ToList(), tunings);
                var formattedProjections = projections.OrderByDescending(p => p.ProjectedPoints).Take(GetProjectionsCount(position, tunings));
                return formattedProjections;
            }
            return projections;
        }

        public async Task<Vector<double>> GetCoefficients(Position position)
        {
            var coefficients = position switch
            {
                Position.QB => await CalculateCoefficients(await projectionModelCalculator.QBWeeklyProjectionModel(), position),
                Position.RB => await CalculateCoefficients(await projectionModelCalculator.RBWeeklyProjectionModel(), position),
                Position.WR => await CalculateCoefficients(await projectionModelCalculator.WRWeeklyProjectionModel(), position),
                Position.TE => await CalculateCoefficients(await projectionModelCalculator.TEWeeklyProjectionModel(), position),
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
                var weightedVector = await projectionModelCalculator.GetWeeklyWeightedAverageVector(player, currentWeek, tunings, filter);
                var projectedPoints = weightedVector * coefficients;
                
                if (projectedPoints > 0 && projectedPoints != coefficients[0])
                {
                    var seasonProjection = seasonProjections.TryGetValue(player.PlayerId, out var proj) ? proj : await fantasyService.GetRecentSeasonFantasyTotal(player.PlayerId);
                    var projection = seasonProjection > 0 ? WeightedWeeklyProjection(seasonProjection / seasonGames, projectedPoints, currentWeek, tunings) : projectedPoints;

                    projections.Add(new WeekProjection
                    {
                        PlayerId = player.PlayerId,
                        Season = _season.CurrentSeason,
                        Week = currentWeek,
                        Name = player.Name,
                        Position = player.Position,
                        ProjectedPoints = projection
                    });
                }
            }

            return projections;
        }

        private async Task<Vector<double>> CalculateCoefficients<T>(List<T> model, Position position, List<string> filter = null)
        {
            var regressorMatrix = matrixCalculator.RegressorMatrix(model, filter);
            var fantasyModel = await projectionModelCalculator.WeeklyFantasyProjectionModel(position);
            var dependentVector = matrixCalculator.DependentVector(fantasyModel, Model.FantasyPoints);
            return MultipleRegression.NormalEquations(regressorMatrix, dependentVector);
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

        private int GetProjectionsCount(Position position, WeeklyTunings tunings)
        {
            return position switch
            {
                Position.QB => tunings.QBProjectionCount,
                Position.RB => tunings.RBProjectionCount,
                Position.WR => tunings.WRProjectionCount,
                Position.TE => tunings.TEProjectionCount,
                _ => 32
            };
        }
    }    
}
