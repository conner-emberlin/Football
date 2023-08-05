using Football.Api.Helpers;
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
        [HttpGet("model-error/{playerId}")]
        [ProducesResponseType(typeof(List<int>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public ActionResult<List<double>> GetModelErrorByPlayer(int playerId)
        {
            PredictionService predictionService = new();
            FantasyService fantasyService = new();

            return predictionService.ModelErrorPerSeason(playerId, fantasyService.GetPlayerPosition(playerId));
        }

        [HttpGet("{position}")]
        [ProducesResponseType(typeof(List<int>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public ActionResult<IEnumerable<ProjectionModel>> GetProjection(int position)
        {
            PredictionService predictionService = new();
            ServiceHelper help = new();
            var pos = help.TransformPosition(position);
            return Ok(predictionService.GetProjections(pos));
        }
    }
}
