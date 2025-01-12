using Football.Models;

namespace Football
{
    public interface ISettingsRepository
    {
        Task<bool> UploadSeasonTunings(Tunings tunings);
        Task<Tunings?> GetSeasonTunings(int season);
        Task<bool> UploadWeeklyTunings(WeeklyTunings tunings);
        Task<WeeklyTunings> GetWeeklyTunings(int season, int week);
        Task<bool> UploadSeasonAdjustments(SeasonAdjustments seasonAdjustments);
        Task<SeasonAdjustments> GetSeasonAdjustments(int season);
    }
}
