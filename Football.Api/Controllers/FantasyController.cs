using Football.Models;
using Football.Enums;
using Football.Data.Models;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Football.Players.Interfaces;
using Football.Fantasy.Analysis.Interfaces;
using Football.Fantasy.Analysis.Models;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FantasyController : ControllerBase
    {
        private readonly IFantasyDataService _fantasyDataService;
        private readonly IMatchupAnalysisService _matchupAnalysisService;
        private readonly IMarketShareService _marketShareService;
        private readonly IStartOrSitService _startOrSitService;
        private readonly IWaiverWireService _waiverWireService;
        private readonly IPlayersService _playersService;
        private readonly Season _season;

        public FantasyController(IFantasyDataService fantasyDataService, IMatchupAnalysisService matchupAnalysisService, IMarketShareService marketShareService,
            IOptionsMonitor<Season> season, IStartOrSitService startOrSitService, IWaiverWireService waiverWireService, IPlayersService playersService)
        {
            _fantasyDataService = fantasyDataService;
            _matchupAnalysisService = matchupAnalysisService;
            _marketShareService = marketShareService;
            _season = season.CurrentValue;
            _startOrSitService = startOrSitService;
            _waiverWireService = waiverWireService;
            _playersService = playersService;
        }

        [HttpPost("data/{position}/{season}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> PostSeasonFantasy( int season, string position)
        {
            if (season > 0 && Enum.TryParse(position.Trim().ToUpper(), out PositionEnum positionEnum))
            {
                return Ok(await _fantasyDataService.PostSeasonFantasy(season, positionEnum));
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
            if (season > 0 && week > 0 && Enum.TryParse(position.Trim().ToUpper(), out PositionEnum positionEnum))
            {
                return Ok(await _fantasyDataService.PostWeeklyFantasy(season, week, positionEnum));
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
        public async Task<ActionResult<List<WeeklyFantasy>>> GetWeeklyFantasyLeaders(int week) =>  week > 0 ? Ok(await _fantasyDataService.GetWeeklyFantasy(_season.CurrentSeason, week))
                        : BadRequest("Bad Request");
        
        [HttpGet("season-totals")]
        [ProducesResponseType(typeof(List<SeasonFantasy>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonFantasy>>> GetCurrentFantasyTotals() => Ok(await _fantasyDataService.GetCurrentFantasyTotals(_season.CurrentSeason));

        [HttpGet("matchup-rankings/{position}")]
        [ProducesResponseType(typeof(List<MatchupRanking>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<MatchupRanking>> GetMatchupRankings(string position) => Enum.TryParse(position.Trim().ToUpper(), out PositionEnum positionEnum) ? 
            Ok(await _matchupAnalysisService.PositionalMatchupRankings(positionEnum)) : BadRequest("Bad Request");

        [HttpGet("team-totals")]
        [ProducesResponseType(typeof(List<TeamTotals>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<TeamTotals>>> GetTeamTotals() => Ok(await _marketShareService.GetTeamTotals());

        [HttpGet("marketshare/{position}")]
        [ProducesResponseType(typeof(List<TeamTotals>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<MarketShare>>> GetMarketShare(string position) => Enum.TryParse(position.Trim().ToUpper(), out PositionEnum positionEnum) ?
            Ok(await _marketShareService.GetMarketShare(positionEnum)) : BadRequest("Bad Request");

        [HttpGet("targetshares")]
        [ProducesResponseType(typeof(TargetShare), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<TargetShare>> GetTargetShares() =>  Ok(await _marketShareService.GetTargetShares());

        [HttpGet("weather/{playerId}")]
        [ProducesResponseType(typeof(Weather), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<Weather>> GetWeather(int playerId) => Ok(await _startOrSitService.GetWeather(playerId));

        [HttpGet("lines/{playerId}")]
        [ProducesResponseType(typeof(MatchLines), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<MatchLines>> GetMatchLines(int playerId) => Ok(await _startOrSitService.GetMatchLines(playerId));

        [HttpPost("start-or-sit")]
        [ProducesResponseType(typeof(List<StartOrSit>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<StartOrSit>>> GetStartOrSit([FromBody] List<int> playerIds) => Ok(await _startOrSitService.GetStartOrSits(playerIds));

        [HttpGet("waiver-wire")]
        [ProducesResponseType(typeof(List<WeeklyRosterPercent>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyRosterPercent>>> GetWaiverWireCandidates() 
        {
            var currentWeek = await _playersService.GetCurrentWeek(_season.CurrentSeason);
            return Ok(await _waiverWireService.GetWaiverWireCandidates(_season.CurrentSeason, currentWeek));
        }
    }
}
