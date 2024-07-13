using Football.Models;

namespace Football
{
    public interface ISettingsRepository
    {
        public Task<bool> UploadSeasonTunings(Tunings tunings);
        public Task<Tunings> GetSeasonTunings(int season);
    }
}
