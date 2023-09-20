using Microsoft.AspNetCore.Mvc;
using Football.Models;
using Football.Data.Models;
using Football.Fantasy.Interfaces;
using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.Extensions.Options;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IPlayersService _playersService;
        private readonly Season _season;
        public PlayerController(IPlayersService playersService, IStatisticsService statisticsService, IOptionsMonitor<Season> season)
        {
            _playersService = playersService;
            _statisticsService = statisticsService;
            _season = season.CurrentValue;
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
        [HttpGet("data/weekly/qb/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataQB>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyDataQB>>> GetWeeklyDataQB(int playerId)
        {
            if (playerId > 0)
            {
                return Ok(await _statisticsService.GetWeeklyDataQB(playerId));
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }
        [HttpGet("data/weekly/rb/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataRB>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyDataRB>>> GetWeeklyDataRB(int playerId)
        {
            if (playerId > 0)
            {
                return Ok(await _statisticsService.GetWeeklyDataRB(playerId));
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }
        [HttpGet("data/weekly/wr/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataWR>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyDataWR>>> GetWeeklyDataWR(int playerId)
        {
            if (playerId > 0)
            {
                return Ok(await _statisticsService.GetWeeklyDataWR(playerId));
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }
        [HttpGet("data/weekly/te/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataTE>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyDataTE>>> GetWeeklyDataTE(int playerId)
        {
            if (playerId > 0)
            {
                return Ok(await _statisticsService.GetWeeklyDataTE(playerId));
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }

        [HttpGet("team/{playerId}")]
        [ProducesResponseType(typeof(PlayerTeam), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<PlayerTeam>> GetPlayerTeam(int playerId)
        {
            if (playerId > 0)
            {
                return Ok(await _playersService.GetPlayerTeam(_season.CurrentSeason, playerId));
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }

        [HttpGet("schedule/{playerId}")]
        [ProducesResponseType(typeof(List<Schedule>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<Schedule>>> GetUpcomingGames(int playerId)
        {
            if (playerId > 0)
            {
                return Ok(await _playersService.GetUpcomingGames(playerId));
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }

        [HttpGet("schedule/weekly")]
        [ProducesResponseType(typeof(List<Schedule>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<Schedule>>> GetGames()
        {
           return Ok(await _playersService.GetGames(_season.CurrentSeason, await _playersService.GetCurrentWeek(_season.CurrentSeason)));
        }


    }
}
