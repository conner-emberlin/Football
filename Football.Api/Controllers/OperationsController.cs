using Football.Data.Interfaces;
using Football.Enums;
using Football.Fantasy.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationsController(IUploadWeeklyDataService weeklyDataService, IFantasyDataService fantasyService, IMatchupAnalysisService matchupAnalysisService, ISettingsService settingsService) : ControllerBase
    {
        [HttpPost("weekly-data/{season}/{week}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadWeeklyData(int season, int week)
        {
            if (season > 0 && week > 0)
            {
                var count = 0;
                count += await weeklyDataService.UploadWeeklyQBData(season, week);
                count += await weeklyDataService.UploadWeeklyRBData(season, week);
                count += await weeklyDataService.UploadWeeklyWRData(season, week);
                count += await weeklyDataService.UploadWeeklyTEData(season, week);
                count += await weeklyDataService.UploadWeeklyKData(season, week);
                count += await weeklyDataService.UploadWeeklyDSTData(season, week);
                count += await weeklyDataService.UploadWeeklyGameResults(season, week);
                count += await weeklyDataService.UploadWeeklyRosterPercentages(season, week, Position.QB.ToString());
                count += await weeklyDataService.UploadWeeklyRosterPercentages(season, week, Position.RB.ToString());
                count += await weeklyDataService.UploadWeeklyRosterPercentages(season, week, Position.WR.ToString());
                count += await weeklyDataService.UploadWeeklyRosterPercentages(season, week, Position.TE.ToString());
                count += await weeklyDataService.UploadWeeklySnapCounts(season, week, Position.QB.ToString());
                count += await weeklyDataService.UploadWeeklySnapCounts(season, week, Position.RB.ToString());
                count += await weeklyDataService.UploadWeeklySnapCounts(season, week, Position.WR.ToString());
                count += await weeklyDataService.UploadWeeklySnapCounts(season, week, Position.TE.ToString());
                return Ok(count);
            }
            return BadRequest();
        }

        [HttpPost("weekly-fantasy/{season}/{week}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadWeeklyFantasy(int season, int week)
        {
            if (season > 0 && week > 0)
            {
                var count = 0;
                count += await fantasyService.PostWeeklyFantasy(season, week, Position.QB);
                count += await fantasyService.PostWeeklyFantasy(season, week, Position.RB);
                count += await fantasyService.PostWeeklyFantasy(season, week, Position.WR);
                count += await fantasyService.PostWeeklyFantasy(season, week, Position.TE);
                count += await fantasyService.PostWeeklyFantasy(season, week, Position.DST);
                count += await fantasyService.PostWeeklyFantasy(season, week, Position.K);
                return Ok(count);
            }
            return BadRequest();
        }

        [HttpPost("matchup-rankings/{week}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadMatchupRankings(int week)
        {
            var count = 0;
            count += await matchupAnalysisService.PostMatchupRankings(Position.QB, week);
            count += await matchupAnalysisService.PostMatchupRankings(Position.RB, week);
            count += await matchupAnalysisService.PostMatchupRankings(Position.WR, week);
            count += await matchupAnalysisService.PostMatchupRankings(Position.TE, week);
            count += await matchupAnalysisService.PostMatchupRankings(Position.DST, week);
            count += await matchupAnalysisService.PostMatchupRankings(Position.K, week);
            return Ok(count);
        }

        [HttpPost("current-season-tunings")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadCurrentSeasonTunings() => Ok(await settingsService.UploadCurrentSeasonTunings());
    }






}
