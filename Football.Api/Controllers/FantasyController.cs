using Football.Interfaces;
using Football.Models;
using Microsoft.AspNetCore.Mvc;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FantasyController : ControllerBase
    {
        private readonly IFantasyService _fantasyService;
        private readonly IPlayerService _playerService;
        public FantasyController(IFantasyService fantasyService, IPlayerService playerService)
        {
            _fantasyService = fantasyService;
            _playerService = playerService;
        }
        //POST fantasy results for a season/position (delete existing ones for the season/position)
        [HttpPost("refresh/{name}/{season}")]
        [ProducesResponseType(typeof(double), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> RefreshFantasyPoints(string name, int season)
        {
            var playerId = await _playerService.GetPlayerId(name);
            var fantasyPoints = await _fantasyService.GetFantasyPoints(playerId, season);
            return Ok(await _fantasyService.RefreshFantasyResults(fantasyPoints));
        }
        //Use this for a complete refresh of a season. Delete season from table first.
        [HttpPost("{position}/{season}")]
        [ProducesResponseType(typeof(double), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> PostFantasyPoints(string position, int season)
        {
            position = position == "WRTE" ? "WR/TE" : position;
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
        [HttpGet("points/{name}/{season}")]
        [ProducesResponseType(typeof(List<FantasyPoints>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<FantasyPointsWithName>>> GetFantasyPoints(string name, int season)
        {
            var playerId = await _playerService.GetPlayerId(name);
            if(playerId > 0)
            {
                var player = await _playerService.GetPlayer(playerId);
                var points = player.FantasyPoints?.Select(fp => new FantasyPointsWithName
                    {
                        Season = fp.Season,
                        PlayerId = fp.PlayerId,
                        TotalPoints = fp.TotalPoints,
                        PassingPoints = fp.PassingPoints,
                        RushingPoints = fp.RushingPoints,
                        ReceivingPoints = fp.ReceivingPoints,
                        Name = player.Name
                    }).ToList();
                return season == 0 ? Ok(points.OrderBy(p => p.Season)) : Ok(points.Where(p => p.Season == season));
            }
            else
            {
                return BadRequest("Player Name cannot be empty/could not be found.");                
            }
        }
    }
}
