using Football.Models;
using Football.Players.Models;

namespace Football.Projections.Interfaces
{
    public interface IStatProjectionCalculator
    {
        public SeasonDataQB CalculateStatProjection(List<SeasonDataQB> seasons, double gamesPlayedInjured, Tunings tunings);
        public SeasonDataRB CalculateStatProjection(List<SeasonDataRB> seasons, double gamesPlayedInjured, Tunings tunings);
        public SeasonDataWR CalculateStatProjection(List<SeasonDataWR> seasons, double gamesPlayedInjured, Tunings tunings);
        public SeasonDataTE CalculateStatProjection(List<SeasonDataTE> seasons, double gamesPlayedInjured, Tunings tunings);
        public WeeklyDataQB WeightedWeeklyAverage(List<WeeklyDataQB> weeks, int currentWeek);
        public WeeklyDataRB WeightedWeeklyAverage(List<WeeklyDataRB> weeks, int currentWeek);
        public WeeklyDataWR WeightedWeeklyAverage(List<WeeklyDataWR> weeks, int currentWeek);
        public WeeklyDataTE WeightedWeeklyAverage(List<WeeklyDataTE> weeks, int currentWeek);
        public WeeklyDataDST WeightedWeeklyAverage(List<WeeklyDataDST> weeks, int currentWeek);
        public WeeklyDataK WeightedWeeklyAverage(List<WeeklyDataK> weeks, int currentWeek);
        public SnapCount WeightedWeeklyAverage(List<SnapCount> weeks, int currentWeek);
    }
}
