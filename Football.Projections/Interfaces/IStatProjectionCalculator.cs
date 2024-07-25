using Football.Models;
using Football.Players.Models;

namespace Football.Projections.Interfaces
{
    public interface IStatProjectionCalculator
    {
        public SeasonDataQB CalculateStatProjection(List<SeasonDataQB> seasons, double gamesPlayedInjured, Tunings tunings, int seasonGames);
        public SeasonDataRB CalculateStatProjection(List<SeasonDataRB> seasons, double gamesPlayedInjured, Tunings tunings, int seasonGames);
        public SeasonDataWR CalculateStatProjection(List<SeasonDataWR> seasons, double gamesPlayedInjured, Tunings tunings, int season);
        public SeasonDataTE CalculateStatProjection(List<SeasonDataTE> seasons, double gamesPlayedInjured, Tunings tunings, int seasonGames);
        public WeeklyDataQB WeightedWeeklyAverage(List<WeeklyDataQB> weeks, int currentWeek, WeeklyTunings weeklyTunings);
        public WeeklyDataRB WeightedWeeklyAverage(List<WeeklyDataRB> weeks, int currentWeek, WeeklyTunings weeklyTunings);
        public WeeklyDataWR WeightedWeeklyAverage(List<WeeklyDataWR> weeks, int currentWeek, WeeklyTunings weeklyTunings);
        public WeeklyDataTE WeightedWeeklyAverage(List<WeeklyDataTE> weeks, int currentWeek, WeeklyTunings weeklyTunings);
        public WeeklyDataDST WeightedWeeklyAverage(List<WeeklyDataDST> weeks, int currentWeek, WeeklyTunings weeklyTunings);
        public WeeklyDataK WeightedWeeklyAverage(List<WeeklyDataK> weeks, int currentWeek, WeeklyTunings weeklyTunings);
        public SnapCount WeightedWeeklyAverage(List<SnapCount> weeks, int currentWeek, WeeklyTunings weeklyTunings);
    }
}
