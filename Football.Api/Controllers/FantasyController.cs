using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FantasyController : ControllerBase
    {
        //POST refresh all fantasy results
        //GET fantasy results for playerId, season
        //POST fantasy results for a season/position (delete existing ones for the season/position)
    }
}
