using Football.Players.Models;

namespace Football.Players.Interfaces
{
    public interface ISleeperLeagueService
    {
        Task<SleeperUser?> GetSleeperUser(string username);
        Task<List<SleeperLeague>?> GetSleeperLeagues(string userId);
        Task<SleeperLeague?> GetSleeperLeague(string leagueId);
        Task<List<SleeperRoster>?> GetSleeperRosters(string leagueId);
        Task<List<SleeperMatchup>?> GetSleeperMatchups(string leagueId, int week);
        Task<List<SleeperPlayer>> GetSleeperPlayers();
        Task<List<SleeperTrendingPlayer>?> GetSleeperTrendingPlayers();
    }
}
