using Football.Data.Models;
using Football.Projections.Models;
using Football.Players.Models;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Projections.Interfaces
{
    public interface IRegressionService
    {
        public Vector<double> CholeskyDecomposition(Matrix<double> regressors, Vector<double> dependents);
        public Task<Vector<double>> PerformRegression(List<Rookie> rookies);
        public QBModelSeason QBModelSeason(SeasonDataQB stat);
        public RBModelSeason RBModelSeason(SeasonDataRB stat);
        public WRModelSeason WRModelSeason(SeasonDataWR stat);
        public TEModelSeason TEModelSeason(SeasonDataTE stat);
        public Task<QBModelWeek> QBModelWeek(WeeklyDataQB stat);
        public Task<RBModelWeek> RBModelWeek(WeeklyDataRB stat);
        public Task<WRModelWeek> WRModelWeek(WeeklyDataWR stat);
        public Task<TEModelWeek> TEModelWeek(WeeklyDataTE stat);
    }
}
