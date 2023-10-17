using Microsoft.AspNetCore.Mvc;
using Football.Enums;
using Football.Models;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using Football.Players.Interfaces;
using Microsoft.Extensions.Options;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectionController : ControllerBase
    {
        private readonly IProjectionService _projectionService;
        private readonly IPlayersService _playersService;
        private readonly IProjectionAnalysisService _analysisService;
        private readonly Season _season;

        public ProjectionController(IProjectionService projectionService, IPlayersService playersService, 
            IProjectionAnalysisService analysisService, IOptionsMonitor<Season> season)
        {
            _projectionService = projectionService;
            _playersService = playersService;
            _analysisService = analysisService;
            _season = season.CurrentValue;
        }

        [HttpGet("season/{position}")]
        [ProducesResponseType(typeof(List<SeasonProjection>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonProjection>>> GetSeasonProjections(string position)
        {
            if(Enum.TryParse(position.Trim().ToUpper(), out PositionEnum positionEnum))
            {
                return positionEnum == PositionEnum.FLEX ? Ok(await _projectionService.SeasonFlexRankings())
                                      : Ok(await _projectionService.GetSeasonProjections(positionEnum));
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
            Ok(await _projectionService.GetSeasonProjection(playerId)) : BadRequest("Bad Request");

        [HttpPost("season/{position}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> PostSeasonProjections(string position)
        {
            if (Enum.TryParse(position.Trim().ToUpper(), out PositionEnum positionEnum))  
            {
                var proj = (await _projectionService.GetSeasonProjections(positionEnum)).ToList();
                return Ok(await _projectionService.PostSeasonProjections(proj));
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
            if (Enum.TryParse(position.Trim().ToUpper(), out PositionEnum positionEnum))
            {
                return Ok(await _projectionService.GetWeeklyProjections(positionEnum));
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }
        [HttpPost("weekly/{position}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> PostWeeklyProjections(string position)
        {
            if (Enum.TryParse(position.Trim().ToUpper(), out PositionEnum positionEnum))
            {
                var proj = (await _projectionService.GetWeeklyProjections(positionEnum)).ToList();
                return Ok(await _projectionService.PostWeeklyProjections(proj));
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
            if (Enum.TryParse(position, out PositionEnum positionEnum))
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
            if (Enum.TryParse(position, out PositionEnum positionEnum))
            {
                return Ok(await _analysisService.GetWeeklyProjectionError(positionEnum, week));
            }
            else return BadRequest("Bad Request");
        }
    }
}
