using Football.News.Models;

namespace Football.News.Interfaces {

    public interface INewsService
    {
        public Task<EspnNews> GetEspnNews();
        public Task<WeatherAPIRoot> GetWeatherAPI(string zip);
    }
}