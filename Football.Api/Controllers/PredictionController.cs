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
        public readonly IFantasyService _fantasyService;
        public readonly IPredictionService _predictionService;
        public readonly IServiceHelper _serviceHelper;
        public PredictionController(IFantasyService fantasyService, IPredictionService predictionService, IServiceHelper serviceHelper)
        {
            _fantasyService = fantasyService;
            _predictionService = predictionService;
            _serviceHelper = serviceHelper;
        }

        [HttpGet("model-error/{playerId}")]
        [ProducesResponseType(typeof(List<int>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<double>>> GetModelErrorByPlayer(int playerId)
        {
            return await _predictionService.ModelErrorPerSeason(playerId, await _fantasyService.GetPlayerPosition(playerId));
        }
        //calculate projections for a position
        [HttpGet("{position}")]
        [ProducesResponseType(typeof(List<ProjectionModel>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<IEnumerable<ProjectionModel>>> GetProjection(int position)
        {
            return Ok(await _predictionService.GetProjections(_serviceHelper.TransformPosition(position)));
        }
        //Upload projections for a position to the DB
        [HttpPost("upload/{position}")]
        [ProducesResponseType(typeof(List<int>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> InsertFantasyProjections(int position)
        {
            return Ok(await _predictionService.InsertFantasyProjections(_serviceHelper.TransformPosition(position)));
        }

    }
}
