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
            projections = await adjustmentService.AdjustmentEngine(projections.ToList());
            var withRookieProjections = (await RookieSeasonProjections(position)).Union(projections);
            var formattedProjections = withRookieProjections.OrderByDescending(p => p.ProjectedPoints).Take(settingsService.GetProjectionsCount(position));
            cache.Set(position.ToString() + Cache.SeasonProjections.ToString(), formattedProjections);
            return formattedProjections;
        }
        private async Task<IEnumerable<SeasonProjection>> CalculateProjections<T1>(List<T1> model, Position position)
        {
            List<SeasonProjection> projections = [];
            var regressorMatrix = matrixCalculator.RegressorMatrix(model);            
            var fantasyModel = await FantasyProjectionModel(position);
            var dependentVector = matrixCalculator.DependentVector(fantasyModel, Model.FantasyPoints);
            var coefficients = MultipleRegression.NormalEquations(regressorMatrix, dependentVector);

            var players = await playersService.GetPlayersByPosition(position, true);

            foreach (var player in players)
            {
                var weightedVector = await GetWeightedAverageVector(player);
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
        private async Task<List<SeasonFantasy>> FantasyProjectionModel(Position position) => await fantasyService.GetAllSeasonFantasyByPosition(position);

        private async Task<List<QBModelSeason>> QBProjectionModel() => mapper.Map<List<QBModelSeason>>(await statisticsService.GetAllSeasonDataByPosition<SeasonDataQB>(Position.QB));

        private async Task<List<RBModelSeason>> RBProjectionModel() => mapper.Map<List<RBModelSeason>>(await statisticsService.GetAllSeasonDataByPosition<SeasonDataRB>(Position.RB));

        private async Task<List<WRModelSeason>> WRProjectionModel() => mapper.Map<List<WRModelSeason>>(await statisticsService.GetAllSeasonDataByPosition<SeasonDataWR>(Position.WR));

        private async Task<List<TEModelSeason>> TEProjectionModel() => mapper.Map<List<TEModelSeason>>(await statisticsService.GetAllSeasonDataByPosition<SeasonDataTE>(Position.TE));
        private async Task<Vector<double>?> GetWeightedAverageVector(Player player)
        {
            _ = Enum.TryParse(player.Position, out Position position);

            return position switch
            {
                Position.QB => matrixCalculator.TransformModel(mapper.Map<QBModelSeason>(statCalculator.CalculateStatProjection(await statisticsService.GetSeasonData<SeasonDataQB>(position, player.PlayerId, true)))),
                Position.RB => matrixCalculator.TransformModel(mapper.Map<RBModelSeason>(statCalculator.CalculateStatProjection(await statisticsService.GetSeasonData<SeasonDataRB>(position, player.PlayerId, true)))),
                Position.WR => matrixCalculator.TransformModel(mapper.Map<WRModelSeason>(statCalculator.CalculateStatProjection(await statisticsService.GetSeasonData<SeasonDataWR>(position, player.PlayerId, true)))),
                Position.TE => matrixCalculator.TransformModel(mapper.Map<TEModelSeason>(statCalculator.CalculateStatProjection(await statisticsService.GetSeasonData<SeasonDataTE>(position, player.PlayerId, true)))),
                _ => Vector<double>.Build.Dense(0)
            }; 
        }

        private async Task<List<SeasonProjection>> RookieSeasonProjections(Position position)
        {
            List<SeasonProjection> rookieProjections = [];
            var historicalRookies = await playersService.GetHistoricalRookies(_season.CurrentSeason, position.ToString());
            if (historicalRookies.Count > 0)
            {
                var coeff = await HistoricalRookieRegression(historicalRookies);
                var currentRookies = await playersService.GetCurrentRookies(_season.CurrentSeason, position.ToString());
                var model = matrixCalculator.RegressorMatrix(currentRookies);
                var projections = (model * coeff).ToList();

                for (int i = 0; i < projections.Count; i++)
                {
                    var rookie = currentRookies.ElementAt(i);
                    rookieProjections.Add(new SeasonProjection
                    {
                        PlayerId = rookie.PlayerId,
                        Name = (await playersService.GetPlayer(rookie.PlayerId)).Name,
                        Season = _season.CurrentSeason,
                        Position = rookie.Position,
                        ProjectedPoints = projections[i]
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
