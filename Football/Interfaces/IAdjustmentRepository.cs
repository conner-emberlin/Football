using Football.Models;

namespace Football.Interfaces
{
    public interface IAdjustmentRepository
    {
        public Task<int> GetGamesSuspended(int playerId, int season);
        public Task<int> GetInjuryConcerns(int playerId, int season);
        public Task<List<Change?>> GetTeamChange(int season, string team);
        public Task<Change?> GetTeamChange(int season, int playerId);
    }
}
