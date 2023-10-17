using Football.Enums;
using Football.Projections.Models;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Projections.Interfaces
{
    public interface IProjectionService
    {
        public Task<List<SeasonFlex>> SeasonFlexRankings();
        public Task<IEnumerable<SeasonProjection>> GetSeasonProjections(PositionEnum position);
        public Task<IEnumerable<WeekProjection>> GetWeeklyProjections(PositionEnum position);
        public bool GetWeeklyProjectionsFromSQL(PositionEnum position, int week, out IEnumerable<WeekProjection> projections);
        public Task<IEnumerable<SeasonProjection>> CalculateSeasonProjections<T>(List<T> model, PositionEnum position);
        public Task<IEnumerable<WeekProjection>> CalculateWeeklyProjections(List<QBModelWeek> model);
        public Task<IEnumerable<WeekProjection>> CalculateWeeklyProjections(List<RBModelWeek> model);
        public Task<IEnumerable<WeekProjection>> CalculateWeeklyProjections(List<WRModelWeek> model);
        public Task<IEnumerable<WeekProjection>> CalculateWeeklyProjections(List<TEModelWeek> model);
        public Task<Vector<double>> PerformRegression(Matrix<double> regressorMatrix, PositionEnum position);
        public Task<Vector<double>> PerformWeeklyRegression(Matrix<double> regressorMatrix, PositionEnum position, int currentWeek);
        public Vector<double> PerformProjection(Matrix<double> model, Vector<double> coeff);
        public Task<List<SeasonProjection>> RookieSeasonProjections(PositionEnum position);
        public Task<int> PostSeasonProjections(List<SeasonProjection> projections);
        public Task<SeasonProjection?> GetSeasonProjection(int playerId);
        public Task<int> PostWeeklyProjections(List<WeekProjection> projections);
    }
}
