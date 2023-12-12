using Football.Data.Models;
using Football.Enums;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Football.Players.Interfaces;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using Football.Statistics.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearRegression;


namespace Football.Projections.Services
{
    public class RegressionService(IFantasyDataService fantasyService, IStatisticsService statsService, IMatrixCalculator matrixCalculator, 
        IRegressionModelService modelService, IPlayersService playersService, IAdvancedStatisticsService advancedStatisticsService)
    {
        public async Task<List<double>> CaclulateRegressionCoefficients(Position position)
        {
            var allWeeklyFantasy = (await fantasyService.GetWeeklyFantasy(position)).OrderBy(w => w.PlayerId).ThenBy(w => w.Week).ToList();
            var depVec = matrixCalculator.DependentVector(allWeeklyFantasy, Model.FantasyPoints);

            var regessorMatrix = position switch
            {
                Position.QB => await GetRegressorMatrixQB(allWeeklyFantasy),
                Position.RB => await GetRegressorMatrixRB(allWeeklyFantasy),
                Position.WR => await GetRegressorMatrixWR(allWeeklyFantasy),
                Position.TE => await GetRegressorMatrixTE(allWeeklyFantasy),
                Position.DST => await GetRegressorMatrixDST(allWeeklyFantasy),
                Position.K => await GetRegressorMatrixK(allWeeklyFantasy),
                _ => throw new NotImplementedException()
            };
            return MultipleRegression.NormalEquations(regessorMatrix, depVec).ToList();
        }
        private async Task<Matrix<double>> GetRegressorMatrixQB(List<WeeklyFantasy> fantasy)
        {
            List<QBModelWeek> model = [];
            foreach (var f in fantasy)
                model.Add(modelService.QBModelWeek((await statsService.GetWeeklyData<WeeklyDataQB>(Position.QB, f.PlayerId)).First(s => s.Week == f.Week)));
            return matrixCalculator.RegressorMatrix(model.OrderBy(s => s.PlayerId).ThenBy(s => s.Week).ToList());
        }
        private async Task<Matrix<double>> GetRegressorMatrixRB(List<WeeklyFantasy> fantasy)
        {
            List<RBModelWeek> model = [];
            foreach (var f in fantasy)
                model.Add(modelService.RBModelWeek((await statsService.GetWeeklyData<WeeklyDataRB>(Position.RB, f.PlayerId)).First(s => s.Week == f.Week)));
            return matrixCalculator.RegressorMatrix(model.OrderBy(s => s.PlayerId).ThenBy(s => s.Week).ToList());
        }

        private async Task<Matrix<double>> GetRegressorMatrixWR(List<WeeklyFantasy> fantasy)
        {
            List<WRModelWeek> model = [];
            foreach (var f in fantasy)
                model.Add(modelService.WRModelWeek((await statsService.GetWeeklyData<WeeklyDataWR>(Position.WR, f.PlayerId)).First(s => s.Week == f.Week)));
            return matrixCalculator.RegressorMatrix(model.OrderBy(s => s.PlayerId).ThenBy(s => s.Week).ToList());
        }
        private async Task<Matrix<double>> GetRegressorMatrixTE(List<WeeklyFantasy> fantasy)
        {
            List<TEModelWeek> model = [];
            foreach (var f in fantasy)
                model.Add(modelService.TEModelWeek((await statsService.GetWeeklyData<WeeklyDataTE>(Position.TE, f.PlayerId)).First(s => s.Week == f.Week)));
            return matrixCalculator.RegressorMatrix(model.OrderBy(s => s.PlayerId).ThenBy(s => s.Week).ToList());
        }
        private async Task<Matrix<double>> GetRegressorMatrixDST(List<WeeklyFantasy> fantasy)
        {
            List<DSTModelWeek> model = [];
            foreach (var f in fantasy)
            {
                var yardsAllowed = await advancedStatisticsService.YardsAllowed(await playersService.GetTeamId(f.PlayerId), f.Week);
                model.Add(modelService.DSTModelWeek((await statsService.GetWeeklyData<WeeklyDataDST>(Position.DST, f.PlayerId)).First(s => s.Week == f.Week), yardsAllowed));
            }                
            return matrixCalculator.RegressorMatrix(model.OrderBy(s => s.PlayerId).ThenBy(s => s.Week).ToList());
        }
        private async Task<Matrix<double>> GetRegressorMatrixK(List<WeeklyFantasy> fantasy)
        {
            List<KModelWeek> model = [];
            foreach (var f in fantasy)
                model.Add(modelService.KModelWeek((await statsService.GetWeeklyData<WeeklyDataK>(Position.K, f.PlayerId)).First(s => s.Week == f.Week)));
            return matrixCalculator.RegressorMatrix(model.OrderBy(s => s.PlayerId).ThenBy(s => s.Week).ToList());
        }
    }
}
