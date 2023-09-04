using Football.Fantasy.Models;
using Football.Data.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IFantasyCalculator
    {
        public SeasonFantasy CalculateQBFantasy(SeasonDataQB stat);
        public SeasonFantasy CalculateRBFantasy(SeasonDataRB stat);
        public SeasonFantasy CalculateWRFantasy(SeasonDataWR stat);
        public SeasonFantasy CalculateTEFantasy(SeasonDataTE stat);
        public SeasonFantasy CalculateDSTFantasy(SeasonDataDST stat);
    }
}
