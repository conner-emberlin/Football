using Football.Fantasy.Models;
using Football.Data.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IFantasyCalculator
    {
        public SeasonFantasy CalculateQBFantasy(SeasonDataQB stat);
        public WeeklyFantasy CalculateQBFantasy(WeeklyDataQB stat);
        public SeasonFantasy CalculateRBFantasy(SeasonDataRB stat);
        public WeeklyFantasy CalculateRBFantasy(WeeklyDataRB stat);
        public SeasonFantasy CalculateWRFantasy(SeasonDataWR stat);
        public WeeklyFantasy CalculateWRFantasy(WeeklyDataWR stat);
        public SeasonFantasy CalculateTEFantasy(SeasonDataTE stat);
        public WeeklyFantasy CalculateTEFantasy(WeeklyDataTE stat);
        public SeasonFantasy CalculateDSTFantasy(SeasonDataDST stat);
    }
}
