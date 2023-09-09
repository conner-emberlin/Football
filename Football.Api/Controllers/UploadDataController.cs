﻿using Football.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadDataController : ControllerBase
    {
        private readonly IUploadWeeklyDataService _weeklyDataService;
        private readonly IUploadSeasonDataService _seasonDataService;
        private readonly IScraperService _scraperService;
        public UploadDataController(IUploadWeeklyDataService weeklyDataService, IUploadSeasonDataService seasonDataService, IScraperService scraperService)
        {
            _weeklyDataService = weeklyDataService;
            _seasonDataService = seasonDataService;
            _scraperService = scraperService;
        }

        [HttpPost("{position}/{week}/{season}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadWeeklyData(int position, int season, int week)
        {
            return position switch
            {
                1 => Ok(await _weeklyDataService.UploadWeeklyQBData(season, week)),
                2 => Ok(await _weeklyDataService.UploadWeeklyRBData(season, week)),
                3 => Ok(await _weeklyDataService.UploadWeeklyWRData(season, week)),
                4 => Ok(await _weeklyDataService.UploadWeeklyTEData(season, week)),
                5 => Ok(await _weeklyDataService.UploadWeeklyDSTData(season, week)),
                _ => BadRequest(),
            };
        }
        [HttpPost("{position}/{season}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadSeasonData(int position, int season)
        {
            return position switch
            {
                1 => Ok(await _seasonDataService.UploadSeasonQBData(season)),
                2 => Ok(await _seasonDataService.UploadSeasonRBData(season)),
                3 => Ok(await _seasonDataService.UploadSeasonWRData(season)),
                4 => Ok(await _seasonDataService.UploadSeasonTEData(season)),
                5 => Ok(await _seasonDataService.UploadSeasonDSTData(season)),
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

    }
}
