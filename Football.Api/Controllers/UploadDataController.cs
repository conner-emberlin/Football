using Football.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadDataController : ControllerBase
    {
        private readonly IUploadWeeklyDataService _weeklyDataService;
        public UploadDataController(IUploadWeeklyDataService weeklyDataService)
        {
            _weeklyDataService = weeklyDataService;
        }

        [HttpPost("weekly/{position}/{week}/{season}")]
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
    }
}
