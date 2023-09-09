using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Football.Models;
using Football.Players.Interfaces;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Serilog;


namespace Football.Projections.Services
{
    public class ProjectionService : IProjectionService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;       
        private readonly ProjectionLimits _projections;
        private readonly Starters _starters;
        private readonly Season _season;
        private readonly IFantasyDataService _fantasyService;
        private readonly IStatisticsService _statisticsService;
        private readonly IMatrixCalculator _matrixCalculator;
        private readonly IStatProjectionCalculator _statCalculator;
        private readonly IRegressionService _regressionService;
        private readonly IAdjustmentService _adjustmentService;
        private readonly IPlayersService _playersService;

        public ProjectionService(IRegressionService regressionService, IFantasyDataService fantasyService, IOptionsMonitor<ProjectionLimits> projections, 
            IMemoryCache cache, ILogger logger, IMatrixCalculator matrixCalculator, IStatProjectionCalculator statCalculator,
            IStatisticsService statisticsService, IOptionsMonitor<Starters> starters, IOptionsMonitor<Season> season, 
            IPlayersService playersService, IAdjustmentService adjustmentService)
        {
            _regressionService = regressionService;
            _projections = projections.CurrentValue;
            _starters = starters.CurrentValue;
            _season = season.CurrentValue;
            _cache = cache;
            _logger = logger;
            _fantasyService = fantasyService;
            _matrixCalculator = matrixCalculator;
            _statCalculator = statCalculator;
            _statisticsService = statisticsService;
            _playersService = playersService;
            _adjustmentService = adjustmentService;
        }
        public async Task<List<SeasonProjection>> RookieSeasonProjections(string position)
        {
            List<SeasonProjection> rookieProjections = new();
            var historicalRookies = await _playersService.GetHistoricalRookies(_season.CurrentSeason, position);
            var coeff = await _regressionService.PerformRegression(historicalRookies);
            var currentRookies = await _playersService.GetCurrentRookies(_season.CurrentSeason, position);
            var model = _matrixCalculator.RegressorMatrix(currentRookies);
            var predictions = PerformProjection(model, coeff).ToList();

            for (int i = 0; i < predictions.Count; i++)
            {
                var rookie = currentRookies.ElementAt(i);
                rookieProjections.Add(new SeasonProjection
                {
                    PlayerId = rookie.PlayerId,
                    Name = (await _playersService.GetPlayer(rookie.PlayerId)).Name,
                    Season = _season.CurrentSeason,
                    Position = rookie.Position,
                    ProjectedPoints = predictions[i]
                });
            }
            return rookieProjections;
        }
        public async Task<List<SeasonFlex>> SeasonFlexRankings()
        {
            List<SeasonFlex> flexRankings = new();
            var qbProjections = (await GetSeasonProjections("QB")).ToList();
            var rbProjections = (await GetSeasonProjections("RB")).ToList();
            var wrProjections = (await GetSeasonProjections("WR")).ToList();
            var teProjections = (await GetSeasonProjections("TE")).ToList();
            try
            {
                var rankings = qbProjections.Concat(rbProjections).Concat(wrProjections).Concat(teProjections).ToList();
                foreach (var rank in rankings)
                {
                    flexRankings.Add(new SeasonFlex
                    {
                        PlayerId = rank.PlayerId,
                        Name = rank.Name,
                        Position = rank.Position,
                        ProjectedPoints = rank.ProjectedPoints,
                        Vorp = rank.ProjectedPoints - await GetReplacementPoints(rank.Position)
                    });
                }
                return flexRankings.OrderByDescending(f => f.Vorp).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString() + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }
        public async Task<IEnumerable<SeasonProjection>> GetSeasonProjections(string position)
        {
            if (RetrieveFromCache(position).Any())
            {
                return RetrieveFromCache(position);
            }
            else if (position == "QB")
            {
                var projections = (await CalculateSeasonProjections(await QBProjectionModel(), position)).ToList();
                projections = await _adjustmentService.AdjustmentEngine(projections);
                var formattedProjections = projections.OrderByDescending(p => p.ProjectedPoints).Take(_projections.QBProjections);
                _cache.Set("QBProjections", formattedProjections);
                return formattedProjections;
            }
            else if (position == "RB")
            {
                var rookieProjections = await RookieSeasonProjections(position);
                var projections = (await CalculateSeasonProjections(await RBProjectionModel(), position)).ToList();
                foreach(var r in rookieProjections)
                {
                    projections.Add(r);
                }
                projections = await _adjustmentService.AdjustmentEngine(projections);
                var formattedProjections = projections.OrderByDescending(p => p.ProjectedPoints).Take(_projections.RBProjections);
                _cache.Set("RBProjections", formattedProjections);
                return formattedProjections;
            }
            else if (position == "WR")
            {
                var projections = (await CalculateSeasonProjections(await WRProjectionModel(), position)).ToList();
                projections = await _adjustmentService.AdjustmentEngine(projections);
                var formattedProjections = projections.OrderByDescending(p => p.ProjectedPoints).Take(_projections.WRProjections);
                _cache.Set("WRProjections", formattedProjections);
                return formattedProjections;
            }
            else if (position == "TE")
            {
                var projections = (await CalculateSeasonProjections(await TEProjectionModel(), position)).ToList();
                projections = await _adjustmentService.AdjustmentEngine(projections);
                var formattedProjections = projections.OrderByDescending(p => p.ProjectedPoints).Take(_projections.TEProjections);
                _cache.Set("TEProjections", formattedProjections);
                return formattedProjections;
            }
            else
            {
                _logger.Error("Unable to retrieve projections");
                return Enumerable.Empty<SeasonProjection>();
            }
        }
            public async Task<IEnumerable<SeasonProjection>> CalculateSeasonProjections<T>(List<T> model, string position)
        {
            List<SeasonProjection> projections = new();
            var regressorMatrix = _matrixCalculator.RegressorMatrix(model);
            var results = PerformProjection(regressorMatrix, await PerformRegression(model, regressorMatrix, position));
            for (int i = 0; i < results.Count; i++)
            {
                var playerId = (int)typeof(T).GetProperties()[0].GetValue(model[i]);
                var player = await _playersService.GetPlayer(playerId);
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
            return projections;
        }
        public async Task<Vector<double>> PerformRegression<T>(List<T> regressionModel, Matrix<double> regressorMatrix, string position)
        {
            var dependentVector = _matrixCalculator.PopulateDependentVector(await SeasonFantasyProjectionModel(position));
            return _regressionService.CholeskyDecomposition(regressorMatrix, dependentVector);
        }
        public Vector<double> PerformProjection(Matrix<double> model, Vector<double> coeff)
        {
            return model * coeff;
        }

        private async Task<List<SeasonFantasy>> SeasonFantasyProjectionModel(string position)
        {
            var players = await _playersService.GetPlayersByPosition(position);
            List<SeasonFantasy> seasonFantasy = new();
            foreach(var player in players) 
            {
                var seasonFantasyResults = await _fantasyService.GetSeasonFantasy(player.PlayerId);
                if (seasonFantasyResults.Any())
                {
                    seasonFantasy.Add(_statCalculator.CalculateStatProjection(seasonFantasyResults));
                }                   
            }
            return seasonFantasy;
        }

        private async Task<List<QBModelSeason>> QBProjectionModel()
        {
            var players = await _playersService.GetPlayersByPosition("QB");
            List<QBModelSeason> qbModel = new();
            foreach(var player in players)
            {
                var stats = (await _statisticsService.GetSeasonDataQB(player.PlayerId));
                if (stats.Any())
                {
                    qbModel.Add(_regressionService.QBModelSeason(_statCalculator.CalculateStatProjection(stats)));
                }
            }
            return qbModel;
        }
        private async Task<List<RBModelSeason>> RBProjectionModel()
        {
            var players = await _playersService.GetPlayersByPosition("RB");
            List<RBModelSeason> rbModel = new();
            foreach (var player in players)
            {
                var stats = (await _statisticsService.GetSeasonDataRB(player.PlayerId));
                if (stats.Any())
                {
                    rbModel.Add(_regressionService.RBModelSeason(_statCalculator.CalculateStatProjection(stats)));
                }
            }
            return rbModel;
        }
        private async Task<List<WRModelSeason>> WRProjectionModel()
        {
            var players = await _playersService.GetPlayersByPosition("WR");
            List<WRModelSeason> wrModel = new();
            foreach (var player in players)
            {
                var stats = (await _statisticsService.GetSeasonDataWR(player.PlayerId));
                if (stats.Any())
                {
                    wrModel.Add(_regressionService.WRModelSeason(_statCalculator.CalculateStatProjection(stats)));
                }
            }
            return wrModel;
        }
        private async Task<List<TEModelSeason>> TEProjectionModel()
        {
            var players = await _playersService.GetPlayersByPosition("TE");
            List<TEModelSeason> teModel = new();
            foreach (var player in players)
            {
                var stats = (await _statisticsService.GetSeasonDataTE(player.PlayerId));
                if (stats.Any())
                {
                    teModel.Add(_regressionService.TEModelSeason(_statCalculator.CalculateStatProjection(stats)));
                }
            }
            return teModel;
        }
        private IEnumerable<SeasonProjection> RetrieveFromCache(string position)
        {
            return _cache.TryGetValue(position + "Projections", out IEnumerable<SeasonProjection> cachedProj) ? cachedProj
                : Enumerable.Empty<SeasonProjection>();
        }
        private async Task<double> GetReplacementPoints(string position)
        {
            return position switch
            {
                "QB" => (await GetSeasonProjections(position)).ElementAt(_starters.QBStarters - 1).ProjectedPoints,
                "RB" => (await GetSeasonProjections(position)).ElementAt(_starters.RBStarters - 1).ProjectedPoints,
                "WR" => (await GetSeasonProjections(position)).ElementAt(_starters.WRStarters - 1).ProjectedPoints,
                "TE" => (await GetSeasonProjections(position)).ElementAt(_starters.TEStarters - 1).ProjectedPoints,
                _ => 0,
            };
        }
    }
}
