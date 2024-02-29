using Microsoft.AspNetCore.Mvc;
using Football.Enums;
using Football.Models;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using Football.Players.Interfaces;
using Microsoft.Extensions.Options;
using Football.Leagues.Interfaces;
using Football.Leagues.Models;
using HtmlAgilityPack;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectionController(IPlayersService playersService,
        IProjectionAnalysisService analysisService, IOptionsMonitor<Season> season,
        IProjectionService<WeekProjection> weekProjectionService, IProjectionService<SeasonProjection> seasonProjectionService,
        ILeagueAnalysisService leagueService) : ControllerBase
    {
        private readonly Season _season = season.CurrentValue;

        [HttpGet("season/{position}")]
        [ProducesResponseType(typeof(List<SeasonProjection>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonProjection>>> GetSeasonProjections(string position)
        {
            if(Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum))
            {
                return positionEnum == Position.FLEX ? Ok(await analysisService.SeasonFlexRankings())
                                      : Ok(await seasonProjectionService.GetProjections(positionEnum));
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
            Ok((await seasonProjectionService.GetPlayerProjections(playerId)).Where(p => p.Season == _season.CurrentSeason).First()) : BadRequest("Bad Request");

        [HttpPost("season/{position}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> PostSeasonProjections(string position)
        {
            if (Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum))  
            {
                var proj = (await seasonProjectionService.GetProjections(positionEnum)).ToList();
                return Ok(await seasonProjectionService.PostProjections(proj));
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
                return positionEnum == Position.FLEX ? Ok(await analysisService.WeeklyFlexRankings()) 
                                                         : Ok(await weekProjectionService.GetProjections(positionEnum));
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
                var proj = (await weekProjectionService.GetProjections(positionEnum)).ToList();
                return Ok(await weekProjectionService.PostProjections(proj));
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
                var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
                for (int i = 2; i < currentWeek; i++)
                {
                    projectionAnalyses.Add(await analysisService.GetWeeklyProjectionAnalysis(positionEnum, i));
                }
                return Ok(projectionAnalyses);
            }
            else return BadRequest("Bad Request");
        }

        [HttpGet("weekly-analysis/player/{playerId}")]
        [ProducesResponseType(typeof(WeeklyProjectionAnalysis), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetWeeklyProjectionAnalysis(int playerId) => playerId > 0 ? Ok(await analysisService.GetWeeklyProjectionAnalysis(playerId)) : BadRequest();

        [HttpGet("weekly-error/{position}/{week}")]
        [ProducesResponseType(typeof(List<WeeklyProjectionError>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyProjectionError>>> GetWeeklyProjectionError(string position, int week)
        {
            if (Enum.TryParse(position, out Position positionEnum))
            {
                return Ok(await analysisService.GetWeeklyProjectionError(positionEnum, week));
            }
            else return BadRequest("Bad Request");
        }

        [HttpGet("weekly-error/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyProjectionError>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetWeeklyProjectionError(int playerId) => playerId > 0 ? Ok(await analysisService.GetWeeklyProjectionError(playerId)) : BadRequest();

        [HttpGet("sleeper-projections/{username}")]
        [ProducesResponseType(typeof(List<WeekProjection>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeekProjection>>> GetSleeperLeagueProjections([FromRoute] string username) => Ok(await leagueService.GetSleeperLeagueProjections(username));

        [HttpGet("sleeper-projections/{username}/matchup")]
        [ProducesResponseType(typeof(List<MatchupProjections>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<MatchupProjections>>> GetMatchupProjections([FromRoute] string username)
        {
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            return Ok(await leagueService.GetMatchupProjections(username, currentWeek));
        }

        [HttpDelete("weekly/{playerId}/{season}/{week}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteWeeklyProjection(int playerId, int season, int week)
        {
            var projection = (await weekProjectionService.GetPlayerProjections(playerId)).FirstOrDefault(p => p.Week == week && p.Season == season);
            return projection != null ? Ok(await weekProjectionService.DeleteProjection(projection)) : BadRequest();
        }

        [HttpDelete("season/{playerId}/{season}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteSeasonProjection(int playerId, int season)
        {
            var projection = (await seasonProjectionService.GetPlayerProjections(playerId)).FirstOrDefault(p => p.Season == season);
            return projection != null ? Ok(await seasonProjectionService.DeleteProjection(projection)) : BadRequest();
        }

        [HttpGet("season-projection-analysis/{position}")]
        [ProducesResponseType(typeof(List<SeasonProjectionAnalysis>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetSesasonProjectionAnalyses([FromRoute] string position) => Enum.TryParse(position, out Position posEnum) ? Ok(await analysisService.GetSeasonProjectionAnalyses(posEnum)) : BadRequest();
    }
}
