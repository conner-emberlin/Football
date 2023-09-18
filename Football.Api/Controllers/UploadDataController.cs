using Football.Data.Interfaces;
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
        private readonly Season _season;
        public UploadDataController(IUploadWeeklyDataService weeklyDataService, IUploadSeasonDataService seasonDataService, 
            IScraperService scraperService, IOptionsMonitor<Season> season)
        {
            _weeklyDataService = weeklyDataService;
            _seasonDataService = seasonDataService;
            _scraperService = scraperService;
            _season = season.CurrentValue;
        }

        [HttpPost("{position}/{season}/{week}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadWeeklyData(string position, int season, int week)
        {
            return position.Trim().ToLower() switch
            {
                "qb" => Ok(await _weeklyDataService.UploadWeeklyQBData(season, week)),
                "rb" => Ok(await _weeklyDataService.UploadWeeklyRBData(season, week)),
                "wr" => Ok(await _weeklyDataService.UploadWeeklyWRData(season, week)),
                "te" => Ok(await _weeklyDataService.UploadWeeklyTEData(season, week)),
                "dst" => Ok(await _weeklyDataService.UploadWeeklyDSTData(season, week)),
                _ => BadRequest(),
            };
        }
        [HttpPost("{position}/{season}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadSeasonData(string position, int season)
        {
            return position.Trim().ToLower() switch
            {
                "qb" => Ok(await _seasonDataService.UploadSeasonQBData(season)),
                "rb"=> Ok(await _seasonDataService.UploadSeasonRBData(season)),
                "wr" => Ok(await _seasonDataService.UploadSeasonWRData(season)),
                "te" => Ok(await _seasonDataService.UploadSeasonTEData(season)),
                "dst" => Ok(await _seasonDataService.UploadSeasonDSTData(season)),
                _ => BadRequest(),
            };
        }

        [HttpPost("headshots/{position}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> DownloadHeadShots(string position)
        {
            return Ok(await _scraperService.DownloadHeadShots(position));
        }
        [HttpPost("logos")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> DownloadTeamLogos()
        {
            return Ok(await _scraperService.DownloadTeamLogos());
        }

        [HttpPost("teams/{position}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadCurrentTeams(string position)
        {
            if (!string.IsNullOrEmpty(position))
            {
                return Ok(await _seasonDataService.UploadCurrentTeams(_season.CurrentSeason, position));
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }

        [HttpPost("schedule")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadSchedule()
        {
            return Ok(await _seasonDataService.UploadSchedule(_season.CurrentSeason));
        }
    }
}
