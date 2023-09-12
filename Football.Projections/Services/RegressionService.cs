using Football.Data.Models;
using Football.Fantasy.Models;
using Football.Fantasy.Interfaces;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using Football.Players.Models;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearRegression;


namespace Football.Projections.Services
{
    public class RegressionService : IRegressionService
    {
        private readonly IMatrixCalculator _matrixCalculator;
        private readonly IFantasyDataService _fantasyService;
        private readonly IProjectionService _projectionService;
        public RegressionService(IMatrixCalculator matrixCalculator, IFantasyDataService fantasyService, IProjectionService projectionService)
        {
            _matrixCalculator = matrixCalculator;
            _fantasyService = fantasyService;
            _projectionService = projectionService;
        }

        public Vector<double> CholeskyDecomposition(Matrix<double> regressors, Vector<double> dependents)
        {
            var vec = MultipleRegression.NormalEquations(regressors, dependents);
            return vec;
        }
        public async Task<Vector<double>> PerformRegression(List<Rookie> rookies)
        {
            List<SeasonFantasy> rookieSeasons = new();
            foreach (var rookie in rookies)
            {
                rookieSeasons.Add(await _fantasyService.GetSeasonFantasy(rookie.PlayerId, rookie.RookieSeason));
            }

            return CholeskyDecomposition(_matrixCalculator.RegressorMatrix(rookies), _matrixCalculator.PopulateDependentVector(rookieSeasons));
        }

        public QBModelSeason QBModelSeason(SeasonDataQB stat)
        {
            return new QBModelSeason
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                PassingAttemptsPerGame = stat.Attempts / stat.Games,
                PassingYardsPerGame = stat.Yards / stat.Games,
                PassingTouchdownsPerGame = stat.TD / stat.Games,
                RushingAttemptsPerGame = stat.RushingAttempts / stat.Games,
                RushingYardsPerGame = stat.RushingYards / stat.Games,
                RushingTouchdownsPerGame = stat.RushingTD / stat.Games,
                SacksPerGame = stat.Sacks / stat.Games
            };
        }
        public async Task<QBModelWeek> QBModelWeek(WeeklyDataQB stat)
        {
            var projection = await _projectionService.GetSeasonProjection(stat.PlayerId);
            return new QBModelWeek
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Week = stat.Week,
                ProjectedPoints = projection != null ? projection.ProjectedPoints : 0,
                PassingAttemptsPerGame = stat.Attempts,
                PassingYardsPerGame = stat.Yards,
                PassingTouchdownsPerGame = stat.TD,
                RushingAttemptsPerGame= stat.RushingAttempts,
                RushingYardsPerGame = stat.RushingYards,
                RushingTouchdownsPerGame = stat.RushingTD,
                SacksPerGame = stat.Sacks
            };
        }
        public RBModelSeason RBModelSeason(SeasonDataRB stat)
        {
            return new RBModelSeason
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                RushingAttemptsPerGame = stat.RushingAtt / stat.Games,
                RushingYardsPerGame = stat.RushingYds / stat.Games,
                RushingYardsPerAttempt = stat.RushingAtt > 0 ? stat.RushingYds / stat.RushingAtt : 0,
                RushingTouchdownsPerGame = stat.RushingTD / stat.Games,
                ReceptionsPerGame = stat.Receptions / stat.Games,
                ReceivingYardsPerGame = stat.Yards / stat.Games,
                ReceivingTouchdownsPerGame = stat.ReceivingTD / stat.Games
            };
        }
        public async Task<RBModelWeek> RBModelWeek(WeeklyDataRB stat)
        {
            var projection = await _projectionService.GetSeasonProjection(stat.PlayerId);
            return new RBModelWeek
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Week = stat.Week,
                ProjectedPoints = projection != null ? projection.ProjectedPoints : 0,
                RushingAttemptsPerGame = stat.RushingAtt,
                RushingYardsPerGame = stat.RushingYds,
                RushingYardsPerAttempt = stat.RushingAtt > 0 ? stat.RushingYds / stat.RushingAtt : 0,
                RushingTouchdownsPerGame = stat.RushingTD,
                ReceptionsPerGame = stat.Receptions,
                ReceivingYardsPerGame = stat.Yards,
                ReceivingTouchdownsPerGame = stat.ReceivingTD
            };
        }
        public WRModelSeason WRModelSeason(SeasonDataWR stat)
        {
            return new WRModelSeason
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                TargetsPerGame = stat.Targets / stat.Games,
                ReceptionsPerGame = stat.Receptions / stat.Games,
                YardsPerGame = stat.Yards / stat.Games,
                YardsPerReception = stat.Receptions > 0 ? stat.Yards / stat.Receptions : 0,
                TouchdownsPerGame = stat.TD / stat.Games
            };
        }
        public async Task<WRModelWeek> WRModelWeek(WeeklyDataWR stat)
        {
            var projection = await _projectionService.GetSeasonProjection(stat.PlayerId);
            return new WRModelWeek
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Week = stat.Week,
                ProjectedPoints = projection != null ? projection.ProjectedPoints : 0,
                TargetsPerGame = stat.Targets,
                ReceptionsPerGame = stat.Receptions,
                YardsPerGame = stat.Yards,
                YardsPerReception = stat.Receptions > 0 ? stat.Yards/stat.Receptions : 0,
                TouchdownsPerGame = stat.TD
            };
        }
        public TEModelSeason TEModelSeason(SeasonDataTE stat)
        {
            return new TEModelSeason
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                TargetsPerGame = stat.Targets / stat.Games,
                ReceptionsPerGame = stat.Receptions / stat.Games,
                YardsPerGame = stat.Yards / stat.Games,
                YardsPerReception = stat.Receptions > 0 ? stat.Yards / stat.Receptions : 0,
                TouchdownsPerGame = stat.TD / stat.Games
            };
        }
        public async Task<TEModelWeek> TEModelWeek(WeeklyDataTE stat)
        {
            var projection = await _projectionService.GetSeasonProjection(stat.PlayerId);
            return new TEModelWeek
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Week = stat.Week,
                ProjectedPoints = projection != null ? projection.ProjectedPoints : 0,
                TargetsPerGame = stat.Targets,
                ReceptionsPerGame = stat.Receptions,
                YardsPerGame = stat.Yards,
                YardsPerReception = stat.Receptions > 0 ? stat.Yards / stat.Receptions : 0,
                TouchdownsPerGame = stat.TD
            };
        }
    }
}
