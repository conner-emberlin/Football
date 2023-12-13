using Football.Data.Models;
using Football.Fantasy.Models;

namespace Football.Projections.Interfaces
{
    public interface IStatProjectionCalculator
    {
        public SeasonDataQB CalculateStatProjection(List<SeasonDataQB> seasons);
        public SeasonDataRB CalculateStatProjection(List<SeasonDataRB> seasons);
        public SeasonDataWR CalculateStatProjection(List<SeasonDataWR> seasons);
        public SeasonDataTE CalculateStatProjection(List<SeasonDataTE> seasons);
        public SeasonFantasy CalculateStatProjection(List<SeasonFantasy> seasons);
        public WeeklyDataQB CalculateWeeklyAverage(List<WeeklyDataQB> weeks, int currentWeek);
        public WeeklyDataRB CalculateWeeklyAverage(List<WeeklyDataRB> weeks, int currentWeek);
        public WeeklyDataWR CalculateWeeklyAverage(List<WeeklyDataWR> weeks, int currentWeek);
        public WeeklyDataTE CalculateWeeklyAverage(List<WeeklyDataTE> weeks, int currentWeek);
        public WeeklyDataDST CalculateWeeklyAverage(List<WeeklyDataDST> weeks, int currentWeek);
        public WeeklyDataK CalculateWeeklyAverage(List<WeeklyDataK> weeks, int currentWeek);
        public WeeklyFantasy CalculateWeeklyAverage(List<WeeklyFantasy> weeks, int currentWeek);
        public SnapCount CalculateWeeklyAverage(List<SnapCount> snaps, int currentWeek);
    }
}
