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
        public Task<QBModelWeek> QBModelWeek(WeeklyDataQB stat);
        public Task<RBModelWeek> RBModelWeek(WeeklyDataRB stat);
        public Task<WRModelWeek> WRModelWeek(WeeklyDataWR stat);
        public Task<TEModelWeek> TEModelWeek(WeeklyDataTE stat);
    }
}
