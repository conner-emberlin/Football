using Football.Models;
using Football.Players.Models;

namespace Football.Projections.Interfaces
{
    public interface IStatProjectionCalculator
    {
        SeasonDataQB CalculateStatProjection(List<SeasonDataQB> seasons, double gamesPlayedInjured, Tunings tunings, int seasonGames);
        SeasonDataRB CalculateStatProjection(List<SeasonDataRB> seasons, double gamesPlayedInjured, Tunings tunings, int seasonGames);
        SeasonDataWR CalculateStatProjection(List<SeasonDataWR> seasons, double gamesPlayedInjured, Tunings tunings, int season);
        SeasonDataTE CalculateStatProjection(List<SeasonDataTE> seasons, double gamesPlayedInjured, Tunings tunings, int seasonGames);
        WeeklyDataQB WeightedWeeklyAverage(List<WeeklyDataQB> weeks, int currentWeek, WeeklyTunings weeklyTunings);
        WeeklyDataRB WeightedWeeklyAverage(List<WeeklyDataRB> weeks, int currentWeek, WeeklyTunings weeklyTunings);
        WeeklyDataWR WeightedWeeklyAverage(List<WeeklyDataWR> weeks, int currentWeek, WeeklyTunings weeklyTunings);
        WeeklyDataTE WeightedWeeklyAverage(List<WeeklyDataTE> weeks, int currentWeek, WeeklyTunings weeklyTunings);
        WeeklyDataDST WeightedWeeklyAverage(List<WeeklyDataDST> weeks, int currentWeek, WeeklyTunings weeklyTunings);
        WeeklyDataK WeightedWeeklyAverage(List<WeeklyDataK> weeks, int currentWeek, WeeklyTunings weeklyTunings);
        SnapCount WeightedWeeklyAverage(List<SnapCount> weeks, int currentWeek, WeeklyTunings weeklyTunings);
    }
}
