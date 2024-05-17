using Football.Players.Interfaces;
using Football.Statistics.Models;
using Football.Statistics.Interfaces;
using Football.Models;
using Football.Players.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Football.Data.Models;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController(IPlayersService playersService, IStatisticsService statisticsService, IFantasyAnalysisService fantasyAnalysisService,
        IOptionsMonitor<Season> season, IFantasyDataService fantasyDataService, IMarketShareService marketShareService, IAdvancedStatisticsService advancedStatisticsService) : ControllerBase
    {
        private readonly Season _season = season.CurrentValue;

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TeamMap>>> GetAllTeams() => Ok(await playersService.GetAllTeams());

        [HttpGet("players/{team}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Player>>> GetPlayersByTeam(string team) => Ok(await playersService.GetPlayersByTeam(team));

        [HttpGet("location/{teamId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<TeamLocation>> GetTeamLocation(int teamId) => Ok(await playersService.GetTeamLocation(teamId));

        [HttpGet("schedule-details/current")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ScheduleDetails>>> GetScheduleDetails() => Ok(await playersService.GetScheduleDetails(_season.CurrentSeason, await playersService.GetCurrentWeek(_season.CurrentSeason)));

        [HttpGet("team-records")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TeamRecord>>> GetTeamRecords() => Ok(await statisticsService.GetTeamRecords(_season.CurrentSeason));

        [HttpGet("game-results")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GameResult>>> GetGameResults() => Ok(await statisticsService.GetGameResults(_season.CurrentSeason));

        [HttpGet("fantasy-performances/{teamId}")]
        [ProducesResponseType(typeof(List<FantasyPerformance>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFantasyPerformances([FromRoute] int teamId) => teamId > 0 ? Ok(await fantasyAnalysisService.GetFantasyPerformances(teamId)) : BadRequest("Bad Request");

        [HttpGet("weekly-fantasy/{team}/{week}")]
        [ProducesResponseType(typeof(List<WeeklyFantasy>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyTeamFantasy([FromRoute] string team, [FromRoute] int week) => week > 0 ? Ok(await fantasyDataService.GetWeeklyTeamFantasy(team, week)) : BadRequest("Bad Request");

        [HttpGet("totals/{teamId}")]
        [ProducesResponseType(typeof(List<TeamTotals>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTeamTotals([FromRoute] int teamId) => teamId > 0 ? Ok(await marketShareService.GetTeamTotals(teamId)) : BadRequest();

        [HttpGet("sos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<StrengthOfSchedule>>> GetRemainingStrengthOfSchedule() => Ok(await advancedStatisticsService.RemainingStrengthOfSchedule());
    }
}
