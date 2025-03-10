﻿using AutoMapper;
using Football.Enums;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Football.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearRegression;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Serilog;
namespace Football.Projections.Services
{
    public class SeasonProjectionService(IFantasyDataService fantasyService,
    IMemoryCache cache, IMatrixCalculator matrixCalculator, IProjectionModelCalculator projectionModelCalculator, IOptionsMonitor<Season> season, IPlayersService playersService,
    IAdjustmentService adjustmentService, IProjectionRepository projectionRepository, ISettingsService settingsService, ILogger logger) : IProjectionService<SeasonProjection>
    {
        private readonly Season _season = season.CurrentValue;

        public Task<IEnumerable<SeasonProjection>?> GetPlayerProjections(int playerId) => projectionRepository.GetSeasonProjection(playerId);
        public Task<string?> GetCurrentProjectionConfigurationFilter(Position position) => projectionRepository.GetCurrentSeasonProjectionFilter(position.ToString(), _season.CurrentSeason);
        public Task<bool> DeleteProjection(SeasonProjection projection) => projectionRepository.DeleteSeasonProjection(projection.PlayerId, projection.Season);
        public async Task<int> PostProjections(List<SeasonProjection> projections, List<string> filters) 
        {
            if (projections.Count == 0) return 0;

            var config = new SeasonProjectionConfiguration
            {
                Season = projections.First().Season,
                Position = projections.First().Position,
                DateCreated = DateTime.Now,
                Filter = string.Join(", ", [.. filters])
            };

            if (await projectionRepository.PostSeasonProjectionConfiguration(config))
            {
                return await projectionRepository.PostSeasonProjections(projections);
            }

            return 0;            
        } 

        public bool GetProjectionsFromSQL(Position position, int season, out IEnumerable<SeasonProjection> projections)
        {
            projections =  projectionRepository.GetSeasonProjectionsFromSQL(position, season);
            return projections.Any();
        }
        public async Task<IEnumerable<SeasonProjection>> GetProjections(Position position, List<string>? filter = null)
        {
            var tunings = await settingsService.GetSeasonTunings(_season.CurrentSeason);
            var seasonAdjustments = await settingsService.GetSeasonAdjustments(_season.CurrentSeason);

            var seasonGames = await playersService.GetCurrentSeasonGames();

            var projections = position switch
            {
                Position.QB => await CalculateProjections(await projectionModelCalculator.QBSeasonProjectionModel(tunings.SeasonDataTrimmingGames), position, tunings, seasonGames, filter),
                Position.RB => await CalculateProjections(await projectionModelCalculator.RBSeasonProjectionModel(tunings.SeasonDataTrimmingGames), position, tunings, seasonGames, filter),
                Position.WR => await CalculateProjections(await projectionModelCalculator.WRSeasonProjectionModel(tunings.SeasonDataTrimmingGames), position, tunings, seasonGames, filter),
                Position.TE => await CalculateProjections(await projectionModelCalculator.TESeasonProjectionModel(tunings.SeasonDataTrimmingGames), position, tunings, seasonGames, filter),
                _ => throw new NotImplementedException()
            };
            var adjustedProjections = await adjustmentService.AdjustmentEngine((await RookieSeasonProjections(position, seasonGames)).Union(projections), tunings, seasonGames, seasonAdjustments);
            var formattedProjections = adjustedProjections.OrderByDescending(p => p.ProjectedPoints).Take(GetProjectionsCount(position, tunings));
            cache.Set(position.ToString() + Cache.SeasonProjections.ToString(), formattedProjections);
            return formattedProjections;
        }

        public async Task<Vector<double>> GetCoefficients(Position position)
        {
            var tunings = await settingsService.GetSeasonTunings(_season.CurrentSeason);
 
            var coefficients = position switch
            {
                Position.QB => await CalculateCoefficients(await projectionModelCalculator.QBSeasonProjectionModel(tunings.SeasonDataTrimmingGames), position, tunings.SeasonDataTrimmingGames),
                Position.RB => await CalculateCoefficients(await projectionModelCalculator.RBSeasonProjectionModel(tunings.SeasonDataTrimmingGames), position, tunings.SeasonDataTrimmingGames),
                Position.WR => await CalculateCoefficients(await projectionModelCalculator.WRSeasonProjectionModel(tunings.SeasonDataTrimmingGames), position, tunings.SeasonDataTrimmingGames),
                Position.TE => await CalculateCoefficients(await projectionModelCalculator.TESeasonProjectionModel(tunings.SeasonDataTrimmingGames), position, tunings.SeasonDataTrimmingGames),
                _ => throw new NotImplementedException()
            };

            return coefficients;
        }

        public IEnumerable<string> GetModelVariablesByPosition(Position position)
        {
            return position switch
            {
                Position.QB => settingsService.GetPropertyNamesFromModel<QBModelSeason>(),
                Position.RB => settingsService.GetPropertyNamesFromModel<RBModelSeason>(),
                Position.WR => settingsService.GetPropertyNamesFromModel<WRModelSeason>(),
                Position.TE => settingsService.GetPropertyNamesFromModel<TEModelSeason>(),
                _ => Enumerable.Empty<string>()
            };
        }

        public async Task<bool> PostProjectionConfiguration(Position position, string filter)
        {
            var config = new SeasonProjectionConfiguration
            {
                Season = _season.CurrentSeason,
                Position = position.ToString(),
                DateCreated = DateTime.Now,
                Filter = filter
            };
            return await projectionRepository.PostSeasonProjectionConfiguration(config);
        }


        private async Task<IEnumerable<SeasonProjection>> CalculateProjections<T1>(List<T1> model, Position position, Tunings tunings, int seasonGames, List<string>? filter = null)
        {
            List<SeasonProjection> projections = [];
            var coefficients = await CalculateCoefficients(model, position, tunings.SeasonDataTrimmingGames, filter);
            var players = await playersService.GetPlayersByPosition(position, activeOnly: true);
            var gamesInjuredDictionary = await playersService.GetGamesPlayedInjuredBySeason(_season.CurrentSeason - 1);

            foreach (var player in players)
            {
                var gamesPlayedInjured = gamesInjuredDictionary.TryGetValue(player.PlayerId, out var inj) ? inj : 0;
                var weightedVector = await projectionModelCalculator.GetSeasonWeightedAverageVector(player, gamesPlayedInjured, tunings, seasonGames, filter);
                var projectedPoints = weightedVector * coefficients;

                if (projectedPoints > 0)
                {
                    projections.Add(new SeasonProjection
                    {
                        PlayerId = player.PlayerId,
                        Season = _season.CurrentSeason,
                        Name = player.Name,
                        Position = player.Position,
                        ProjectedPoints = projectedPoints
                    });
                }
            }           
            return projections;
        }
        private async Task<Vector<double>> CalculateCoefficients<T>(List<T> model, Position position, int gameTrim, List<string>? filter = null)
        {
            var regressorMatrix = matrixCalculator.RegressorMatrix(model, filter);
            var fantasyModel = await projectionModelCalculator.SeasonFantasyProjectionModel(position, gameTrim);
            var dependentVector = matrixCalculator.DependentVector(fantasyModel, Model.FantasyPoints);
            return MultipleRegression.NormalEquations(regressorMatrix, dependentVector);
        }
        private async Task<List<SeasonProjection>> RookieSeasonProjections(Position position, int seasonGames)
        {
            List<SeasonProjection> rookieProjections = [];
            var historicalRookies = await playersService.GetHistoricalRookies(_season.CurrentSeason, position.ToString());
            if (historicalRookies.Count > 0)
            {
                var coeff = await HistoricalRookieRegression(historicalRookies, seasonGames);
                var currentRookies = await playersService.GetCurrentRookies(_season.CurrentSeason, position.ToString());

                foreach (var cr in currentRookies)
                {
                    var rookieModel = matrixCalculator.TransformModel(cr);
                    var projectedPoints = coeff * rookieModel;
                    rookieProjections.Add(new SeasonProjection
                    {
                        PlayerId = cr.PlayerId,
                        Name = (await playersService.GetPlayer(cr.PlayerId)).Name,
                        Season = _season.CurrentSeason,
                        Position = cr.Position,
                        ProjectedPoints = projectedPoints
                    });
                }
            }
            return rookieProjections;
        }

        private async Task<Vector<double>> HistoricalRookieRegression(List<Rookie> historicalRookies, int seasonGames)
        {
            List<SeasonFantasy> rookieSeasons = [];
            List<Rookie> rookies = [];
            foreach (var rookie in historicalRookies)
            {
                var rookieFantasy = (await fantasyService.GetSeasonFantasy(rookie.PlayerId)).FirstOrDefault(rf => rf.Season == rookie.RookieSeason);
                if (rookieFantasy != null)
                {
                    rookieFantasy.FantasyPoints = (rookieFantasy.FantasyPoints / rookieFantasy.Games) * seasonGames;
                    rookieSeasons.Add(rookieFantasy);
                    rookies.Add(rookie);
                }
            }
            return MultipleRegression.NormalEquations(matrixCalculator.RegressorMatrix(rookies), matrixCalculator.DependentVector(rookieSeasons, Model.FantasyPoints));
        }

        private int GetProjectionsCount(Position position, Tunings tunings)
        {
            return position switch
            {
                Position.QB => tunings.QBProjectionCount,
                Position.RB => tunings.RBProjectionCount,
                Position.WR => tunings.WRProjectionCount,
                Position.TE => tunings.TEProjectionCount,
                _ => 0
            };
        }
    }
}
