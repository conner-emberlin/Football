using Football.News.Models;
using Microsoft.AspNetCore.Mvc;
using Football.News.Interfaces;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController(INewsService newsService) : ControllerBase
    {
        [HttpGet("espn")]
        [ProducesResponseType(typeof(EspnNews), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEspnNews() => Ok(await newsService.GetEspnNews());

        [HttpGet("weather/{zip}")]
        [ProducesResponseType(typeof(WeatherAPIRoot), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWeatherAPI(string zip) => Ok(await newsService.GetWeatherAPI(zip));

        [HttpGet("nfl-odds")]
        [ProducesResponseType(typeof(List<NFLOddsRoot>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNFLOdds() => Ok(await newsService.GetNFLOdds());


    }
}