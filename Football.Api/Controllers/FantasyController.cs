using Football.Api.Helpers;
using Football.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FantasyController : ControllerBase
    {
        //POST refresh all fantasy results
        [HttpGet("fantasy/refresh/{season}/{pos}")]
        [ProducesResponseType(typeof(double), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public ActionResult<(int,int)> RefreshFantasyPoints(int playerId, int season)
        {
            ServiceHelper serviceHelper = new();
            FantasyService fantasyService = new();

            var fantasyPoints = fantasyService.GetFantasyPoints(playerId, season);
            return Ok(fantasyService.RefreshFantasyResults(fantasyPoints));

        }
        //GET fantasy results for playerId, season
        //POST fantasy results for a season/position (delete existing ones for the season/position)
    }
}
