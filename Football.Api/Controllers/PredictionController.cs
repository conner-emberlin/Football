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
        public PredictionController(IFantasyService fantasyService, IPredictionService predictionService)
        {
            _fantasyService = fantasyService;
            _predictionService = predictionService;
        }
        [HttpGet("model-error/{playerId}")]
        [ProducesResponseType(typeof(List<int>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public ActionResult<List<double>> GetModelErrorByPlayer(int playerId)
        {
            return _predictionService.ModelErrorPerSeason(playerId, _fantasyService.GetPlayerPosition(playerId));
        }

        [HttpGet("{position}")]
        [ProducesResponseType(typeof(List<int>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public ActionResult<IEnumerable<ProjectionModel>> GetProjection(int position)
        {
            ServiceHelper help = new();
            var pos = help.TransformPosition(position);
            return Ok(_predictionService.GetProjections(pos));
        }
    }
}
