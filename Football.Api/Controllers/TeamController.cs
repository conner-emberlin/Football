using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly IPlayersService _playersService;

        public TeamController(IPlayersService playersService)
        {
            _playersService = playersService;
        }

        [HttpGet("teams")]
        [ProducesResponseType(typeof(List<TeamMap>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<Player>>> GetAllTeams() => Ok(await _playersService.GetAllTeams());

        [HttpGet("players/{team}")]
        [ProducesResponseType(typeof(List<PlayerTeam>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<Player>>> GetPlayersByTeam(string team) => Ok(await _playersService.GetPlayersByTeam(team));
    }
}
