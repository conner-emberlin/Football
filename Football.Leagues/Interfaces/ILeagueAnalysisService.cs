using Football.Leagues.Models;
using Football.Projections.Models;

namespace Football.Leagues.Interfaces
{
    public interface ILeagueAnalysisService
    {
        public Task<int> UploadSleeperPlayerMap();
        public Task<List<WeekProjection>> GetSleeperLeagueProjections(string username);
        public Task<List<MatchupProjections>> GetMatchupProjections(string username, int week);
        public Task<List<TrendingPlayer>> GetTrendingPlayers();
    }
}
