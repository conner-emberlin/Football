using Football.Models;

namespace Football.Interfaces
{
    public interface IRegressionModelService
    {
        public RegressionModelQB RegressionModelQB(Player player, int season);
        public RegressionModelRB RegressionModelRB(Player player, int season);    
        public RegressionModelPassCatchers RegressionModelPC(Player player, int season);
        public Task<List<FantasyPoints>> PopulateFantasyResults(int season, string position);
        public RegressionModelQB RegressionModelQB(PassingStatistic passingStat, RushingStatistic rushingStat, int playerId);
        public RegressionModelRB RegressionModelRB(RushingStatistic rushingStat, ReceivingStatistic receivingStat, int playerId);
        public RegressionModelPassCatchers RegressionModelPC(ReceivingStatistic receivingStat, int playerId);
    }
}
