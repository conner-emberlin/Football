using Football.Data.Models;
using Football.Enums;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Football.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using Football.Statistics.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MathNet.Numerics.LinearRegression;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Projections.Services
{
    public class SeasonProjectionService(IRegressionModelService regressionService, IFantasyDataService fantasyService,
    IMemoryCache cache, IMatrixCalculator matrixCalculator, IStatProjectionCalculator statCalculator,
    IStatisticsService statisticsService, IOptionsMonitor<Season> season, IPlayersService playersService,
    IAdjustmentService adjustmentService, IProjectionRepository projectionRepository, ISettingsService settingsService) : IProjectionService<SeasonProjection>
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
        public async Task<int> PostProjections(List<SeasonProjection> projections) => await projectionRepository.PostSeasonProjections(projections);

        public bool GetProjectionsFromSQL(Position position, int season, out IEnumerable<SeasonProjection> projections)
        {
            projections =  projectionRepository.GetSeasonProjectionsFromSQL(position, season);
            return projections.Any();
        }
        public bool GetProjectionsFromCache(Position position, out IEnumerable<SeasonProjection> cachedValues)
        {
            if (cache.TryGetValue(position.ToString() + Cache.SeasonProjections.ToString(), out cachedValues!))
            {
                return cachedValues.Any();
            }
            else return false;
        }
        
        public async Task<IEnumerable<SeasonProjection>> GetProjections(Position position)
        {
            if (GetProjectionsFromCache(position, out var cachedProj))
            {
                return cachedProj;
            }
            else if(GetProjectionsFromSQL(position, _season.CurrentSeason, out var projectionsSQL))
            {
                cache.Set(position.ToString() + Cache.SeasonProjections.ToString(), projectionsSQL);
                return projectionsSQL;
            }
            else
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
        }
        public async Task<IEnumerable<SeasonProjection>> CalculateProjections<T1>(List<T1> model, Position position)
        {
            List<SeasonProjection> projections = [];
            var regressorMatrix = matrixCalculator.RegressorMatrix(model);            
            var fantasyModel = await FantasyProjectionModel(model);
            var dependentVector = matrixCalculator.DependentVector(fantasyModel, Model.FantasyPoints);
            var coefficients = MultipleRegression.NormalEquations(regressorMatrix, dependentVector);
            var results = regressorMatrix * coefficients;

            for (int i = 0; i < results.Count; i++)
            {
                var playerId = (int)settingsService.GetValueFromModel(model[i], Model.PlayerId);
                if (playerId > 0)
                {
                    var player = await playersService.GetPlayer(playerId);
                    if (player.Active == 1)
                    {
                        projections.Add(new SeasonProjection
                        {
                            PlayerId = playerId,
                            Season = _season.CurrentSeason,
                            Name = player.Name,
                            Position = player.Position,
                            ProjectedPoints = results[i]
                        });
                    }
                }
            }
            return projections;
        }
        private async Task<List<SeasonFantasy>> FantasyProjectionModel<T>(List<T> statsModel)
        {
            List<SeasonFantasy> seasonFantasy = [];
            foreach (var stat in statsModel)
            {
                var playerId = (int)settingsService.GetValueFromModel(stat, Model.PlayerId);
                if (playerId > 0)
                {
                    var seasonFantasyResults = await fantasyService.GetSeasonFantasy(playerId);
                    if (seasonFantasyResults.Count > 0)
                    {
                        var seasonAverage = statCalculator.CalculateStatProjection(seasonFantasyResults);
                        seasonFantasy.Add(seasonAverage);
                    }
                }
            }
            return seasonFantasy;
        }
        private async Task<List<QBModelSeason>> QBProjectionModel()
        {
            var players = await playersService.GetPlayersByPosition(Position.QB);
            List<QBModelSeason> qbModel = [];
            foreach (var player in players)
            {
                var stats = (await statisticsService.GetSeasonData<SeasonDataQB>(Position.QB, player.PlayerId, true));
                if (stats.Count > 0)
                {
                    qbModel.Add(regressionService.QBModelSeason(statCalculator.CalculateStatProjection(stats)));
                }
            }
            return qbModel;
        }
        private async Task<List<RBModelSeason>> RBProjectionModel()
        {
            var players = await playersService.GetPlayersByPosition(Position.RB);
            List<RBModelSeason> rbModel = [];
            foreach (var player in players)
            {
                var stats = (await statisticsService.GetSeasonData<SeasonDataRB>(Position.RB, player.PlayerId, true));
                if (stats.Count > 0)
                {
                    rbModel.Add(regressionService.RBModelSeason(statCalculator.CalculateStatProjection(stats)));
                }
            }
            return rbModel;
        }
        private async Task<List<WRModelSeason>> WRProjectionModel()
        {
            var players = await playersService.GetPlayersByPosition(Position.WR);
            List<WRModelSeason> wrModel = [];
            foreach (var player in players)
            {
                var stats = (await statisticsService.GetSeasonData<SeasonDataWR>(Position.WR, player.PlayerId, true));
                if (stats.Count > 0)
                {
                    wrModel.Add(regressionService.WRModelSeason(statCalculator.CalculateStatProjection(stats)));
                }
            }
            return wrModel;
        }
        private async Task<List<TEModelSeason>> TEProjectionModel()
        {
            var players = await playersService.GetPlayersByPosition(Position.TE);
            List<TEModelSeason> teModel = [];
            foreach (var player in players)
            {
                var stats = (await statisticsService.GetSeasonData<SeasonDataTE>(Position.TE, player.PlayerId, true));
                if (stats.Count > 0)
                {
                    teModel.Add(regressionService.TEModelSeason(statCalculator.CalculateStatProjection(stats)));
                }
            }
            return teModel;
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
                var predictions = (model * coeff).ToList();

                for (int i = 0; i < predictions.Count; i++)
                {
                    var rookie = currentRookies.ElementAt(i);
                    rookieProjections.Add(new SeasonProjection
                    {
                        PlayerId = rookie.PlayerId,
                        Name = (await playersService.GetPlayer(rookie.PlayerId)).Name,
                        Season = _season.CurrentSeason,
                        Position = rookie.Position,
                        ProjectedPoints = predictions[i]
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
                var rookieFantasy = await fantasyService.GetSeasonFantasy(rookie.PlayerId);
                rookieSeasons.Add(rookieFantasy.First(rf => rf.Season == rookie.RookieSeason));
            }
            return MultipleRegression.NormalEquations(matrixCalculator.RegressorMatrix(historicalRookies), matrixCalculator.DependentVector(rookieSeasons, Model.FantasyPoints));
        }
    }
}
