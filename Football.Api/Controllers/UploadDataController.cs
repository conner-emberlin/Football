using Football.Enums;
using Football.Models;
using Football.Data.Interfaces;
using Football.Leagues.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadDataController(IUploadWeeklyDataService weeklyDataService, IUploadSeasonDataService seasonDataService,
        IScraperService scraperService, IOptionsMonitor<Season> season, ILeagueAnalysisService leagueAnalysisService) : ControllerBase
    {
        private readonly Season _season = season.CurrentValue;

        [HttpPost("{position}/{season}/{week}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadWeeklyData(string position, int season, int week)
        {
            if (Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum))
            {
                return positionEnum switch
                {
                    Position.QB => Ok(await weeklyDataService.UploadWeeklyQBData(season, week)),
                    Position.RB => Ok(await weeklyDataService.UploadWeeklyRBData(season, week)),
                    Position.WR => Ok(await weeklyDataService.UploadWeeklyWRData(season, week)),
                    Position.TE => Ok(await weeklyDataService.UploadWeeklyTEData(season, week)),
                    Position.DST => Ok(await weeklyDataService.UploadWeeklyDSTData(season, week)),
                    Position.K => Ok(await weeklyDataService.UploadWeeklyKData(season, week)),
                    _ => BadRequest()
                };
            }
            return BadRequest();
        }

        [HttpPost("{position}/{season}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadSeasonData(string position, int season)
        {
            if (Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum))
            {
                return positionEnum switch
                {
                    Position.QB => Ok(await seasonDataService.UploadSeasonQBData(season)),
                    Position.RB => Ok(await seasonDataService.UploadSeasonRBData(season)),
                    Position.WR => Ok(await seasonDataService.UploadSeasonWRData(season)),
                    Position.TE => Ok(await seasonDataService.UploadSeasonTEData(season)),
                    Position.DST => Ok(await seasonDataService.UploadSeasonDSTData(season)),
                    _ => BadRequest(),
                };
            }
            return BadRequest();
        }

        [HttpPost("roster-percent/{position}/{season}/{week}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadWeeklyRosterPercentages(string position, int season, int week)
        {
            if (!string.IsNullOrWhiteSpace(position) && season > 0 && week > 0)
                return Ok(await weeklyDataService.UploadWeeklyRosterPercentages(season, week, position));
            return BadRequest();
        }

        [HttpPost("headshots/{position}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> DownloadHeadShots(string position) => Ok(await scraperService.DownloadHeadShots(position));

        [HttpPost("logos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> DownloadTeamLogos() => Ok(await scraperService.DownloadTeamLogos());
  
        [HttpPost("teams/{position}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadCurrentTeams(string position) => !string.IsNullOrEmpty(position) ?
            Ok(await seasonDataService.UploadCurrentTeams(_season.CurrentSeason, position)) : BadRequest();

        [HttpPost("schedule")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> UploadSchedule() => Ok(await seasonDataService.UploadSchedule(_season.CurrentSeason));

        [HttpPost("schedule-details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> UploadScheduleDetails() => Ok(await seasonDataService.UploadScheduleDetails(_season.CurrentSeason));

        [HttpPost("game-results/{season}/{week}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadWeeklyGameResults(int season, int week) => season > 0 && week > 0 ? Ok(await weeklyDataService.UploadWeeklyGameResults(season, week)) : BadRequest();

        [HttpPost("adp/{position}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadADP(string position) => Ok(await seasonDataService.UploadADP(_season.CurrentSeason, position));

        [HttpPost("sleeper-map")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> UploadSleeperPlayerMap() => Ok(await leagueAnalysisService.UploadSleeperPlayerMap());

        [HttpPost("snaps/{position}/{season}/{week}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadWeeklySnapCounts(string position, int season, int week) => Ok(await weeklyDataService.UploadWeeklySnapCounts(season, week, position));

    }
}
