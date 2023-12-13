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
using Football.Leagues.Interfaces;
using Football.Leagues.Models;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FantasyController(IFantasyDataService fantasyDataService, IMatchupAnalysisService matchupAnalysisService, IMarketShareService marketShareService,
        IOptionsMonitor<Season> season, IStartOrSitService startOrSitService, IWaiverWireService waiverWireService,
        IPlayersService playersService, IFantasyAnalysisService boomBustService, ILeagueAnalysisService leagueService, ISnapCountService snapCountService) : ControllerBase
    {
        private readonly Season _season = season.CurrentValue;

        [HttpPost("data/{position}/{season}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> PostSeasonFantasy(int season, string position)
        {
            if (season > 0 && Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum))
                return Ok(await fantasyDataService.PostSeasonFantasy(season, positionEnum));
            else return BadRequest();
        }
        [HttpPost("data/{position}/{season}/{week}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> PostWeeklyFantasy(int season, int week, string position)
        {
            if (season > 0 && week > 0 && Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum))
            {
                return Ok(await fantasyDataService.PostWeeklyFantasy(season, week, positionEnum));
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
            return playerId > 0 ? Ok(await fantasyDataService.GetSeasonFantasy(playerId))
                            : BadRequest("Bad Request");
        }
        [HttpGet("data/weekly/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyFantasy>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyFantasy>>> GetWeeklyFantasy(int playerId)
        {
            return playerId > 0 ? Ok(await fantasyDataService.GetWeeklyFantasy(playerId))
                : BadRequest("Bad Request");

        }
        [HttpGet("data/weekly/leaders/{week}")]
        [ProducesResponseType(typeof(List<WeeklyFantasy>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WeeklyFantasy>>> GetWeeklyFantasyLeaders(int week) =>  week > 0 ? Ok(await fantasyDataService.GetWeeklyFantasy(_season.CurrentSeason, week))
                        : BadRequest("Bad Request");
        
        [HttpGet("season-totals")]
        [ProducesResponseType(typeof(List<SeasonFantasy>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<SeasonFantasy>>> GetCurrentFantasyTotals() => Ok(await fantasyDataService.GetCurrentFantasyTotals(_season.CurrentSeason));

        [HttpGet("matchup-rankings/{position}")]
        [ProducesResponseType(typeof(List<MatchupRanking>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<MatchupRanking>> GetMatchupRankings(string position) => Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum) ? 
            Ok(await matchupAnalysisService.PositionalMatchupRankings(positionEnum)) : BadRequest("Bad Request");

        [HttpGet("team-totals")]
        [ProducesResponseType(typeof(List<TeamTotals>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<TeamTotals>>> GetTeamTotals() => Ok(await marketShareService.GetTeamTotals());

        [HttpGet("marketshare/{position}")]
        [ProducesResponseType(typeof(List<TeamTotals>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<MarketShare>>> GetMarketShare(string position) => Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum) ?
            Ok(await marketShareService.GetMarketShare(positionEnum)) : BadRequest("Bad Request");

        [HttpGet("targetshares")]
        [ProducesResponseType(typeof(TargetShare), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<TargetShare>> GetTargetShares() =>  Ok(await marketShareService.GetTargetShares());

        [HttpGet("weather/{playerId}")]
        [ProducesResponseType(typeof(Weather), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<Weather>> GetWeather(int playerId) => Ok(await startOrSitService.GetWeather(playerId));

        [HttpGet("lines/{playerId}")]
        [ProducesResponseType(typeof(MatchLines), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<MatchLines>> GetMatchLines(int playerId) => Ok(await startOrSitService.GetMatchLines(playerId));

        [HttpPost("start-or-sit")]
        [ProducesResponseType(typeof(List<StartOrSit>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<StartOrSit>>> GetStartOrSit([FromBody] List<int> playerIds) => Ok(await startOrSitService.GetStartOrSits(playerIds));

        [HttpGet("waiver-wire")]
        [ProducesResponseType(typeof(List<WaiverWireCandidate>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<WaiverWireCandidate>>> GetWaiverWireCandidates() 
        {
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            return Ok(await waiverWireService.GetWaiverWireCandidates(_season.CurrentSeason, currentWeek));
        }

        [HttpGet("boom-busts/{position}")]
        [ProducesResponseType(typeof(List<BoomBust>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<BoomBust>>> GetBoomBusts([FromRoute] string position) => Enum.TryParse(position, out Position posEnum) ? Ok(await boomBustService.GetBoomBusts(posEnum)) : BadRequest();

        [HttpGet("weekly-boom-busts/{playerId}")]
        [ProducesResponseType(typeof(List<BoomBustByWeek>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<BoomBustByWeek>> GetBoomBustByWeek([FromRoute] int playerId) => playerId > 0 ? Ok(await boomBustService.GetBoomBustsByWeek(playerId)) : BadRequest();


        [HttpGet("fantasy-performance/{position}")]
        [ProducesResponseType(typeof(List<FantasyPerformance>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<FantasyPerformance>>> GetFantasyPerformances([FromRoute] string position) => Enum.TryParse(position, out Position posEnum) ? Ok(await boomBustService.GetFantasyPerformances(posEnum)) : BadRequest();

        [HttpGet("shares/{position}")]
        [ProducesResponseType(typeof(List<FantasyPerformance>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<FantasyPerformance>>> GetFantasyPercentages([FromRoute] string position) => Enum.TryParse(position, out Position posEnum) ? Ok(await boomBustService.GetFantasyPercentages(posEnum)) : BadRequest();

        [HttpGet("trending-players")]
        [ProducesResponseType(typeof(List<TrendingPlayer>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetTrendingPlayers() => Ok(await leagueService.GetTrendingPlayers());

        [HttpGet("snap-analysis/{position}")]
        [ProducesResponseType(typeof(List<SnapCountAnalysis>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetSnapCountAnalysis([FromRoute] string position) => Enum.TryParse(position, out Position posEnum) ? Ok(await snapCountService.GetSnapCountAnalysis(posEnum, _season.CurrentSeason)) : BadRequest();
    }
}
