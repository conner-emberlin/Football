using Football.Players.Interfaces;
using Football.Models;
using Football.Players.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using AutoMapper;
using Football.Shared.Models.Fantasy;
using Football.Shared.Models.Teams;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController(IPlayersService playersService, IStatisticsService statisticsService, IFantasyAnalysisService fantasyAnalysisService,
        IOptionsMonitor<Season> season, IFantasyDataService fantasyDataService, IMarketShareService marketShareService, IAdvancedStatisticsService advancedStatisticsService, ITeamsService teamsService, IMapper mapper) : ControllerBase
    {
        private readonly Season _season = season.CurrentValue;

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TeamMapModel>>> GetAllTeams() => Ok(mapper.Map<List<TeamMapModel>>(await teamsService.GetAllTeams()));

        [HttpGet("players/{team}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PlayerTeam>>> GetPlayersByTeam(string team) => Ok(await teamsService.GetPlayersByTeam(team));

        [HttpGet("schedule-details/current")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ScheduleDetailsModel>>> GetScheduleDetails() => Ok(mapper.Map<List<ScheduleDetailsModel>>(await teamsService.GetScheduleDetails(_season.CurrentSeason, await playersService.GetCurrentWeek(_season.CurrentSeason))));

        [HttpGet("team-records")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TeamRecordModel>>> GetTeamRecords() => Ok(mapper.Map<List<TeamRecordModel>>(await teamsService.GetTeamRecords(_season.CurrentSeason)));

        [HttpGet("game-results")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GameResultModel>>> GetGameResults() => Ok(mapper.Map<List<GameResultModel>>(await statisticsService.GetGameResults(_season.CurrentSeason)));

        [HttpGet("fantasy-performances/{teamId}")]
        [ProducesResponseType(typeof(List<FantasyPerformanceModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFantasyPerformances([FromRoute] int teamId) => teamId > 0 ? Ok(mapper.Map<List<FantasyPerformanceModel>>(await fantasyAnalysisService.GetFantasyPerformances(teamId))) : BadRequest("Bad Request");

        [HttpGet("weekly-fantasy/{team}/{week}")]
        [ProducesResponseType(typeof(List<WeeklyFantasyModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyTeamFantasy([FromRoute] string team, [FromRoute] int week) => week > 0 ? Ok(mapper.Map<List<WeeklyFantasyModel>>(await fantasyDataService.GetWeeklyTeamFantasy(team, week))) : BadRequest("Bad Request");

        [HttpGet("totals/{teamId}")]
        [ProducesResponseType(typeof(List<TeamTotals>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTeamTotals([FromRoute] int teamId) => teamId > 0 ? Ok(await marketShareService.GetTeamTotals(teamId)) : BadRequest();

        [HttpGet("sos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<StrengthOfSchedule>>> GetRemainingStrengthOfSchedule() => Ok(await advancedStatisticsService.RemainingStrengthOfSchedule());

        [HttpGet("league-information/{teamId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TeamLeagueInformationModel>>> GetTeamLeagueInformation([FromRoute] int teamId) => Ok(mapper.Map<TeamLeagueInformationModel>(await teamsService.GetTeamLeagueInformation(teamId)));

        [HttpGet("team-records/division/{teamId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TeamRecordModel>>> GetTeamRecordInDivision([FromRoute] int teamId) => Ok(mapper.Map<TeamRecordModel>(await teamsService.GetTeamRecordInDivision(teamId)));
    }
}
