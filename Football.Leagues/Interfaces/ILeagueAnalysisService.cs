using Football.Leagues.Models;
using Football.Projections.Models;

namespace Football.Leagues.Interfaces
{
    public interface ILeagueAnalysisService
    {
        public Task<int> UploadSleeperPlayerMap();
        public Task<List<WeekProjection>> GetSleeperLeagueProjections(string username);
        public Task<Dictionary<string, List<WeekProjection>>> GetMatchupProjections(string username, int week);
    }
}
