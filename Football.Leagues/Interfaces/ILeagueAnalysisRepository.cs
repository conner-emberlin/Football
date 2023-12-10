using Football.Leagues.Models;

namespace Football.Leagues.Interfaces
{
    public interface ILeagueAnalysisRepository
    {
        public Task<int> UploadSleeperPlayerMap(List<SleeperPlayerMap> playerMap);
        public Task<SleeperPlayerMap?> GetSleeperPlayerMap(int sleeperId);
        public Task<List<SleeperPlayerMap>> GetSleeperPlayerMaps();
    }
}
