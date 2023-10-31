using Microsoft.AspNetCore.Mvc;
using Football.Enums;
using Football.Models;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using Football.Players.Interfaces;
using Microsoft.Extensions.Options;
using Football.LeagueAnalysis.Interfaces;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectionController : ControllerBase
    {
        private readonly IPlayersService _playersService;
        private readonly IProjectionAnalysisService _analysisService;
        private readonly IProjectionService<SeasonProjection> _seasonProjectionService;
        private readonly IProjectionService<WeekProjection> _weekProjectionService;
        private readonly ILeagueAnalysisService _leagueService;
        private readonly Season _season;

        public ProjectionController(IPlayersService playersService, 
            IProjectionAnalysisService analysisService, IOptionsMonitor<Season> season, 
            IProjectionService<WeekProjection> weekProjectionService, IProjectionService<SeasonProjection> seasonProjectionService,
            ILeagueAnalysisService leagueService)
        {
            _playersService = playersService;
            _analysisService = analysisService;
            _season = season.CurrentValue;
            _seasonProjectionService = seasonProjectionService;
            _weekProjectionService = weekProjectionService;
            _leagueService = leagueService;
        }

        [HttpGet("season/{position}")]
        [ProducesResponseType(typeof(List<SeasonProjection>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonProjection>>> GetSeasonProjections(string position)
        {
            if(Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum))
            {
                return positionEnum == Position.FLEX ? Ok(await _analysisService.SeasonFlexRankings())
                                      : Ok(await _seasonProjectionService.GetProjections(positionEnum));
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }
        [HttpGet("season/player/{playerId}")]
        [ProducesResponseType(typeof(SeasonProjection), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<SeasonProjection>> GetSeasonProjections(int playerId) => playerId > 0 ? 
            Ok((await _seasonProjectionService.GetPlayerProjections(playerId)).Where(p => p.Season == _season.CurrentSeason).First()) : BadRequest("Bad Request");

        [HttpPost("season/{position}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> PostSeasonProjections(string position)
        {
            if (Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum))  
            {
                var proj = (await _seasonProjectionService.GetProjections(positionEnum)).ToList();
                return Ok(await _seasonProjectionService.PostProjections(proj));
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }

        [HttpGet("weekly/{position}")]
        [ProducesResponseType(typeof(List<WeekProjection>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeekProjection>>> GetWeeklyProjections(string position)
        {
            if (Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum))
            {
                return positionEnum == Position.FLEX ? Ok(await _analysisService.WeeklyFlexRankings()) 
                                                         : Ok(await _weekProjectionService.GetProjections(positionEnum));
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPost("weekly/{position}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> PostWeeklyProjections(string position)
        {
            if (Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum))
            {
                var proj = (await _weekProjectionService.GetProjections(positionEnum)).ToList();
                return Ok(await _weekProjectionService.PostProjections(proj));
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }

        [HttpGet("weekly-analysis/{position}")]
        [ProducesResponseType(typeof(List<WeeklyProjectionAnalysis>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyProjectionAnalysis>>> GetWeeklyProjectionAnalysis(string position)
        {
            if (Enum.TryParse(position, out Position positionEnum))
            {
                List<WeeklyProjectionAnalysis> projectionAnalyses = new();
                var currentWeek = await _playersService.GetCurrentWeek(_season.CurrentSeason);
                for (int i = 2; i < currentWeek; i++)
                {
                    projectionAnalyses.Add(await _analysisService.GetWeeklyProjectionAnalysis(positionEnum, i));
                }
                return Ok(projectionAnalyses);
            }
            else return BadRequest("Bad Request");
        }

        [HttpGet("weekly-error/{position}/{week}")]
        [ProducesResponseType(typeof(List<WeeklyProjectionError>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyProjectionError>>> GetWeeklyProjectionError(string position, int week)
        {
            if (Enum.TryParse(position, out Position positionEnum))
            {
                return Ok(await _analysisService.GetWeeklyProjectionError(positionEnum, week));
            }
            else return BadRequest("Bad Request");
        }

        [HttpGet("sleeper-projections/{username}")]
        [ProducesResponseType(typeof(List<WeekProjection>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeekProjection>>> GetSleeperLeagueProjections([FromRoute] string username) => Ok(await _leagueService.GetSleeperLeagueProjections(username));

        [HttpGet("sleeper-projections/{username}/matchup")]
        [ProducesResponseType(typeof(Dictionary<string, List<WeekProjection>>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<Dictionary<string, List<WeekProjection>>>> GetMatchupProjections([FromRoute] string username)
        {
            var currentWeek = await _playersService.GetCurrentWeek(_season.CurrentSeason);
            return Ok(await _leagueService.GetMatchupProjections(username, currentWeek));
        }
    }
}
