using Football.LeagueAnalysis.Models;

namespace Football.LeagueAnalysis.Interfaces
{
    public interface ISleeperLeagueService
    {
        public Task<SleeperUser?> GetSleeperUser(string username);
        public Task<List<SleeperLeague>?> GetSleeperLeagues(string userId);
        public Task<SleeperLeague?> GetSleeperLeague(string leagueId);
        public Task<List<SleeperRoster>?> GetSleeperRosters(string leagueId);
        public Task<List<SleeperPlayer>> GetSleeperPlayers();
        public Task<List<SleeperPlayerMap>> GetSleeperPlayerMap(List<SleeperPlayer> sleeperPlayers);
    }
}
