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
        public async Task<ActionResult<int>> RefreshFantasyPoints(int playerId, int season)
        { 
            var fantasyPoints = await _fantasyService.GetFantasyPoints(playerId, season);
            return Ok(await _fantasyService.RefreshFantasyResults(fantasyPoints));

        }
        //Use this for a complete refresh of a season. Delete season from table first.
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
        [HttpGet("points/{name}/{season}")]
        [ProducesResponseType(typeof(List<FantasyPoints>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<FantasyPointsWithName>>> GetFantasyPoints(string name, int season)
        {
            var playerId = await _playerService.GetPlayerId(name);
            List<FantasyPointsWithName> points = new();
            if(playerId != 0)
            {
                var player = await _playerService.GetPlayer(playerId);
                foreach(var point in player.FantasyPoints)
                {
                    points.Add(new FantasyPointsWithName
                    {
                        Season = point.Season,
                        PlayerId = point.PlayerId,
                        TotalPoints = point.TotalPoints,
                        PassingPoints = point.PassingPoints,
                        RushingPoints = point.RushingPoints,
                        ReceivingPoints = point.ReceivingPoints,
                        Name = player.Name
                    });
                }
                if (season == 0)
                {
                    return Ok(points.OrderBy(p => p.Season));
                }
                else
                {
                    return Ok(points.Where(p => p.Season == season));
                }
            }
            else
            {
                return BadRequest("Player Name cannot be empty/could not be found.");                
            }
        }
    }
}
