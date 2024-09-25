using Football.Enums;
using Football.Models;
using Football.Data.Interfaces;
using Football.Players.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadDataController(IUploadWeeklyDataService weeklyDataService, IUploadSeasonDataService seasonDataService,
        IScraperService scraperService, IPlayersService playersService, IOptionsMonitor<Season> season) : ControllerBase
    {
        private readonly Season _season = season.CurrentValue;

        [HttpPost("{position}/{season}/{week}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadWeeklyData(string position, int season, int week)
        {
            if (Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum))
            {
                var ignoreList = await playersService.GetIgnoreList();
                return positionEnum switch
                {
                    Position.QB => Ok(await weeklyDataService.UploadWeeklyQBData(season, week, ignoreList)),
                    Position.RB => Ok(await weeklyDataService.UploadWeeklyRBData(season, week, ignoreList)),
                    Position.WR => Ok(await weeklyDataService.UploadWeeklyWRData(season, week, ignoreList)),
                    Position.TE => Ok(await weeklyDataService.UploadWeeklyTEData(season, week, ignoreList)),
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
        public async Task<ActionResult<int>> UploadCurrentTeams(string position) => Enum.TryParse(position, out Position pos) ?
            Ok(await seasonDataService.UploadCurrentTeams(_season.CurrentSeason, pos)) : BadRequest();

        [HttpPost("teams/in-season/{position}/{week}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<int>> UploadWeeklyPlayerTeams(string position, int week) => Enum.TryParse(position, out Position pos) ?
                        Ok(await weeklyDataService.UploadPlayerTeams(_season.CurrentSeason, week, pos)) : BadRequest();

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
        public async Task<IActionResult> UploadADP(string position) => Ok(await seasonDataService.UploadADP(_season.CurrentSeason, position));

        [HttpPost("consensus-projections/{position}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> UploadConsensusProjections(string position) => Ok(await seasonDataService.UploadConsensusProjections(position));

        [HttpPost("consensus-weekly-projections/{week}/{position}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> UploadConsensusWeeklyProjections(int week, string position) => Ok(await weeklyDataService.UploadConsensusWeeklyProjections(week, position, await playersService.GetIgnoreList()));

        [HttpPost("snaps/{position}/{season}/{week}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> UploadWeeklySnapCounts(string position, int season, int week) => Ok(await weeklyDataService.UploadWeeklySnapCounts(season, week, position));

    }
}
