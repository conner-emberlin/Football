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
    public class FantasyController : ControllerBase
    {
        private readonly IFantasyService _fantasyService;
        private readonly IPlayerService _playerService;
        public readonly IServiceHelper _serviceHelper;
        public FantasyController(IFantasyService fantasyService,IPlayerService playerService, IServiceHelper serviceHelper)
        {
            _fantasyService = fantasyService;
            _playerService = playerService;
            _serviceHelper = serviceHelper;
        }
        //POST fantasy results for a season/position (delete existing ones for the season/position)
        [HttpPost("refresh/{playerId}/{season}")]
        [ProducesResponseType(typeof(double), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<(int,int)>> RefreshFantasyPoints(int playerId, int season)
        { 
            //FantasyService fantasyService = new();
            var fantasyPoints = await _fantasyService.GetFantasyPoints(playerId, season);
            return Ok(await _fantasyService.RefreshFantasyResults(fantasyPoints));

        }
        //Use this for a complete refresh of a season. Truncate table first.
        [HttpPost("{position}/{season}")]
        [ProducesResponseType(typeof(double), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> PostFantasyPoints(string position, int season)
        {
            if (position == "WRTE")
            {
                position = "WR/TE";
            }
            var players = await _playerService.GetPlayersByPosition(position);
            int count = 0;
            foreach (var player in players)
            {
                var fantasyPoints = await _fantasyService.GetFantasyPoints(player, season);
                if (fantasyPoints.TotalPoints > 0)
                {
                    count += await _fantasyService.InsertFantasyPoints(fantasyPoints);
                }
            }
            return Ok(count);

        }

        //GET fantasy results for playerId, season
        [HttpGet("points/{playerId}/{season}")]
        [ProducesResponseType(typeof(FantasyPoints), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<FantasyPoints>> GetFantasyPoints(int playerId, int season)
        {
            return Ok(await _fantasyService.GetFantasyPoints(playerId, season));
        }
    }
}
