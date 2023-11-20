using Football.Players.Interfaces;
using Football.Statistics.Models;
using Football.Statistics.Interfaces;
using Football.Models;
using Football.Players.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Football.Data.Models;
using Football.Fantasy.Analysis.Interfaces;
using Football.Fantasy.Analysis.Models;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly IPlayersService _playersService;
        private readonly IStatisticsService _statisticsService;
        private readonly IFantasyAnalysisService _fantasyAnalysisService;
        private readonly Season _season;
        public TeamController(IPlayersService playersService, IStatisticsService statisticsService, IFantasyAnalysisService fantasyAnalysisService,
            IOptionsMonitor<Season> season)
        {
            _playersService = playersService;
            _statisticsService = statisticsService;
            _fantasyAnalysisService = fantasyAnalysisService;
            _season = season.CurrentValue;
        }

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Player>>> GetAllTeams() => Ok(await _playersService.GetAllTeams());

        [HttpGet("players/{team}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Player>>> GetPlayersByTeam(string team) => Ok(await _playersService.GetPlayersByTeam(team));

        [HttpGet("location/{teamId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<TeamLocation>> GetTeamLocation(int teamId) => Ok(await _playersService.GetTeamLocation(teamId));

        [HttpGet("schedule-details/current")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ScheduleDetails>>> GetScheduleDetails() => Ok(await _playersService.GetScheduleDetails(_season.CurrentSeason, await _playersService.GetCurrentWeek(_season.CurrentSeason)));

        [HttpGet("team-records")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TeamRecord>>> GetTeamRecords() => Ok(await _statisticsService.GetTeamRecords(_season.CurrentSeason));

        [HttpGet("game-results")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GameResult>>> GetGameResults() => Ok(await _statisticsService.GetGameResults(_season.CurrentSeason));

        [HttpGet("fantasy-performances/{teamId}")]
        [ProducesResponseType(typeof(List<FantasyPerformance>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFantasyPerformances([FromRoute] int teamId) => teamId > 0 ? Ok(await _fantasyAnalysisService.GetFantasyPerformances(teamId)) : BadRequest("Bad Request");


    }
}
