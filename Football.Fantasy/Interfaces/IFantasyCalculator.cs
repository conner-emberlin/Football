using Football.Fantasy.Models;
using Football.Players.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IFantasyCalculator
    {
        SeasonFantasy CalculateFantasy(SeasonDataQB stat);
        WeeklyFantasy CalculateFantasy(WeeklyDataQB stat);
        SeasonFantasy CalculateFantasy(SeasonDataRB stat);
        WeeklyFantasy CalculateFantasy(WeeklyDataRB stat);
        SeasonFantasy CalculateFantasy(SeasonDataWR stat);
        WeeklyFantasy CalculateFantasy(WeeklyDataWR stat);
        SeasonFantasy CalculateFantasy(SeasonDataTE stat);
        WeeklyFantasy CalculateFantasy(WeeklyDataTE stat);
        WeeklyFantasy CalculateFantasy(WeeklyDataDST stat, WeeklyDataDST opponentStat, GameResult result, int teamId);
        WeeklyFantasy CalculateFantasy(WeeklyDataK stat);
    }
}
