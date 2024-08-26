using Football.Enums;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Football.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MathNet.Numerics.LinearRegression;
using MathNet.Numerics.LinearAlgebra;
using AutoMapper;

namespace Football.Projections.Services
{
    public class SeasonProjectionService(IFantasyDataService fantasyService,
    IMemoryCache cache, IMatrixCalculator matrixCalculator, IStatProjectionCalculator statCalculator,
    IStatisticsService statisticsService, IOptionsMonitor<Season> season, IPlayersService playersService,
    IAdjustmentService adjustmentService, IProjectionRepository projectionRepository, ISettingsService settingsService,
    IMapper mapper) : IProjectionService<SeasonProjection>
    {
        private readonly Season _season = season.CurrentValue;

        public async Task<bool> DeleteProjection(SeasonProjection projection)
        {
           var recordDeleted  = await projectionRepository.DeleteSeasonProjection(projection.PlayerId, projection.Season);
            if (recordDeleted)
                cache.Remove(projection.Position + Cache.SeasonProjections.ToString());
            return recordDeleted;
        }
        public async Task<IEnumerable<SeasonProjection>?> GetPlayerProjections(int playerId) => await projectionRepository.GetSeasonProjection(playerId);
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
            var seasonGames = await playersService.GetCurrentSeasonGames();

            var projections = position switch
            {
                Position.QB => await CalculateProjections(await QBProjectionModel(tunings.SeasonDataTrimmingGames), position, tunings, seasonGames, filter),
                Position.RB => await CalculateProjections(await RBProjectionModel(tunings.SeasonDataTrimmingGames), position, tunings, seasonGames, filter),
                Position.WR => await CalculateProjections(await WRProjectionModel(tunings.SeasonDataTrimmingGames), position, tunings, seasonGames, filter),
                Position.TE => await CalculateProjections(await TEProjectionModel(tunings.SeasonDataTrimmingGames), position, tunings, seasonGames, filter),
                _ => throw new NotImplementedException()
            };
            var adjustedProjections = await adjustmentService.AdjustmentEngine((await RookieSeasonProjections(position, seasonGames)).Union(projections), tunings, seasonGames);
            var formattedProjections = adjustedProjections.OrderByDescending(p => p.ProjectedPoints);
            cache.Set(position.ToString() + Cache.SeasonProjections.ToString(), formattedProjections);
            return formattedProjections;
        }

        public async Task<Vector<double>> GetCoefficients(Position position)
        {
            var tunings = await settingsService.GetSeasonTunings(_season.CurrentSeason);
 
            var coefficients = position switch
            {
                Position.QB => await CalculateCoefficients(await QBProjectionModel(tunings.SeasonDataTrimmingGames), position, tunings.SeasonDataTrimmingGames),
                Position.RB => await CalculateCoefficients(await RBProjectionModel(tunings.SeasonDataTrimmingGames), position, tunings.SeasonDataTrimmingGames),
                Position.WR => await CalculateCoefficients(await WRProjectionModel(tunings.SeasonDataTrimmingGames), position, tunings.SeasonDataTrimmingGames),
                Position.TE => await CalculateCoefficients(await TEProjectionModel(tunings.SeasonDataTrimmingGames), position, tunings.SeasonDataTrimmingGames),
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

        public async Task<string?> GetCurrentProjectionConfigurationFilter(Position position) => await projectionRepository.GetCurrentSeasonProjectionFilter(position.ToString(), _season.CurrentSeason);
        private async Task<IEnumerable<SeasonProjection>> CalculateProjections<T1>(List<T1> model, Position position, Tunings tunings, int seasonGames, List<string>? filter = null)
        {
            List<SeasonProjection> projections = [];
            var coefficients = await CalculateCoefficients(model, position, tunings.SeasonDataTrimmingGames, filter);
            var players = await playersService.GetPlayersByPosition(position, activeOnly: true);
            var gamesInjuredDictionary = await playersService.GetGamesPlayedInjuredBySeason(_season.CurrentSeason - 1);

            foreach (var player in players)
            {
                var gamesPlayedInjured = gamesInjuredDictionary.TryGetValue(player.PlayerId, out var inj) ? inj : 0;
                var weightedVector = await GetWeightedAverageVector(player, gamesPlayedInjured, tunings, seasonGames, filter);
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
            var fantasyModel = await FantasyProjectionModel(position, gameTrim);
            var dependentVector = matrixCalculator.DependentVector(fantasyModel, Model.FantasyPoints);
            return MultipleRegression.NormalEquations(regressorMatrix, dependentVector);
        }
        private async Task<List<SeasonFantasy>> FantasyProjectionModel(Position position, int gameTrim) 
        {
            var seasonFantasy = await fantasyService.GetAllSeasonFantasyByPosition(position, gameTrim);            
            return seasonFantasy;
        } 

        private async Task<List<QBModelSeason>> QBProjectionModel(int gameTrim) 
        {
            var seasonData = await statisticsService.GetAllSeasonDataByPosition<AllSeasonDataQB>(Position.QB);
            return mapper.Map<List<QBModelSeason>>(seasonData.Where(s => s.Games >= gameTrim));
        }

        private async Task<List<RBModelSeason>> RBProjectionModel(int gameTrim) 
        {
            var seasonData = await statisticsService.GetAllSeasonDataByPosition<AllSeasonDataRB>(Position.RB);
            return mapper.Map<List<RBModelSeason>>(seasonData.Where(s => s.Games >= gameTrim));
        }

        private async Task<List<WRModelSeason>> WRProjectionModel(int gameTrim) 
        {
            var seasonData = await statisticsService.GetAllSeasonDataByPosition<AllSeasonDataWR>(Position.WR);
            return mapper.Map<List<WRModelSeason>>(seasonData.Where(s => s.Games >= gameTrim));
        }

        private async Task<List<TEModelSeason>> TEProjectionModel(int gameTrim) 
        {
            var seasonData = await statisticsService.GetAllSeasonDataByPosition<AllSeasonDataTE>(Position.TE);
            return mapper.Map<List<TEModelSeason>>(seasonData.Where(s => s.Games >= gameTrim));
        } 
        private async Task<Vector<double>> GetWeightedAverageVector(Player player, double gamesPlayedInjured, Tunings tunings, int seasonGames, List<string>? filter = null)
        {
            _ = Enum.TryParse(player.Position, out Position position);

            switch (position)
            {
                case Position.QB:
                    var modelQB = mapper.Map<QBModelSeason>(statCalculator.CalculateStatProjection(await statisticsService.GetSeasonData<SeasonDataQB>(Position.QB, player.PlayerId, true), gamesPlayedInjured, tunings, seasonGames));
                    modelQB.YearsExperience = await statisticsService.GetYearsExperience(player.PlayerId, Position.QB);
                    return matrixCalculator.TransformModel(modelQB, filter);
                case Position.RB:
                    var modelRB = mapper.Map<RBModelSeason>(statCalculator.CalculateStatProjection(await statisticsService.GetSeasonData<SeasonDataRB>(Position.RB, player.PlayerId, true), gamesPlayedInjured, tunings, seasonGames));
                    modelRB.YearsExperience = await statisticsService.GetYearsExperience(player.PlayerId, Position.RB);
                    return matrixCalculator.TransformModel(modelRB, filter);
                case Position.WR:
                    var modelWR = mapper.Map<WRModelSeason>(statCalculator.CalculateStatProjection(await statisticsService.GetSeasonData<SeasonDataWR>(Position.WR, player.PlayerId, true), gamesPlayedInjured, tunings, seasonGames));
                    modelWR.YearsExperience = await statisticsService.GetYearsExperience(player.PlayerId, Position.WR);
                    return matrixCalculator.TransformModel(modelWR, filter);
                case Position.TE:
                    var modelTE = mapper.Map<TEModelSeason>(statCalculator.CalculateStatProjection(await statisticsService.GetSeasonData<SeasonDataTE>(Position.TE, player.PlayerId, true), gamesPlayedInjured, tunings, seasonGames));
                    modelTE.YearsExperience = await statisticsService.GetYearsExperience(player.PlayerId, Position.TE);
                    return matrixCalculator.TransformModel(modelTE, filter);
                default: return Vector<double>.Build.Dense(0);

            }
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
            foreach (var rookie in historicalRookies)
            {
                var rookieFantasy = (await fantasyService.GetSeasonFantasy(rookie.PlayerId)).First(rf => rf.Season == rookie.RookieSeason);
                rookieFantasy.FantasyPoints = (rookieFantasy.FantasyPoints / rookieFantasy.Games) * seasonGames;
                rookieSeasons.Add(rookieFantasy);
            }
            return MultipleRegression.NormalEquations(matrixCalculator.RegressorMatrix(historicalRookies), matrixCalculator.DependentVector(rookieSeasons, Model.FantasyPoints));
        }
    }
}
