using Football.Fantasy.Models;
using Football.Data.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IFantasyCalculator
    {
        public SeasonFantasy CalculateFantasy(SeasonDataQB stat);
        public WeeklyFantasy CalculateFantasy(WeeklyDataQB stat);
        public SeasonFantasy CalculateFantasy(SeasonDataRB stat);
        public WeeklyFantasy CalculateFantasy(WeeklyDataRB stat);
        public SeasonFantasy CalculateFantasy(SeasonDataWR stat);
        public WeeklyFantasy CalculateFantasy(WeeklyDataWR stat);
        public SeasonFantasy CalculateFantasy(SeasonDataTE stat);
        public WeeklyFantasy CalculateFantasy(WeeklyDataTE stat);
        public WeeklyFantasy CalculateFantasy(WeeklyDataDST stat, WeeklyDataDST opponentStat, GameResult result, int teamId);
        public WeeklyFantasy CalculateFantasy(WeeklyDataK stat);
    }
}
