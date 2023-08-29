using Football.Models;

namespace Football.Interfaces
{
    public interface IFantasyService
    {
        public Task<double> CalculateTotalPoints(int playerId, int season);
        public Task<double>  CalculatePassingPoints(int playerId, int season);
        public Task<double> CalculateRushingPoints(int playerId, int season);
        public Task<double> CalculateReceivingPoints(int playerId, int season);
        public Task<FantasyPoints> GetFantasyPoints(int playerId, int season);
        public Task<List<FantasyPoints>> GetAllFantasyResults(int playerId);
        public Task<FantasyPoints> GetFantasyResults(int playerId, int season);
        public Task<int> InsertFantasyPoints(FantasyPoints fantasyPoints);
        public Task<int> RefreshFantasyResults(FantasyPoints fantasyPoints);
        public Task<int> InsertFantasyProjections(int rank, ProjectionModel proj);
        public Task<List<FantasyPoints>> GetRookieFantasyResults(List<Rookie> rookies);
    }
}
