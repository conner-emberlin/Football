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
            return playerId > 0 ? Ok(await _playerService.GetPlayer(playerId)) 
                                : BadRequest("No player with that name");
        }
        [HttpGet("id/{name}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> GetPlayerId(string name)
        {
            return Ok(await _playerService.GetPlayerId(name));
        }

        [HttpGet("name/{id}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<string>> GetPlayerName(int id)
        {
            return Ok(await _playerService.GetPlayerName(id));
        }

        [HttpGet("games/{playerId}/{season}")]
        [ProducesResponseType(typeof(FantasySeasonGames), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<FantasySeasonGames>> GetFantasySeasonGames(int playerId, int season)
        {
            var all = await _playerService.GetFantasySeasonGames(playerId);
            return Ok(all.Where(f => f.Season == season).FirstOrDefault());
        }

        [HttpPost("stats/pass/add/")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> AddPassingStat([FromBody]PassingStatisticWithSeason pass)
        {
            return (Ok(await _playerService.AddPassingStat(pass)));
        }

        [HttpPost("stats/pass/update/")]
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
        [HttpPost("stats/rush/add/")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> AddRushingStat([FromBody] RushingStatisticWithSeason rush)
        {
            return Ok(await _playerService.AddRushingStat(rush));
        }

        [HttpPost("stats/rush/update/")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UpdateRushingStats([FromBody] List<RushingStatisticWithSeason> rushes)
        {
            var playerId = await _playerService.GetPlayerId(rushes.First().Name);
            await _playerService.DeleteRushingStats(playerId);
            int count = 0;
            foreach (var rush in rushes)
            {
                count += await _playerService.AddRushingStat(rush);
            }
            return Ok(count);
        }
        [HttpPost("stats/rec/add/")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> AddReceivingStat([FromBody] ReceivingStatisticWithSeason rec)
        {
            return (Ok(await _playerService.AddReceivingStat(rec)));
        }

        [HttpPost("stats/rec/update/")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UpdateReceivingStats([FromBody] List<ReceivingStatisticWithSeason> recs)
        {
            var playerId = await _playerService.GetPlayerId(recs.First().Name);
            await _playerService.DeleteReceivingStats(playerId);
            int count = 0;
            foreach (var rec in recs)
            {
                count += await _playerService.AddReceivingStat(rec);
            }
            return Ok(count);
        }

        [HttpPost("create/{name}/{position}/{active}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> CreatePlayer(string name, string position, int active)
        {
            position = position == "WR" || position == "TE" ? "WR/TE" : position;
            return Ok(await _playerService.CreatePlayer(name, position, active));
        }
    }
}
