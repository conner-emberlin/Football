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
        public Task<QBModelWeek> QBModelWeek(WeeklyDataQB stat, SnapCount snaps);
        public Task<RBModelWeek> RBModelWeek(WeeklyDataRB stat, SnapCount snaps);
        public Task<WRModelWeek> WRModelWeek(WeeklyDataWR stat, SnapCount snaps);
        public Task<TEModelWeek> TEModelWeek(WeeklyDataTE stat, SnapCount snaps);
        public Task<DSTModelWeek> DSTModelWeek(WeeklyDataDST stat);
        public Task<KModelWeek> KModelWeek(WeeklyDataK stat);
    }
}
