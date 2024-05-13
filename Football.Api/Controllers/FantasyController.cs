using Football.Models;
using Football.Enums;
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
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostSeasonFantasy(int season, string position)
        {
            if (season > 0 && Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum))
                return Ok(await fantasyDataService.PostSeasonFantasy(season, positionEnum));
            return BadRequest();
        }
        [HttpPost("data/{position}/{season}/{week}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostWeeklyFantasy(int season, int week, string position)
        {
            if (season > 0 && week > 0 && Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum))
                return Ok(await fantasyDataService.PostWeeklyFantasy(season, week, positionEnum));
            return BadRequest();
        }

        [HttpGet("data/season/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonFantasy>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSeasonFantasy(int playerId) => playerId > 0 ? Ok(await fantasyDataService.GetSeasonFantasy(playerId)) : BadRequest();

        [HttpGet("data/weekly/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyFantasy>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyFantasy(int playerId) => playerId > 0 ? Ok(await fantasyDataService.GetWeeklyFantasy(playerId)) : BadRequest();

        [HttpGet("data/weekly/leaders/{week}")]
        [ProducesResponseType(typeof(List<WeeklyFantasy>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyFantasyLeaders(int week) => week > 0 ? Ok(await fantasyDataService.GetWeeklyFantasy(_season.CurrentSeason, week)) : BadRequest();
                                
        [HttpGet("season-totals")]
        [ProducesResponseType(typeof(List<SeasonFantasy>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrentFantasyTotals() => Ok(await fantasyDataService.GetCurrentFantasyTotals(_season.CurrentSeason));

        [HttpGet("matchup-rankings/{position}")]
        [ProducesResponseType(typeof(List<MatchupRanking>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMatchupRankings(string position) => Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum) ? Ok(await matchupAnalysisService.PositionalMatchupRankings(positionEnum)) : BadRequest();

        [HttpGet("top-opponents/{teamId}/{position}")]
        [ProducesResponseType(typeof(List<WeeklyFantasy>), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTopOpponents(int teamId, string position) => Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum) ? Ok(await matchupAnalysisService.GetTopOpponents(positionEnum, teamId)) : BadRequest();

        [HttpGet("team-totals")]
        [ProducesResponseType(typeof(List<TeamTotals>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTeamTotals() => Ok(await marketShareService.GetTeamTotals());

        [HttpGet("marketshare/{position}")]
        [ProducesResponseType(typeof(List<TeamTotals>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMarketShare(string position) => Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum) ? Ok(await marketShareService.GetMarketShare(positionEnum)) : BadRequest();


        [HttpGet("targetshares")]
        [ProducesResponseType(typeof(TargetShare), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTargetShares() =>  Ok(await marketShareService.GetTargetShares());

        [HttpGet("weather/{playerId}")]
        [ProducesResponseType(typeof(Weather), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWeather(int playerId) => Ok(await startOrSitService.GetWeather(playerId));

        [HttpGet("lines/{playerId}")]
        [ProducesResponseType(typeof(MatchLines), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMatchLines(int playerId) => Ok(await startOrSitService.GetMatchLines(playerId));

        [HttpPost("start-or-sit")]
        [ProducesResponseType(typeof(List<StartOrSit>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStartOrSit([FromBody] List<int> playerIds) => Ok(await startOrSitService.GetStartOrSits(playerIds));

        [HttpGet("waiver-wire")]
        [ProducesResponseType(typeof(List<WaiverWireCandidate>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWaiverWireCandidates() 
        {
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            return Ok(await waiverWireService.GetWaiverWireCandidates(_season.CurrentSeason, currentWeek));
        }

        [HttpGet("boom-busts/{position}")]
        [ProducesResponseType(typeof(List<BoomBust>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetBoomBusts([FromRoute] string position) => Enum.TryParse(position, out Position posEnum) ? Ok(await boomBustService.GetBoomBusts(posEnum)) : BadRequest();

        [HttpGet("weekly-boom-busts/{playerId}")]
        [ProducesResponseType(typeof(List<BoomBustByWeek>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetBoomBustByWeek([FromRoute] int playerId) => playerId > 0 ? Ok(await boomBustService.GetBoomBustsByWeek(playerId)) : BadRequest();


        [HttpGet("fantasy-performance/{position}")]
        [ProducesResponseType(typeof(List<FantasyPerformance>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFantasyPerformances([FromRoute] string position) => Enum.TryParse(position, out Position posEnum) ? Ok(await boomBustService.GetFantasyPerformances(posEnum)) : BadRequest();

        [HttpGet("shares/{position}")]
        [ProducesResponseType(typeof(List<FantasyPerformance>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFantasyPercentages([FromRoute] string position) => Enum.TryParse(position, out Position posEnum) ? Ok(await boomBustService.GetFantasyPercentages(posEnum)) : BadRequest();

        [HttpGet("trending-players")]
        [ProducesResponseType(typeof(List<TrendingPlayer>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTrendingPlayers() => Ok(await leagueService.GetTrendingPlayers());

        [HttpGet("snap-analysis/{position}")]
        [ProducesResponseType(typeof(List<SnapCountAnalysis>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSnapCountAnalysis([FromRoute] string position) => Enum.TryParse(position, out Position posEnum) ? Ok(await snapCountService.GetSnapCountAnalysis(posEnum, _season.CurrentSeason)) : BadRequest();


    }
}
