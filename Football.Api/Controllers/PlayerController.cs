using Football.Api.Helpers;
using Football.Interfaces;
using Football.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        private readonly IServiceHelper _serviceHelper;
        public PlayerController(IPlayerService playerService, IServiceHelper serviceHelper)
        {
            _playerService = playerService;
            _serviceHelper = serviceHelper;
        }

        [HttpGet("{name}")]
        [ProducesResponseType(typeof(Player), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<Player>> GetPlayer(string name)
        {
            var playerId = await _playerService.GetPlayerId(name);
            if(playerId != 0){
                return Ok(await _playerService.GetPlayer(playerId));
            }
            else
            {
                return BadRequest("No player with that name");
            }
        }
        [HttpGet("games/{playerId}/{season}")]
        [ProducesResponseType(typeof(FantasySeasonGames), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<FantasySeasonGames>> GetFantasySeasonGames(int playerId, int season)
        {
            var all = await _playerService.GetFantasySeasonGames(playerId);
            return Ok(all.Where(f => f.Season == season).FirstOrDefault());
        }
    }
}
