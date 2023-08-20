using Football.Models;

namespace Football.Interfaces
{
    public interface IRegressionModelService
    {
        public Task<RegressionModelQB> PopulateRegressionModelQB(int playerId, int season);
        public Task<RegressionModelRB> PopulateRegressionModelRb(int playerId, int season);    
        public Task<RegressionModelPassCatchers> PopulateRegressionModelPassCatchers(int playerId, int season);
        public Task<List<FantasyPoints>> PopulateFantasyResults(int season, string position);
        public RegressionModelQB PopulateProjectedAverageModelQB(PassingStatistic passingStat, RushingStatistic rushingStat, int playerId);
        public RegressionModelRB PopulateProjectedAverageModelRB(RushingStatistic rushingStat, ReceivingStatistic receivingStat, int playerId);
        public RegressionModelPassCatchers PopulateProjectedAverageModelPassCatchers(ReceivingStatistic receivingStat, int playerId);
    }
}
