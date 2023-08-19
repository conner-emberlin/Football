using Football.Models;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Interfaces
{
    public interface IPredictionService {

        public RegressionModelQB PopulateProjectedAverageModelQB(PassingStatistic passingStat, RushingStatistic rushingStat, int playerId);
        public RegressionModelRB PopulateProjectedAverageModelRB(RushingStatistic rushingStat, ReceivingStatistic receivingStat, int playerId);
        public RegressionModelPassCatchers PopulateProjectedAverageModelPassCatchers(ReceivingStatistic receivingStat, int playerId);
        public Task<List<FantasyPoints>> AverageProjectedFantasyByPosition(string position);
        public Task<List<RegressionModelQB>> AverageProjectedModelQB();
        public Task<List<RegressionModelRB>> AverageProjectedModelRB();
        public Task<List<RegressionModelPassCatchers>> AverageProjectedModelPassCatchers();
        public Vector<double> PerformPrediction(Matrix<double> model, Vector<double> coeff);
        public Task<IEnumerable<ProjectionModel>> GetProjections(string position);
        public Task<int> InsertFantasyProjections(string position);
    }
}
