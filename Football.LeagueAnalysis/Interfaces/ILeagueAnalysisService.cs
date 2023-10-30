using Football.LeagueAnalysis.Models;
using Football.Projections.Models;

namespace Football.LeagueAnalysis.Interfaces
{
    public interface ILeagueAnalysisService
    {
        public Task<int> UploadSleeperPlayerMap();
        public Task<SleeperPlayerMap?> GetSleeperPlayerMap(int sleeperId);
        public Task<List<WeekProjection>> GetSleeperLeagueProjections(string username);
    }
}
