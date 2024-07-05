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
using MathNet.Numerics.LinearAlgebra.Double;

namespace Football.Projections.Services
{
    public class SeasonProjectionService(IFantasyDataService fantasyService,
    IMemoryCache cache, IMatrixCalculator matrixCalculator, IStatProjectionCalculator statCalculator,
    IStatisticsService statisticsService, IOptionsMonitor<Season> season, IOptionsMonitor<Tunings> tunings, IPlayersService playersService,
    IAdjustmentService adjustmentService, IProjectionRepository projectionRepository, ISettingsService settingsService,
    IMapper mapper) : IProjectionService<SeasonProjection>
    {
        private readonly Season _season = season.CurrentValue;
        private readonly Tunings _tunings = tunings.CurrentValue;

        public async Task<bool> DeleteProjection(SeasonProjection projection)
        {
           var recordDeleted  = await projectionRepository.DeleteSeasonProjection(projection.PlayerId, projection.Season);
            if (recordDeleted)
                cache.Remove(projection.Position + Cache.SeasonProjections.ToString());
            return recordDeleted;
        }
        public async Task<IEnumerable<SeasonProjection>?> GetPlayerProjections(int playerId) => await projectionRepository.GetSeasonProjection(playerId);
        public async Task<int> PostProjections(List<SeasonProjection> projections) 
        {
            cache.Remove(projections.First().Position + Cache.SeasonProjections.ToString());
            return await projectionRepository.PostSeasonProjections(projections);
        } 

        public bool GetProjectionsFromSQL(Position position, int season, out IEnumerable<SeasonProjection> projections)
        {
            projections =  projectionRepository.GetSeasonProjectionsFromSQL(position, season);
            return projections.Any();
        }
        public bool GetProjectionsFromCache(Position position, out IEnumerable<SeasonProjection> cachedValues)
        {
            if (cache.TryGetValue(position.ToString() + Cache.SeasonProjections.ToString(), out cachedValues!))
                return cachedValues.Any();
            return false;
        }
        
        public async Task<IEnumerable<SeasonProjection>> GetProjections(Position position)
        {

            var projections = position switch
            {
                Position.QB => await CalculateProjections(await QBProjectionModel(), position),
                Position.RB => await CalculateProjections(await RBProjectionModel(), position),
                Position.WR => await CalculateProjections(await WRProjectionModel(), position),
                Position.TE => await CalculateProjections(await TEProjectionModel(), position),
                _ => throw new NotImplementedException()
            };
            var adjustedProjections = await adjustmentService.AdjustmentEngine((await RookieSeasonProjections(position)).Union(projections));
            var formattedProjections = adjustedProjections.OrderByDescending(p => p.ProjectedPoints).Take(settingsService.GetProjectionsCount(position));
            cache.Set(position.ToString() + Cache.SeasonProjections.ToString(), formattedProjections);
            return formattedProjections;
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
        private async Task<IEnumerable<SeasonProjection>> CalculateProjections<T1>(List<T1> model, Position position)
        {
            List<SeasonProjection> projections = [];
            var coefficients = await CalculateCoefficients(model, position);
            var players = await playersService.GetPlayersByPosition(position, activeOnly: true);
            var gamesInjuredDictionary = await playersService.GetGamesPlayedInjuredBySeason(_season.CurrentSeason - 1);

            foreach (var player in players)
            {
                var gamesPlayedInjured = gamesInjuredDictionary.TryGetValue(player.PlayerId, out var inj) ? inj : 0;
                var weightedVector = await GetWeightedAverageVector(player, gamesPlayedInjured);
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

        private async Task<Vector<double>> CalculateCoefficients<T>(List<T> model, Position position)
        {
            var regressorMatrix = matrixCalculator.RegressorMatrix(model);
            var fantasyModel = await FantasyProjectionModel(position);
            var dependentVector = matrixCalculator.DependentVector(fantasyModel, Model.FantasyPoints);
            return MultipleRegression.NormalEquations(regressorMatrix, dependentVector);
        }
        private async Task<List<SeasonFantasy>> FantasyProjectionModel(Position position) 
        {
            var seasonFantasy = await fantasyService.GetAllSeasonFantasyByPosition(position, _tunings.SeasonDataTrimmingGames);            
            return seasonFantasy;
        } 

        private async Task<List<QBModelSeason>> QBProjectionModel() 
        {
            var seasonData = await statisticsService.GetAllSeasonDataByPosition<AllSeasonDataQB>(Position.QB);
            return mapper.Map<List<QBModelSeason>>(seasonData.Where(s => s.Games >= _tunings.SeasonDataTrimmingGames));
        }

        private async Task<List<RBModelSeason>> RBProjectionModel() 
        {
            var seasonData = await statisticsService.GetAllSeasonDataByPosition<AllSeasonDataRB>(Position.RB);
            return mapper.Map<List<RBModelSeason>>(seasonData.Where(s => s.Games >= _tunings.SeasonDataTrimmingGames));
        }

        private async Task<List<WRModelSeason>> WRProjectionModel() 
        {
            var seasonData = await statisticsService.GetAllSeasonDataByPosition<AllSeasonDataWR>(Position.WR);
            return mapper.Map<List<WRModelSeason>>(seasonData.Where(s => s.Games >= _tunings.SeasonDataTrimmingGames));
        }

        private async Task<List<TEModelSeason>> TEProjectionModel() 
        {
            var seasonData = await statisticsService.GetAllSeasonDataByPosition<AllSeasonDataTE>(Position.TE);
            return mapper.Map<List<TEModelSeason>>(seasonData.Where(s => s.Games >= _tunings.SeasonDataTrimmingGames));
        } 
        private async Task<Vector<double>> GetWeightedAverageVector(Player player, double gamesPlayedInjured)
        {
            _ = Enum.TryParse(player.Position, out Position position);

            switch (position)
            {
                case Position.QB:
                    var modelQB = mapper.Map<QBModelSeason>(statCalculator.CalculateStatProjection(await statisticsService.GetSeasonData<SeasonDataQB>(Position.QB, player.PlayerId, true), gamesPlayedInjured));
                    modelQB.YearsExperience = await statisticsService.GetYearsExperience(player.PlayerId, Position.QB);
                    return matrixCalculator.TransformModel(modelQB);
                case Position.RB:
                    var modelRB = mapper.Map<RBModelSeason>(statCalculator.CalculateStatProjection(await statisticsService.GetSeasonData<SeasonDataRB>(Position.RB, player.PlayerId, true), gamesPlayedInjured));
                    modelRB.YearsExperience = await statisticsService.GetYearsExperience(player.PlayerId, Position.RB);
                    return matrixCalculator.TransformModel(modelRB);
                case Position.WR:
                    var modelWR = mapper.Map<WRModelSeason>(statCalculator.CalculateStatProjection(await statisticsService.GetSeasonData<SeasonDataWR>(Position.WR, player.PlayerId, true), gamesPlayedInjured));
                    modelWR.YearsExperience = await statisticsService.GetYearsExperience(player.PlayerId, Position.WR);
                    return matrixCalculator.TransformModel(modelWR);
                case Position.TE:
                    var modelTE = mapper.Map<TEModelSeason>(statCalculator.CalculateStatProjection(await statisticsService.GetSeasonData<SeasonDataTE>(Position.TE, player.PlayerId, true), gamesPlayedInjured));
                    modelTE.YearsExperience = await statisticsService.GetYearsExperience(player.PlayerId, Position.TE);
                    return matrixCalculator.TransformModel(modelTE);
                default: return Vector<double>.Build.Dense(0);

            }
        }

        private async Task<List<SeasonProjection>> RookieSeasonProjections(Position position)
        {
            List<SeasonProjection> rookieProjections = [];
            var historicalRookies = await playersService.GetHistoricalRookies(_season.CurrentSeason, position.ToString());
            if (historicalRookies.Count > 0)
            {
                var coeff = await HistoricalRookieRegression(historicalRookies);
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

        private async Task<Vector<double>> HistoricalRookieRegression(List<Rookie> historicalRookies)
        {
            List<SeasonFantasy> rookieSeasons = [];
            foreach (var rookie in historicalRookies)
            {
                var rookieFantasy = (await fantasyService.GetSeasonFantasy(rookie.PlayerId)).First(rf => rf.Season == rookie.RookieSeason);
                rookieFantasy.FantasyPoints = (rookieFantasy.FantasyPoints / rookieFantasy.Games) * _season.Games;
                rookieSeasons.Add(rookieFantasy);
            }
            return MultipleRegression.NormalEquations(matrixCalculator.RegressorMatrix(historicalRookies), matrixCalculator.DependentVector(rookieSeasons, Model.FantasyPoints));
        }
    }
}
