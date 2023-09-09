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
        public RegressionService(IMatrixCalculator matrixCalculator, IFantasyDataService fantasyService)
        {
            _matrixCalculator = matrixCalculator;
            _fantasyService = fantasyService;
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
    }
}
