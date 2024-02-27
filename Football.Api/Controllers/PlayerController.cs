using Microsoft.AspNetCore.Mvc;
using Football.Models;
using Football.Enums;
using Football.Data.Models;
using Football.Statistics.Interfaces;
using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.Extensions.Options;
using Football.Statistics.Models;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController(IPlayersService playersService, IStatisticsService statisticsService, IDistanceService distanceService,
        IOptionsMonitor<Season> season, IAdvancedStatisticsService advancedStatisticsService) : ControllerBase
    {
        private readonly Season _season = season.CurrentValue;

        [HttpGet("data/players")]
        [ProducesResponseType(typeof(List<Player>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<Player>>> GetAllPlayers() => Ok(await playersService.GetAllPlayers());

        [HttpGet("data/qb/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataQB>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonDataQB>>> GetSeasonDataQB(int playerId) => playerId > 0 ? Ok(await statisticsService.GetSeasonData<SeasonDataQB>(Position.QB, playerId, true)) : BadRequest();

        [HttpGet("data/rb/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataRB>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonDataRB>>> GetSeasonDataRB(int playerId) => playerId > 0 ? Ok(await statisticsService.GetSeasonData<SeasonDataRB>(Position.RB, playerId, true)) : BadRequest();

        [HttpGet("data/wr/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataWR>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonDataWR>>> GetSeasonDataWR(int playerId) => playerId > 0 ? Ok(await statisticsService.GetSeasonData<SeasonDataWR>(Position.WR, playerId, true)) : BadRequest();

        [HttpGet("data/te/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataTE>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonDataTE>>> GetSeasonDataTE(int playerId) => playerId > 0 ? Ok(await statisticsService.GetSeasonData<SeasonDataTE>(Position.TE, playerId, true)) : BadRequest();

        [HttpGet("data/dst/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataDST>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonDataDST>>> GetSeasonDataDST(int playerId) => playerId > 0 ? Ok(await statisticsService.GetSeasonData<SeasonDataDST>(Position.DST, playerId, true)) : BadRequest();

        [HttpGet("data/weekly/qb/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataQB>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyDataQB>>> GetWeeklyDataQB(int playerId) => playerId > 0 ? Ok((await statisticsService.GetWeeklyData<WeeklyDataQB>(Position.QB, playerId)).Where(w => w.Season == _season.CurrentSeason).ToList()) : BadRequest();

        [HttpGet("data/weekly/rb/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataRB>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyDataRB>>> GetWeeklyDataRB(int playerId) => playerId > 0 ? Ok((await statisticsService.GetWeeklyData<WeeklyDataRB>(Position.RB, playerId)).Where(w => w.Season == _season.CurrentSeason).ToList()) : BadRequest();

        [HttpGet("data/weekly/wr/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataWR>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyDataWR>>> GetWeeklyDataWR(int playerId) => playerId > 0 ? Ok((await statisticsService.GetWeeklyData<WeeklyDataWR>(Position.WR, playerId)).Where(w => w.Season == _season.CurrentSeason).ToList()) : BadRequest();

        [HttpGet("data/weekly/te/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataTE>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyDataTE>>> GetWeeklyDataTE(int playerId) => playerId > 0 ? Ok((await statisticsService.GetWeeklyData<WeeklyDataTE>(Position.TE, playerId)).Where(w => w.Season == _season.CurrentSeason).ToList()) : BadRequest();

        [HttpGet("data/weekly/dst/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataDST>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyDataDST>>> GetWeeklyDataDST(int playerId) => playerId > 0 ?  Ok((await statisticsService.GetWeeklyData<WeeklyDataDST>(Position.DST, playerId)).Where(w => w.Season == _season.CurrentSeason).ToList()) : BadRequest();

        [HttpGet("data/weekly/k/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataK>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyDataK>>> GetWeeklyDataK(int playerId) => playerId > 0 ? Ok((await statisticsService.GetWeeklyData<WeeklyDataK>(Position.K, playerId)).Where(w => w.Season == _season.CurrentSeason).ToList()) : BadRequest();

        [HttpGet("team/{playerId}")]
        [ProducesResponseType(typeof(PlayerTeam), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<PlayerTeam>> GetPlayerTeam(int playerId) => playerId > 0 ? Ok(await playersService.GetPlayerTeam(_season.CurrentSeason, playerId)) : BadRequest();

        [HttpGet("schedule/{playerId}")]
        [ProducesResponseType(typeof(List<Schedule>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<Schedule>>> GetUpcomingGames(int playerId) => playerId > 0 ? Ok(await playersService.GetUpcomingGames(playerId)) : BadRequest();

        [HttpGet("schedule/weekly")]
        [ProducesResponseType(typeof(List<Schedule>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<Schedule>>> GetGames() => Ok(await playersService.GetGames(_season.CurrentSeason, await playersService.GetCurrentWeek(_season.CurrentSeason)));

        [HttpGet("current-week")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> GetCurrentWeek() => Ok(await playersService.GetCurrentWeek(_season.CurrentSeason));

        [HttpPost("injury")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> PostInSeasonInjury([FromBody] InSeasonInjury injury) => injury != null ? Ok(await playersService.PostInSeasonInjury(injury)) : BadRequest();

        [HttpGet("player-injuries")]
        [ProducesResponseType(typeof(List<PlayerInjury>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<PlayerInjury>>> GetPlayerInjuries() => Ok(await playersService.GetPlayerInjuries());

        [HttpPut("update-injury")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<bool>> UpdateInjury([FromBody] InSeasonInjury injury) => Ok(await playersService.UpdateInjury(injury));

        [HttpPost("team-change/in-season")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> PostInSeasonTeamChange([FromBody] InSeasonTeamChange teamChange) => Ok(await playersService.PostInSeasonTeamChange(teamChange));

        [HttpPost("travel-distance/{playerId}")]
        [ProducesResponseType(typeof(double), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<double>> GetTravelDistance([FromRoute] int playerId) => Ok(await distanceService.GetTravelDistance(playerId));

        [HttpGet("advanced-stats/qb")]
        [ProducesResponseType(typeof(List<AdvancedQBStatistics>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<AdvancedQBStatistics>>> GetAdvancedQBStatistics() => Ok(await advancedStatisticsService.GetAdvancedQBStatistics());

        [HttpGet("snap-counts/{playerId}")]
        [ProducesResponseType(typeof(List<SnapCount>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetSnapCounts([FromRoute] int playerId) => Ok(await statisticsService.GetSnapCounts(playerId));
    }
}
