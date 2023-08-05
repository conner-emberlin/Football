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

        public List<double> ModelErrorPerSeason(int playerId, string position);
        public PassingStatistic CalculateAveragePassingStats(int playerId);
        public RushingStatistic CalculateAverageRushingStats(int playerId);
        public ReceivingStatistic CalculateAverageReceivingStats(int playerId);
        public double ProjectStatToFullSeason(double averageGames, double averageStat);
        public RegressionModelQB PopulateProjectedAverageModelQB(PassingStatistic passingStat, RushingStatistic rushingStat, int playerId);
        public RegressionModelRB PopulateProjectedAverageModelRB(RushingStatistic rushingStat, ReceivingStatistic receivingStat, int playerId);
        public RegressionModelPassCatchers PopulateProjectedAverageModelPassCatchers(ReceivingStatistic receivingStat, int playerId);
        public FantasyPoints CalculateProjectedAverageFantasyPoints(int playerId);
        public List<FantasyPoints> AverageProjectedFantasyByPosition(string position);
        public List<RegressionModelQB> AverageProjectedModelQB();
        public List<RegressionModelRB> AverageProjectedModelRB();
        public List<RegressionModelPassCatchers> AverageProjectedModelPassCatchers();
        public Vector<double> PerformPrediction(Matrix<double> model, Vector<double> coeff);
        public IEnumerable<ProjectionModel> GetProjections(string position);
    }
}
