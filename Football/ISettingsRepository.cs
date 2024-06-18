using Football.Models;

namespace Football
{
    public interface ISettingsRepository
    {
        public Task<bool> UploadCurrentSeasonTunings(Tunings tunings);
    }
}
