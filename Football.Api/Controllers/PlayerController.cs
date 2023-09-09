using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Football.Data.Models;
using Football.Fantasy.Interfaces;
using Football.Players.Interfaces;
using Football.Players.Models;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IPlayersService _playersService;
        public PlayerController(IPlayersService playersService, IStatisticsService statisticsService)
        {
            _playersService = playersService;
            _statisticsService = statisticsService;
        }
        [HttpGet("data/players")]
        [ProducesResponseType(typeof(List<Player>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<Player>>> GetAllPlayers()
        {
            return Ok(await _playersService.GetAllPlayers());
        }

        [HttpGet("data/qb/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataQB>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonDataQB>>> GetSeasonDataQB(int playerId)
        {
            if (playerId > 0)
            {
                return Ok(await _statisticsService.GetSeasonDataQB(playerId));
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }
        [HttpGet("data/rb/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataRB>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonDataRB>>> GetSeasonDataRB(int playerId)
        {
            if (playerId > 0)
            {
                return Ok(await _statisticsService.GetSeasonDataRB(playerId));
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }
        [HttpGet("data/wr/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataWR>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonDataWR>>> GetSeasonDataWR(int playerId)
        {
            if (playerId > 0)
            {
                return Ok(await _statisticsService.GetSeasonDataWR(playerId));
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }
        [HttpGet("data/te/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataTE>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonDataTE>>> GetSeasonDataTE(int playerId)
        {
            if (playerId > 0)
            {
                return Ok(await _statisticsService.GetSeasonDataTE(playerId));
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }
    }
}
