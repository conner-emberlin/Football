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
    public class PlayerController : ControllerBase
    {
        private readonly IFantasyService _fantasyService;
        private readonly IPlayerService _playerService;
        public readonly IServiceHelper _serviceHelper;
        public PlayerController(IFantasyService fantasyService, IPlayerService playerService, IServiceHelper serviceHelper)
        {
            _fantasyService = fantasyService;
            _playerService = playerService;
            _serviceHelper = serviceHelper;
        }

        [HttpGet("{playerId}")]
        [ProducesResponseType(typeof(Player), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<Player>> GetPlayer(int playerId)
        {
            return Ok(await _playerService.GetPlayer(playerId));
        }
    }
}
