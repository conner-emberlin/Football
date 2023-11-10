using Microsoft.AspNetCore.Mvc;
using Football.Models;
using Football.Enums;
using Football.Data.Models;
using Football.Statistics.Interfaces;
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
        private readonly IDistanceService _distanceService;
        private readonly IAdvancedStatisticsService _advancedStatisticsService;
        private readonly Season _season;
        public PlayerController(IPlayersService playersService, IStatisticsService statisticsService, IDistanceService distanceService,
            IOptionsMonitor<Season> season, IAdvancedStatisticsService advancedStatisticsService)
        {
            _playersService = playersService;
            _statisticsService = statisticsService;
            _distanceService = distanceService;
            _season = season.CurrentValue;
            _advancedStatisticsService = advancedStatisticsService;
        }
        [HttpGet("data/players")]
        [ProducesResponseType(typeof(List<Player>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<Player>>> GetAllPlayers() => Ok(await _playersService.GetAllPlayers());

        [HttpGet("data/qb/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataQB>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonDataQB>>> GetSeasonDataQB(int playerId) => playerId > 0 ? Ok(await _statisticsService.GetSeasonData<SeasonDataQB>(Position.QB, playerId, true)) : BadRequest();

        [HttpGet("data/rb/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataRB>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonDataRB>>> GetSeasonDataRB(int playerId) => playerId > 0 ? Ok(await _statisticsService.GetSeasonData<SeasonDataRB>(Position.RB, playerId, true)) : BadRequest();

        [HttpGet("data/wr/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataWR>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonDataWR>>> GetSeasonDataWR(int playerId) => playerId > 0 ? Ok(await _statisticsService.GetSeasonData<SeasonDataWR>(Position.WR, playerId, true)) : BadRequest();

        [HttpGet("data/te/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataTE>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonDataTE>>> GetSeasonDataTE(int playerId) => playerId > 0 ? Ok(await _statisticsService.GetSeasonData<SeasonDataTE>(Position.TE, playerId, true)) : BadRequest();

        [HttpGet("data/dst/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataDST>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonDataDST>>> GetSeasonDataDST(int playerId) => playerId > 0 ? Ok(await _statisticsService.GetSeasonData<SeasonDataDST>(Position.DST, playerId, true)) : BadRequest();

        [HttpGet("data/weekly/qb/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataQB>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyDataQB>>> GetWeeklyDataQB(int playerId) => playerId > 0 ? Ok(await _statisticsService.GetWeeklyData<WeeklyDataQB>(Position.QB, playerId)) : BadRequest();

        [HttpGet("data/weekly/rb/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataRB>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyDataRB>>> GetWeeklyDataRB(int playerId) => playerId > 0 ? Ok(await _statisticsService.GetWeeklyData<WeeklyDataRB>(Position.RB, playerId)) : BadRequest();

        [HttpGet("data/weekly/wr/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataWR>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyDataWR>>> GetWeeklyDataWR(int playerId) => playerId > 0 ? Ok(await _statisticsService.GetWeeklyData<WeeklyDataWR>(Position.WR, playerId)) : BadRequest();

        [HttpGet("data/weekly/te/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataTE>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyDataTE>>> GetWeeklyDataTE(int playerId) => playerId > 0 ? Ok(await _statisticsService.GetWeeklyData<WeeklyDataTE>(Position.TE, playerId)) : BadRequest();

        [HttpGet("data/weekly/dst/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataDST>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyDataDST>>> GetWeeklyDataDST(int playerId) => playerId > 0 ? Ok(await _statisticsService.GetWeeklyData<WeeklyDataDST>(Position.DST, playerId)) : BadRequest();

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

        [HttpPost("team-change/in-season")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> PostInSeasonTeamChange([FromBody] InSeasonTeamChange teamChange) => Ok(await _playersService.PostInSeasonTeamChange(teamChange));

        [HttpPost("travel-distance/{playerId}")]
        [ProducesResponseType(typeof(double), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<double>> GetTravelDistance([FromRoute] int playerId) => Ok(await _distanceService.GetTravelDistance(playerId));

        [HttpGet("qb-value")]
        [ProducesResponseType(typeof(double), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<double>> GetQBValue() => Ok(await _advancedStatisticsService.FiveThirtyEightQBValue());

        [HttpGet("passer-rating")]
        [ProducesResponseType(typeof(double), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<double>> GetPasserRating() => Ok(await _advancedStatisticsService.PasserRating());

        [HttpGet("passer-rating/{playerId}")]
        [ProducesResponseType(typeof(double), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<double>> GetPasserRating(int playerId) => Ok(await _advancedStatisticsService.PasserRating(playerId));

        [HttpGet("qb-value/{playerId}")]
        [ProducesResponseType(typeof(double), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<double>> GetQBValue(int playerId) => Ok(await _advancedStatisticsService.FiveThirtyEightQBValue(playerId));

    }
}
