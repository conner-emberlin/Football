using Microsoft.AspNetCore.Mvc;
using Football.Projections.Interfaces;
using Football.Projections.Models;


namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectionController : ControllerBase
    {
        private readonly IProjectionService _projectionService;

        public ProjectionController(IProjectionService projectionService)
        {
            _projectionService = projectionService;
        }

        [HttpGet("season/{position}")]
        [ProducesResponseType(typeof(List<SeasonProjection>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonProjection>>> GetSeasonProjections(string position)
        {
            if (position == "FLEX")
            {
                return Ok(await _projectionService.SeasonFlexRankings());
            }
            else
            {
                return Ok(await _projectionService.GetSeasonProjections(position));
            }
        }
        [HttpGet("season/player/{playerId}")]
        [ProducesResponseType(typeof(SeasonProjection), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<SeasonProjection>> GetSeasonProjections(int playerId)
        {
            if(playerId > 0)
            {
                return Ok(await _projectionService.GetSeasonProjection(playerId));
            }
            else
            {
                return BadRequest("Bad Request");
            }

        }

        [HttpPost("season/{position}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> PostSeasonProjections(string position)
        {
            if (position != null)
            {
                var proj = (await _projectionService.GetSeasonProjections(position)).ToList();
                return Ok(await _projectionService.PostSeasonProjections(proj));
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }

    }
}
