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
        [ProducesResponseType(typeof(List<TeamMap>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<Player>>> GetAllTeams() => Ok(await _playersService.GetAllTeams());

        [HttpGet("players/{team}")]
        [ProducesResponseType(typeof(List<PlayerTeam>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<Player>>> GetPlayersByTeam(string team) => Ok(await _playersService.GetPlayersByTeam(team));

        [HttpGet("location/{teamId}")]
        [ProducesResponseType(typeof(TeamLocation), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<TeamLocation>> GetTeamLocation(int teamId) => Ok(await _playersService.GetTeamLocation(teamId));

        [HttpGet("schedule-details/{week}")]
        [ProducesResponseType(typeof(List<ScheduleDetails>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<ScheduleDetails>>> GetScheduleDetails(int week) => Ok(await _playersService.GetScheduleDetails(_season.CurrentSeason, week));

        [HttpGet("team-records")]
        [ProducesResponseType(typeof(List<TeamRecord>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<TeamRecord>>> GetTeamRecords() => Ok(await _statisticsService.GetTeamRecords(_season.CurrentSeason));

        [HttpGet("game-results")]
        [ProducesResponseType(typeof(List<GameResult>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<GameResult>>> GetGameResults() => Ok(await _statisticsService.GetGameResults(_season.CurrentSeason));

        [HttpGet("fantasy-performances/{teamId}")]
        [ProducesResponseType(typeof(List<FantasyPerformance>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<FantasyPerformance>>> GetFantasyPerformances([FromRoute] int teamId) => Ok(await _fantasyAnalysisService.GetFantasyPerformances(teamId));


    }
}
