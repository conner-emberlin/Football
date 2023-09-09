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
        public async Task<ActionResult<List<SeasonProjection>>> GetSeasonProjection(string position)
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
        [HttpGet("season/flex")]
        [ProducesResponseType(typeof(List<SeasonFlex>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonFlex>>> GetSeasonFlexRankings()
        {
            return Ok(await _projectionService.SeasonFlexRankings());
        }
        [HttpGet("season/rookie/{position}")]
        [ProducesResponseType(typeof(List<SeasonProjection>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonProjection>>> GetRookieProjections(string position)
        {
            return Ok(await _projectionService.RookieSeasonProjections(position));
        }
    }
}
