using Football.Data.Models;
using Football.Enums;
using Football.Fantasy.Interfaces;
using Football.Projections.Interfaces;
using Football.Statistics.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearRegression;


namespace Football.Projections.Services
{
    public class RegressionService(IFantasyDataService fantasyService, IStatisticsService statsService, IMatrixCalculator matrixCalculator)
    {
        public async Task<Vector<double>> CaclulateRegressionCoefficients(Position position)
        {
            var allWeeklyFantasy = (await fantasyService.GetWeeklyFantasy(position)).OrderBy(w => w.PlayerId).ThenBy(w => w.Week).ToList();
            var depVec = matrixCalculator.DependentVector(allWeeklyFantasy, Model.FantasyPoints);

            var regMatrix = position switch
            {
                Position.QB => matrixCalculator.RegressorMatrix((await statsService.GetWeeklyData<WeeklyDataQB>(Position.QB)).OrderBy(w => w.PlayerId).ThenBy(w => w.Week).ToList()),
                Position.RB => matrixCalculator.RegressorMatrix((await statsService.GetWeeklyData<WeeklyDataRB>(Position.RB)).OrderBy(w => w.PlayerId).ThenBy(w => w.Week).ToList()),
                Position.WR => matrixCalculator.RegressorMatrix((await statsService.GetWeeklyData<WeeklyDataWR>(Position.WR)).OrderBy(w => w.PlayerId).ThenBy(w => w.Week).ToList()),
                Position.TE => matrixCalculator.RegressorMatrix((await statsService.GetWeeklyData<WeeklyDataTE>(Position.TE)).OrderBy(w => w.PlayerId).ThenBy(w => w.Week).ToList()),
                Position.DST => matrixCalculator.RegressorMatrix((await statsService.GetWeeklyData<WeeklyDataDST>(Position.DST)).OrderBy(w => w.PlayerId).ThenBy(w => w.Week).ToList()),
                Position.K => matrixCalculator.RegressorMatrix((await statsService.GetWeeklyData<WeeklyDataK>(Position.K)).OrderBy(w => w.PlayerId).ThenBy(w => w.Week).ToList()),
                _ => throw new NotImplementedException()
            };
            return MultipleRegression.NormalEquations(regMatrix, depVec);
        }
    }
}
