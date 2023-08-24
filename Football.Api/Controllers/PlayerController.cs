using Football.Api.Helpers;
using Football.Interfaces;
using Football.Models;
using Microsoft.AspNetCore.Mvc;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpGet("{playerId}")]
        [ProducesResponseType(typeof(Player), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<Player>> GetPlayer(int playerId)
        {
            if(playerId != 0){
                return Ok(await _playerService.GetPlayer(playerId));
            }
            else
            {
                return BadRequest("No player with that name");
            }
        }
        [HttpGet("name/{name}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> GetPlayerId(string name)
        {
            return Ok(await _playerService.GetPlayerId(name));
        }

        [HttpGet("games/{playerId}/{season}")]
        [ProducesResponseType(typeof(FantasySeasonGames), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<FantasySeasonGames>> GetFantasySeasonGames(int playerId, int season)
        {
            var all = await _playerService.GetFantasySeasonGames(playerId);
            return Ok(all.Where(f => f.Season == season).FirstOrDefault());
        }

        [HttpPost("stats/add/")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> AddPassingStat([FromBody]PassingStatisticWithSeason pass)
        {
            return (Ok(await _playerService.AddPassingStat(pass)));
        }

        [HttpPost("stats/update/")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UpdatePassingStats([FromBody] List<PassingStatisticWithSeason> passes)
        {
            var playerId = await _playerService.GetPlayerId(passes.First().Name);
            await _playerService.DeletePassingStats(playerId);
            int count = 0;
            foreach(var pass in passes)
            {                              
                count += await _playerService.AddPassingStat(pass);
            }
            return Ok(count);
        }
    }
}
