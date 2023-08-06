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
        public Task<RegressionModelQB> PopulateRegressionModelQB(int playerId, int season);
        public Task<RegressionModelRB> PopulateRegressionModelRb(int playerId, int season);    
        public Task<RegressionModelPassCatchers> PopulateRegressionModelPassCatchers(int playerId, int season);
        public Task<List<FantasyPoints>> PopulateFantasyResults(int season, string position);
        public Task<PassingStatistic> GetPassingStatistic(int playerId, int season);
        public Task<RushingStatistic> GetRushingStatistic(int playerId, int season);
        public Task<ReceivingStatistic> GetReceivingStatistic(int playerId, int season);
    }
}
