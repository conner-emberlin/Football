using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Microsoft.AspNetCore.Mvc;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FantasyController : ControllerBase
    {
        private readonly IFantasyDataService _fantasyDataService;
        public FantasyController(IFantasyDataService fantasyDataService)
        {
            _fantasyDataService = fantasyDataService;   
        }

        [HttpPost("data/{position}/{season}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> PostSeasonFantasy( int season, string position)
        {
            if (season > 0 && position != null)
            {
                return Ok(await _fantasyDataService.PostSeasonFantasy(season, position));
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }
        [HttpPost("data/{position}/{season}/{week}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> PostWeeklyFantasy(int season, int week, string position)
        {
            if (season > 0 && week > 0 && position != null)
            {
                return Ok(await _fantasyDataService.PostWeeklyFantasy(season, week, position));
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }


        [HttpGet("data/season/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonFantasy>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonFantasy>>> GetSeasonFantasy(int playerId)
        {
            if (playerId > 0)
            {
                return Ok(await _fantasyDataService.GetSeasonFantasy(playerId));
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }
        [HttpGet("data/weekly/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyFantasy>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyFantasy>>> GetWeeklyFantasy(int playerId)
        {
            if (playerId > 0)
            {
                return Ok(await _fantasyDataService.GetWeeklyFantasy(playerId));
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }
    }
}
