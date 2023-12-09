using Football.Data.Interfaces;
using Football.Enums;
using Football.Leagues.Interfaces;
using Football.Leagues.Models;
using Football.Models;
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
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadWeeklyData(string position, int season, int week)
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
                return Ok(await weeklyDataService.UploadWeeklyRosterPercentages(season, week, position));
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
                    Position.QB => Ok(await seasonDataService.UploadSeasonQBData(season)),
                    Position.RB => Ok(await seasonDataService.UploadSeasonRBData(season)),
                    Position.WR => Ok(await seasonDataService.UploadSeasonWRData(season)),
                    Position.TE => Ok(await seasonDataService.UploadSeasonTEData(season)),
                    Position.DST => Ok(await seasonDataService.UploadSeasonDSTData(season)),
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
        public async Task<ActionResult<int>> DownloadHeadShots(string position) => Ok(await scraperService.DownloadHeadShots(position));

        [HttpPost("logos")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> DownloadTeamLogos() => Ok(await scraperService.DownloadTeamLogos());
  
        [HttpPost("teams/{position}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadCurrentTeams(string position) => !string.IsNullOrEmpty(position) ?
            Ok(await seasonDataService.UploadCurrentTeams(_season.CurrentSeason, position)) : BadRequest("Bad Request");

        [HttpPost("schedule")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadSchedule() => Ok(await seasonDataService.UploadSchedule(_season.CurrentSeason));

        [HttpPost("schedule-details")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadScheduleDetails() => Ok(await seasonDataService.UploadScheduleDetails(_season.CurrentSeason));

        [HttpPost("game-results/{season}/{week}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadWeeklyGameResults(int season, int week) => Ok(await weeklyDataService.UploadWeeklyGameResults(season, week));

        [HttpPost("adp/{position}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> ScrapeADP(string position) => Ok(await seasonDataService.UploadADP(_season.CurrentSeason, position));

        [HttpPost("sleeper-map")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadSleeperPlayerMap() => Ok(await leagueAnalysisService.UploadSleeperPlayerMap());
    }
}
