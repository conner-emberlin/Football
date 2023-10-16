using Football.News.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IStartOrSitService
    {
        public Task<Hour> GetGamedayForecast(int playerId);
    }
}
