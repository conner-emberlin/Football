using Football.Models;

namespace Football.Interfaces
{
    public interface IFantasyRepository
    {
        public Task<FantasyPassing> GetFantasyPassing(int playerId, int season);
        public Task<FantasyRushing>GetFantasyRushing(int playerId, int season);
        public Task<FantasyReceiving> GetFantasyReceiving(int playerId, int season);
        public Task<int> InsertFantasyPoints(FantasyPoints fantasyPoints);
        public Task<List<FantasyPoints>> GetAllFantasyResults(int playerId);
        public Task<FantasyPoints> GetFantasyResults(int playerId, int season);
        public Task<(int, int)> RefreshFantasyResults(FantasyPoints fantasyPoints);
        public Task<int> InsertFantasyProjections(int rank, ProjectionModel proj);

    }
}
