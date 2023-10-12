using Microsoft.AspNetCore.Mvc;
using Football.Fantasy.MockDraft.Interfaces;
using Football.Fantasy.MockDraft.Models;
using Football.Data.Models;
using Football.Enums;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MockDraftController : ControllerBase
    {
        private readonly IMockDraftService _mockDraftService;

        public MockDraftController(IMockDraftService mockDraftService)
        {
            _mockDraftService = mockDraftService;
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(Mock), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<Mock>> CreateMockDraft([FromBody] MockDraftParameters param) => Ok(await _mockDraftService.CreateMockDraft(param));

        [HttpPost("available-players")]
        [ProducesResponseType(typeof(List<SeasonADP>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonADP>>> GetAvailablePlayers([FromBody] Mock mock) => Ok(await _mockDraftService.GetAvailablePlayers(mock));

        [HttpPost("team-needs")]
        [ProducesResponseType(typeof(List<PositionEnum>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<PositionEnum>>> GetTeamNeeds([FromBody] FantasyTeam team) => Ok(await _mockDraftService.GetTeamNeeds(team));

        [HttpPost("choose-player")]
        [ProducesResponseType(typeof(SeasonADP), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<SeasonADP>> ChoosePlayer([FromBody] List<SeasonADP> players) => Ok(await _mockDraftService.ChoosePlayer(players));

        [HttpPost("add-player")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> AddPlayerToTeam([FromBody] MockDraftResults result) => Ok(await _mockDraftService.AddPlayerToTeam(result));
    }
}
