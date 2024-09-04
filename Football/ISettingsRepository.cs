using Football.Models;

namespace Football
{
    public interface ISettingsRepository
    {
        public Task<bool> UploadSeasonTunings(Tunings tunings);
        public Task<Tunings> GetSeasonTunings(int season);
        public Task<bool> UploadWeeklyTunings(WeeklyTunings tunings);
        public Task<WeeklyTunings> GetWeeklyTunings(int season, int week);
        public Task<bool> UploadSeasonAdjustments(SeasonAdjustments seasonAdjustments);
        public Task<SeasonAdjustments> GetSeasonAdjustments(int season);
    }
}
