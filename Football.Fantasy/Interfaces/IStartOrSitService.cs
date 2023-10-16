using Football.News.Models;
using Football.Fantasy.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IStartOrSitService
    {
        public Task<Hour> GetGamedayForecast(int playerId);
        public Task<MatchLines> GetMatchLines(int playerId);
    }
}
