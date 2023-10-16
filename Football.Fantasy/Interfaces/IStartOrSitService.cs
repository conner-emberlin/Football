using Football.News.Models;
using Football.Fantasy.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IStartOrSitService
    {
        public Task<List<StartOrSit>> GetStartOrSits(List<int> playerIds);
        public Task<Weather> GetWeather(int playerId);
        public Task<MatchLines> GetMatchLines(int playerId);
    }
}
