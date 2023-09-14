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
        public WeeklyDataQB CalculateWeeklyAverage(List<WeeklyDataQB> weeks);
        public WeeklyDataRB CalculateWeeklyAverage(List<WeeklyDataRB> weeks);
        public WeeklyDataWR CalculateWeeklyAverage(List<WeeklyDataWR> weeks);
        public WeeklyDataTE CalculateWeeklyAverage(List<WeeklyDataTE> weeks);
        public WeeklyFantasy CalculateWeeklyAverage(List<WeeklyFantasy> weeks);
    }
}
