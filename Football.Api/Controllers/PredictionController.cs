using Football.Api.Helpers;
using Football.Interfaces;
using Football.Models;
using Football.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionController : ControllerBase
    {
        private readonly IFantasyService _fantasyService;
        private readonly IPredictionService _predictionService;
        private readonly IPlayerService _playerService;
        private readonly IServiceHelper _serviceHelper;
        public PredictionController(IFantasyService fantasyService, IPredictionService predictionService, IPlayerService playerService, IServiceHelper serviceHelper)
        {
            _fantasyService = fantasyService;
            _predictionService = predictionService;
            _playerService = playerService;
            _serviceHelper = serviceHelper;
        }

        [HttpGet("{position}")]
        [ProducesResponseType(typeof(List<ProjectionModel>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<IEnumerable<ProjectionModel>>> GetProjection(int position)
        {
            if (position == 5)
            {
                return Ok(await _predictionService.FlexRankings());
            }
            else
            {
                return Ok(await _predictionService.GetFinalProjections(_serviceHelper.TransformPosition(position)));
            }
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
