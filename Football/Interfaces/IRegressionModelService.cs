using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Football.Models;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Interfaces
{
    public interface IRegressionModelService
    {
        public RegressionModelQB PopulateRegressionModelQB(int playerId, int season);
        public RegressionModelRB PopulateRegressionModelRb(int playerId, int season);    
        public RegressionModelPassCatchers PopulateRegressionModelPassCatchers(int playerId, int season);
        public List<FantasyPoints> PopulateFantasyResults(int season, string position);
        public PassingStatistic GetPassingStatistic(int playerId, int season);
        public RushingStatistic GetRushingStatistic(int playerId, int season);
        public ReceivingStatistic GetReceivingStatistic(int playerId, int season);
    }
}
