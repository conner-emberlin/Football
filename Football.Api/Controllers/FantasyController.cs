using Football.Models;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FantasyController : ControllerBase
    {
        private readonly IFantasyDataService _fantasyDataService;
        private readonly IMatchupAnalysisService _matchupAnalysisService;
        private readonly Season _season;

        public FantasyController(IFantasyDataService fantasyDataService, IMatchupAnalysisService matchupAnalysisService, IOptionsMonitor<Season> season)
        {
            _fantasyDataService = fantasyDataService;
            _matchupAnalysisService = matchupAnalysisService;
            _season = season.CurrentValue;
        }

        [HttpPost("data/{position}/{season}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> PostSeasonFantasy( int season, string position)
        {
            if (season > 0 && position != null)
            {
                return Ok(await _fantasyDataService.PostSeasonFantasy(season, position.Trim().ToUpper()));
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }
        [HttpPost("data/{position}/{season}/{week}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> PostWeeklyFantasy(int season, int week, string position)
        {
            if (season > 0 && week > 0 && position != null)
            {
                return Ok(await _fantasyDataService.PostWeeklyFantasy(season, week, position.Trim().ToUpper()));
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }


        [HttpGet("data/season/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonFantasy>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonFantasy>>> GetSeasonFantasy(int playerId)
        {
            return playerId > 0 ? Ok(await _fantasyDataService.GetSeasonFantasy(playerId))
                            : BadRequest("Bad Request");
        }
        [HttpGet("data/weekly/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyFantasy>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyFantasy>>> GetWeeklyFantasy(int playerId)
        {
            return playerId > 0 ? Ok(await _fantasyDataService.GetWeeklyFantasy(playerId))
                : BadRequest("Bad Request");

        }
        [HttpGet("data/weekly/leaders/{week}")]
        [ProducesResponseType(typeof(List<WeeklyFantasy>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyFantasy>>> GetWeeklyFantasyLeaders(int week)
        {
            return week > 0 ? Ok(await _fantasyDataService.GetWeeklyFantasy(_season.CurrentSeason, week))
                        : BadRequest("Bad Request");
        }

        [HttpGet("season-totals")]
        [ProducesResponseType(typeof(List<SeasonFantasy>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonFantasy>>> GetCurrentFantasyTotals() => Ok(await _fantasyDataService.GetCurrentFantasyTotals(_season.CurrentSeason));

        [HttpGet("matchup-rankings")]
        [ProducesResponseType(typeof(List<MatchupRanking>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<MatchupRanking>> GetMatchupRankings(string position) => position != null ? 
            Ok(await _matchupAnalysisService.PositionalMatchupRankings(position)) : BadRequest("Bad Request");



    }
}
