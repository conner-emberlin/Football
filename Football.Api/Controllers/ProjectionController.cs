using Microsoft.AspNetCore.Mvc;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using Football.Fantasy.Models;

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
            return Ok(await _projectionService.GetSeasonProjections(position));
        }
    }
}
