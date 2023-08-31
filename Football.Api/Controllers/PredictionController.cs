using Football.Api.Helpers;
using Football.Interfaces;
using Football.Models;
using Microsoft.AspNetCore.Mvc;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionController : ControllerBase
    {
        private readonly IPredictionService _predictionService;
        private readonly IServiceHelper _serviceHelper;
        public PredictionController(IPredictionService predictionService, IServiceHelper serviceHelper)
        {
            _predictionService = predictionService;
            _serviceHelper = serviceHelper;
        }

        [HttpGet("{position}")]
        [ProducesResponseType(typeof(List<ProjectionModel>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<IEnumerable<ProjectionModel>>> GetProjection(int position)
        {
            return position == 5 ? Ok(await _predictionService.FlexRankings())
                        : Ok(await _predictionService.GetFinalProjections(_serviceHelper.TransformPosition(position)));
        }

        [HttpGet("flex-rankings")]
        [ProducesResponseType(typeof(List<FlexProjectionModel>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<FlexProjectionModel>>> GetFlexRankings()
        {
            return Ok(await _predictionService.FlexRankings());
        }

        [HttpPost("upload/{position}")]
        [ProducesResponseType(typeof(List<int>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> InsertFantasyProjections(int position)
        {
            return Ok(await _predictionService.InsertFantasyProjections(_serviceHelper.TransformPosition(position)));
        }

    }
}
