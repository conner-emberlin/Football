using Football.Players.Models;

namespace Football.Players.Interfaces
{
    public interface ISleeperLeagueService
    {
        public Task<SleeperUser?> GetSleeperUser(string username);
        public Task<List<SleeperLeague>?> GetSleeperLeagues(string userId);
        public Task<SleeperLeague?> GetSleeperLeague(string leagueId);
        public Task<List<SleeperRoster>?> GetSleeperRosters(string leagueId);
        public Task<List<SleeperMatchup>?> GetSleeperMatchups(string leagueId, int week);
        public Task<List<SleeperPlayer>> GetSleeperPlayers();
        public Task<List<SleeperTrendingPlayer>?> GetSleeperTrendingPlayers();
    }
}
