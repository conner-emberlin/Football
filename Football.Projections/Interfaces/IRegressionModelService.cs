using Football.Data.Models;
using Football.Projections.Models;

namespace Football.Projections.Interfaces
{
    public interface IRegressionModelService
    {
        public QBModelSeason QBModelSeason(SeasonDataQB stat);
        public RBModelSeason RBModelSeason(SeasonDataRB stat);
        public WRModelSeason WRModelSeason(SeasonDataWR stat);
        public TEModelSeason TEModelSeason(SeasonDataTE stat);
        public QBModelWeek QBModelWeek(WeeklyDataQB stat);
        public RBModelWeek RBModelWeek(WeeklyDataRB stat);
        public WRModelWeek WRModelWeek(WeeklyDataWR stat);
        public TEModelWeek TEModelWeek(WeeklyDataTE stat);
        public DSTModelWeek DSTModelWeek(WeeklyDataDST stat, double yardsAllowed);
        public KModelWeek KModelWeek(WeeklyDataK stat);
    }
}
