using Football.Fantasy.Analysis.Models;

namespace Football.Fantasy.Analysis.Interfaces
{
    public interface IStartOrSitService
    {
        public Task<List<StartOrSit>> GetStartOrSits(List<int> playerIds);
        public Task<Weather> GetWeather(int playerId);
        public Task<MatchLines> GetMatchLines(int playerId);
    }
}
