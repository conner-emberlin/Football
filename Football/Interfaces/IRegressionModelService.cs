using Football.Models;

namespace Football.Interfaces
{
    public interface IRegressionModelService
    {
        public RegressionModelQB RegressionModelQB(Player player, int season);
        public RegressionModelRB RegressionModelRB(Player player, int season);    
        public RegressionModelPassCatchers RegressionModelPC(Player player, int season);
        public Task<List<FantasyPoints>> PopulateFantasyResults(int season, string position);
        public RegressionModelQB PopulateProjectedAverageModelQB(PassingStatistic passingStat, RushingStatistic rushingStat, int playerId);
        public RegressionModelRB PopulateProjectedAverageModelRB(RushingStatistic rushingStat, ReceivingStatistic receivingStat, int playerId);
        public RegressionModelPassCatchers PopulateProjectedAverageModelPassCatchers(ReceivingStatistic receivingStat, int playerId);
    }
}
