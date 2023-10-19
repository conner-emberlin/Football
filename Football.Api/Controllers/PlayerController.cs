using Microsoft.AspNetCore.Mvc;
using Football.Models;
using Football.Enums;
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
        public async Task<ActionResult<List<Player>>> GetAllPlayers() => Ok(await _playersService.GetAllPlayers());

        [HttpGet("data/qb/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataQB>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonDataQB>>> GetSeasonDataQB(int playerId) => playerId > 0 ? Ok(await _statisticsService.GetSeasonDataQB(playerId)) : BadRequest();

        [HttpGet("data/rb/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataRB>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonDataRB>>> GetSeasonDataRB(int playerId) => playerId > 0 ? Ok(await _statisticsService.GetSeasonDataRB(playerId)) : BadRequest();

        [HttpGet("data/wr/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataWR>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonDataWR>>> GetSeasonDataWR(int playerId) => playerId > 0 ? Ok(await _statisticsService.GetSeasonDataWR(playerId)) : BadRequest();

        [HttpGet("data/te/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataTE>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonDataTE>>> GetSeasonDataTE(int playerId) => playerId > 0 ? Ok(await _statisticsService.GetSeasonDataTE(playerId)) : BadRequest();

        [HttpGet("data/dst/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataDST>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonDataDST>>> GetSeasonDataDST(int playerId) => playerId > 0 ? Ok(await _statisticsService.GetSeasonDataDST(playerId)) : BadRequest();

        [HttpGet("data/weekly/qb/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataQB>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyDataQB>>> GetWeeklyDataQB(int playerId) => playerId > 0 ? Ok(await _statisticsService.GetWeeklyData<WeeklyDataQB>(PositionEnum.QB, playerId)) : BadRequest();

        [HttpGet("data/weekly/rb/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataRB>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyDataRB>>> GetWeeklyDataRB(int playerId) => playerId > 0 ? Ok(await _statisticsService.GetWeeklyData<WeeklyDataRB>(PositionEnum.RB, playerId)) : BadRequest();

        [HttpGet("data/weekly/wr/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataWR>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyDataWR>>> GetWeeklyDataWR(int playerId) => playerId > 0 ? Ok(await _statisticsService.GetWeeklyData<WeeklyDataWR>(PositionEnum.WR, playerId)) : BadRequest();

        [HttpGet("data/weekly/te/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataTE>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyDataTE>>> GetWeeklyDataTE(int playerId) => playerId > 0 ? Ok(await _statisticsService.GetWeeklyData<WeeklyDataTE>(PositionEnum.TE, playerId)) : BadRequest();

        [HttpGet("data/weekly/dst/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataDST>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyDataDST>>> GetWeeklyDataDST(int playerId) => playerId > 0 ? Ok(await _statisticsService.GetWeeklyData<WeeklyDataDST>(PositionEnum.DST, playerId)) : BadRequest();

        [HttpGet("team/{playerId}")]
        [ProducesResponseType(typeof(PlayerTeam), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<PlayerTeam>> GetPlayerTeam(int playerId) => playerId > 0 ? Ok(await _playersService.GetPlayerTeam(_season.CurrentSeason, playerId)) : BadRequest();

        [HttpGet("schedule/{playerId}")]
        [ProducesResponseType(typeof(List<Schedule>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<Schedule>>> GetUpcomingGames(int playerId) => playerId > 0 ? Ok(await _playersService.GetUpcomingGames(playerId)) : BadRequest();

        [HttpGet("schedule/weekly")]
        [ProducesResponseType(typeof(List<Schedule>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<Schedule>>> GetGames() => Ok(await _playersService.GetGames(_season.CurrentSeason, await _playersService.GetCurrentWeek(_season.CurrentSeason)));

        [HttpGet("current-week")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> GetCurrentWeek() => Ok(await _playersService.GetCurrentWeek(_season.CurrentSeason));

        [HttpPost("injury")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> PostInSeasonInjury([FromBody] InSeasonInjury injury) => injury != null ? Ok(await _playersService.PostInSeasonInjury(injury)) : BadRequest();
    }
}
