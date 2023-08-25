using Football.Models;

namespace Football.Interfaces
{
    public interface IAdjustmentRepository
    {
        public Task<int> GetGamesSuspended(int playerId, int season);
    }
}
