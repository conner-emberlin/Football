using Football.Data.Interfaces;
using Football.Enums;
using Football.LeagueAnalysis.Interfaces;
using Football.LeagueAnalysis.Models;
using Football.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadDataController : ControllerBase
    {
        private readonly IUploadWeeklyDataService _weeklyDataService;
        private readonly IUploadSeasonDataService _seasonDataService;
        private readonly IScraperService _scraperService;
        private readonly ILeagueAnalysisService _leagueAnalysisService;
        private readonly Season _season;
        public UploadDataController(IUploadWeeklyDataService weeklyDataService, IUploadSeasonDataService seasonDataService, 
            IScraperService scraperService, IOptionsMonitor<Season> season, ILeagueAnalysisService leagueAnalysisService)
        {
            _weeklyDataService = weeklyDataService;
            _seasonDataService = seasonDataService;
            _scraperService = scraperService;
            _season = season.CurrentValue;
            _leagueAnalysisService = leagueAnalysisService;
        }

        [HttpPost("{position}/{season}/{week}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadWeeklyData(string position, int season, int week)
        {
            if (Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum))
            {
                return positionEnum switch
                {
                    Position.QB => Ok(await _weeklyDataService.UploadWeeklyQBData(season, week)),
                    Position.RB => Ok(await _weeklyDataService.UploadWeeklyRBData(season, week)),
                    Position.WR => Ok(await _weeklyDataService.UploadWeeklyWRData(season, week)),
                    Position.TE => Ok(await _weeklyDataService.UploadWeeklyTEData(season, week)),
                    Position.DST => Ok(await _weeklyDataService.UploadWeeklyDSTData(season, week)),
                    _ => BadRequest()
                };
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }
        [HttpPost("roster-percent/{position}/{season}/{week}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadWeeklyRosterPercentages(string position, int season, int week)
        {
            if (!string.IsNullOrWhiteSpace(position) && season > 0 && week > 0)
            {
                return Ok(await _weeklyDataService.UploadWeeklyRosterPercentages(season, week, position));
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }

        [HttpPost("{position}/{season}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadSeasonData(string position, int season)
        {
            if (Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum))
            {
                return positionEnum switch
                {
                    Position.QB => Ok(await _seasonDataService.UploadSeasonQBData(season)),
                    Position.RB => Ok(await _seasonDataService.UploadSeasonRBData(season)),
                    Position.WR => Ok(await _seasonDataService.UploadSeasonWRData(season)),
                    Position.TE => Ok(await _seasonDataService.UploadSeasonTEData(season)),
                    Position.DST => Ok(await _seasonDataService.UploadSeasonDSTData(season)),
                    _ => BadRequest(),
                };
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }

        [HttpPost("headshots/{position}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> DownloadHeadShots(string position) => Ok(await _scraperService.DownloadHeadShots(position));

        [HttpPost("logos")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> DownloadTeamLogos() => Ok(await _scraperService.DownloadTeamLogos());
  
        [HttpPost("teams/{position}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadCurrentTeams(string position) => !string.IsNullOrEmpty(position) ?
            Ok(await _seasonDataService.UploadCurrentTeams(_season.CurrentSeason, position)) : BadRequest("Bad Request");

        [HttpPost("schedule")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadSchedule() => Ok(await _seasonDataService.UploadSchedule(_season.CurrentSeason));

        [HttpPost("schedule-details")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadScheduleDetails() => Ok(await _seasonDataService.UploadScheduleDetails(_season.CurrentSeason));

        [HttpPost("game-results/{season}/{week}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadWeeklyGameResults(int season, int week) => Ok(await _weeklyDataService.UploadWeeklyGameResults(season, week));

        [HttpPost("adp/{position}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> ScrapeADP(string position) => Ok(await _seasonDataService.UploadADP(_season.CurrentSeason, position));

        [HttpPost("sleeper-map")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadSleeperPlayerMap() => Ok(await _leagueAnalysisService.UploadSleeperPlayerMap());
    }
}
