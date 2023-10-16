using Football.News.Models;
using Microsoft.AspNetCore.Mvc;
using Football.News.Interfaces;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet("espn")]
        [ProducesResponseType(typeof(EspnNews), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<EspnNews>> GetEspnNews() => Ok(await _newsService.GetEspnNews());

        [HttpGet("weather/{zip}")]
        [ProducesResponseType(typeof(WeatherAPIRoot), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<WeatherAPIRoot>> GetWeatherAPI(string zip) => Ok(await _newsService.GetWeatherAPI(zip));

        [HttpGet("nfl-odds")]
        [ProducesResponseType(typeof(List<NFLOddsRoot>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<NFLOddsRoot>>> GetNFLOdds() => Ok(await _newsService.GetNFLOdds());


    }
}