using Football.Models;
using Football.Services;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.Interfaces
{
    public interface IPredictionService {

        public Task<List<double>> ModelErrorPerSeason(int playerId, string position);
        public Task<PassingStatistic> CalculateAveragePassingStats(int playerId);
        public Task<RushingStatistic> CalculateAverageRushingStats(int playerId);
        public Task<ReceivingStatistic>  CalculateAverageReceivingStats(int playerId);
        public RegressionModelQB PopulateProjectedAverageModelQB(PassingStatistic passingStat, RushingStatistic rushingStat, int playerId);
        public RegressionModelRB PopulateProjectedAverageModelRB(RushingStatistic rushingStat, ReceivingStatistic receivingStat, int playerId);
        public RegressionModelPassCatchers PopulateProjectedAverageModelPassCatchers(ReceivingStatistic receivingStat, int playerId);
        public Task<FantasyPoints>CalculateProjectedAverageFantasyPoints(int playerId);
        public Task<List<FantasyPoints>> AverageProjectedFantasyByPosition(string position);
        public Task<List<RegressionModelQB>> AverageProjectedModelQB();
        public Task<List<RegressionModelRB>> AverageProjectedModelRB();
        public Task<List<RegressionModelPassCatchers>> AverageProjectedModelPassCatchers();
        public Vector<double> PerformPrediction(Matrix<double> model, Vector<double> coeff);
        public Task<IEnumerable<ProjectionModel>> GetProjections(string position);
    }
}
